﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Asteroids
{
    public class MainModelView : INotifyPropertyChanged
    {
        public const int WINDOWHEIGHT = 800;
        public const int WINDOWWIDTH = 1450;
        public const int NUM_ROCKS = 5;
        public const int VELOCITY = 4;
        public const int HEIGHT = 100;
        private System.Timers.Timer aTimer;
     


        // MainModelView Constructor
        public MainModelView()
        {
            Random rand = new Random();
            numAsteroids = NUM_ROCKS;

            // Initialize a Ship pointing up, no velocity in middle of the board
            Player1Ship = new SpaceObject('S', 600, 400, 25, 0, -90);
            Player2Ship = new SpaceObject('E', 800, 400, 25, 0, -90);

            // Make a list of space objects and add player1 and player 2
            listOfSpaceObjects = new ObservableCollection<SpaceObject>();
            ListOfSpaceObjects.Add(Player1Ship);
            ListOfSpaceObjects.Add(Player2Ship);
            
            // Variables to initialize the array of asteroids
            double startX, startY, angle;

            // initialize the asteroids position and direction
            for (int i = 0; i < numAsteroids; i++)
            {
                startX = WINDOWWIDTH * (rand.NextDouble());
                startY = WINDOWHEIGHT * (rand.NextDouble());
                angle = 360 * (rand.NextDouble());
                var rock = new SpaceObject('R', startX, startY, HEIGHT, VELOCITY / 2, angle);
                listOfSpaceObjects.Add(rock);
            }
            EstablishConnections();
            SetTimer();
        }


        static TcpClient player1Client;
        static TcpClient player2Client;
        static NetworkStream player1Stream;
        static NetworkStream player2Stream;

        public void EstablishConnections()
        {
            TcpListener listener = new TcpListener(IPAddress.Loopback,30000);
            listener.Start();
            player1Client = listener.AcceptTcpClient();
            player1Stream = player1Client.GetStream();
            Console.WriteLine("Player1 Connected");

            player2Client = listener.AcceptTcpClient();
            player2Stream = player2Client.GetStream();
            Console.WriteLine("Player2 Connected");


        }


        // Switch statement for keys depressed
        internal void KeyPressed(Key key)
        {
            switch (key)
            {
                case Key.A:
                    MoveShipLeft(true);
                    break;
                case Key.D:
                    MoveShipRight(true);
                    break;
                case Key.W:
                    MoveShip(true);
                    break;
                case Key.Space:
                    FireBullet(true);
                    break;
                case Key.NumPad4:
                    EnemyMoveShipLeft(true);
                    break;
                case Key.NumPad6:
                    EnemyMoveShipRight(true);
                    break;
                case Key.NumPad8:
                    EnemyMoveShip(true);
                    break;
                case Key.NumPad0:
                    FireEnemyBullet(true);
                    break;
                default: break;
            }
        }
        internal void KeyReleased(Key key)
        {
            switch (key)
            {
                case Key.A:
                    MoveShipLeft(false);
                    break;
                case Key.D:
                    MoveShipRight(false);
                    break;
                case Key.W:
                    MoveShip(false);
                    break;
                case Key.Space:
                    FireBullet(false);
                    break;
                case Key.NumPad4:
                    EnemyMoveShipLeft(false);
                    break;
                case Key.NumPad6:
                    EnemyMoveShipRight(false);
                    break;
                case Key.NumPad8:
                    EnemyMoveShip(false);
                    break;
                case Key.NumPad0:
                    FireEnemyBullet(false);
                    break;
                default: break;
            }
        }
        protected void MoveShipLeft(bool change)
        { left = change; }

        protected void MoveShipRight(bool change)
        { right = change; }

        public void FireBullet(bool change)
        { shoot = change; }

        protected void MoveShip(bool change)
        {
            accel = change;
            if (change == false) // Change image when accelerating
                    { Player1Ship._Image = new BitmapImage(new Uri(@"Resources\ship.png", UriKind.Relative)); }
            else    { Player1Ship._Image = new BitmapImage(new Uri(@"Resources\shipwithflame.png", UriKind.Relative)); }
        }

        protected void EnemyMoveShipLeft(bool change)
        { enemyLeft = change; }

        protected void EnemyMoveShipRight(bool change)
        { enemyRight = change; }

        public void FireEnemyBullet(bool change)
        { enemyShoot = change; }

        protected void EnemyMoveShip(bool change)
        {
            enemyAccel = change;
            if (change == false) // Change image when accelerating
            {
                Player2Ship._Image = new BitmapImage(new Uri(@"Resources\p2.png", UriKind.Relative));
            }
            else
                Player2Ship._Image = new BitmapImage(new Uri(@"Resources\p2withflame.png", UriKind.Relative));
        }


        private void SetTimer()
        {
            aTimer = new System.Timers.Timer(1);
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
            aTimer.Elapsed += ATimer_Elapsed;
        }


        // An Elapsed time ticker based on SetTimer,
        private void ATimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
           try
           {
                foreach (SpaceObject r in Rocks.ToList())
                {
                    if (r != null)
                    {
                        r.XCoordinate += r.Velocity * Math.Cos(Math.PI * r.OriginalAngle / 180);
                        r.YCoordinate += r.Velocity * Math.Sin(Math.PI * r.OriginalAngle / 180);
                        r.Theta += 3 * Math.Cos(Math.PI * r.OriginalAngle / 180);

                        if (r.XCoordinate > WINDOWWIDTH - 50)
                            r.XCoordinate = -50;
                        else if (r.XCoordinate < -50)
                            r.XCoordinate = WINDOWWIDTH - 50;
                        if (r.YCoordinate > WINDOWHEIGHT - 50)
                            r.YCoordinate = -50;
                        else if (r.YCoordinate < -50)
                            r.YCoordinate = WINDOWHEIGHT - 50;
                    }
                }
                // Player1 Score increases for every mili second your alive
                Player1Ship.Score++;

                //// Move player 1
                if (accel)
                {
                    if (Player1Ship.Velocity <= 3 * VELOCITY)
                        Player1Ship.Velocity += .1;
                }
                else
                {
                    if (Player1Ship.Velocity <= 0)
                        Player1Ship.Velocity = 0;
                    else
                        Player1Ship.Velocity -= .08;
                }

                if (left)
                {
                    Player1Ship.Theta -= 3;
                }
                if (right)
                {
                    Player1Ship.Theta += 3 ;
                }

                if (shoot)  // add a bullet
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        var bullet = new SpaceObject('B', Player1Ship.XCoordinate + 8, Player1Ship.YCoordinate + 8, 10, 4 * VELOCITY, Player1Ship.Theta);
                        listOfSpaceObjects.Add(bullet);
                    }));
                }


                Player1Ship.XCoordinate += Player1Ship.Velocity * Math.Cos((Math.PI * Player1Ship.Theta / 180));
                Player1Ship.YCoordinate += Player1Ship.Velocity * Math.Sin((Math.PI * Player1Ship.Theta / 180));
                if (Player1Ship.XCoordinate > WINDOWWIDTH - 50)
                    Player1Ship.XCoordinate = -50;
                else if (Player1Ship.XCoordinate < -50)
                    Player1Ship.XCoordinate = WINDOWWIDTH - 50;
                if (Player1Ship.YCoordinate > WINDOWHEIGHT - 50)
                    Player1Ship.YCoordinate = -50;
                else if (Player1Ship.YCoordinate < -50)
                    Player1Ship.YCoordinate = WINDOWHEIGHT - 50;

                // Score increases for every millisecodn you're alive
                Player2Ship.Score++;
                // Move player 2 Ship
                if (enemyAccel)
                {
                    if (Player2Ship.Velocity <= 3 * VELOCITY)
                        Player2Ship.Velocity += .1;
                }
                else
                {
                    if (Player2Ship.Velocity <= 0)
                        Player2Ship.Velocity = 0;
                    else
                        Player2Ship.Velocity -= .08;
                }

                if (enemyLeft)
                {
                    Player2Ship.Theta -= 3;
                }
                if (enemyRight)
                {
                    Player2Ship.Theta += 3;
                }
                if (enemyShoot)  // add a enemy bullet
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        var bullet = new SpaceObject('F', Player2Ship.XCoordinate + 8, Player2Ship.YCoordinate + 8, 10, 4 * VELOCITY, Player2Ship.Theta);
                        listOfSpaceObjects.Add(bullet);
                    }));
                }
                Player2Ship.XCoordinate += Player2Ship.Velocity * Math.Cos((Math.PI * Player2Ship.Theta / 180));
                Player2Ship.YCoordinate += Player2Ship.Velocity * Math.Sin((Math.PI * Player2Ship.Theta / 180));
                if (Player2Ship.XCoordinate > WINDOWWIDTH - 50)
                    Player2Ship.XCoordinate = -50;
                else if (Player2Ship.XCoordinate < -50)
                    Player2Ship.XCoordinate = WINDOWWIDTH - 50;
                if (Player2Ship.YCoordinate > WINDOWHEIGHT - 50)
                    Player2Ship.YCoordinate = -50;
                else if (Player2Ship.YCoordinate < -50)
                    Player2Ship.YCoordinate = WINDOWHEIGHT - 50;


                ////  Move the player1Bullets
                for (int i = 0; i < Bullets.Count(); i++)

                {
                    var b = Bullets.Skip(i).First();
                    if (b != null)
                    {
                        b.XCoordinate += b.Velocity * Math.Cos(Math.PI * b.Theta / 180);
                        b.YCoordinate += b.Velocity * Math.Sin(Math.PI * b.Theta / 180);
                        if (b.XCoordinate > WINDOWWIDTH - 50 || b.XCoordinate < -50 || b.YCoordinate > WINDOWHEIGHT - 50 || b.YCoordinate < -50)
                            RemoveSpaceObject(b);
                    }
                }
                // Move enemy bullets
                for (int i = 0; i < EnemyBullets.Count(); i++)

                {
                    var b = EnemyBullets.Skip(i).First();
                    if (b != null)
                    {
                        b.XCoordinate += b.Velocity * Math.Cos(Math.PI * b.Theta / 180);
                        b.YCoordinate += b.Velocity * Math.Sin(Math.PI * b.Theta / 180);
                        if (b.XCoordinate > WINDOWWIDTH - 50 || b.XCoordinate < -50 || b.YCoordinate > WINDOWHEIGHT - 50 || b.YCoordinate < -50)
                            RemoveSpaceObject(b);
                    }
                }


                // Check for collisions. If bullet hits rock, remove/split rock if rock hits ship, reset ship
                foreach (SpaceObject rock in Rocks.ToList())
                {
                    if (rock != null)
                    {
                        foreach (SpaceObject b in Bullets.ToList())
                        {
                            //var b = Bullets.Skip(j).First();
                            if (b != null)
                            {
                                if (((b.XCoordinate > rock.XCoordinate && b.XCoordinate < rock.XCoordinate + rock.Height) ||
                                    (b.XCoordinate + 5 > rock.XCoordinate && b.XCoordinate + 5 < rock.XCoordinate + rock.Height)) &&
                                    ((b.YCoordinate > rock.YCoordinate && b.YCoordinate < rock.YCoordinate + rock.Height) ||
                                    (b.YCoordinate + 5 > rock.YCoordinate && b.YCoordinate + 5 < rock.YCoordinate + rock.Height)))
                                {
                                    Player1Ship.Score += Math.Pow((rock.NumberOfHits + 1), 2) * 100;
                                    RemoveSpaceObject(rock);
                                    RemoveSpaceObject(b);
                                }

                                
                            }
                        }

                        // Chcek against Player2 Bullets
                        foreach (SpaceObject b in EnemyBullets.ToList())
                        //for (int j = 0; j < EnemyBullets.Count(); j++)
                        {
                            //var b = EnemyBullets.Skip(j).First();
                            if (b != null)
                            {
                                if (((b.XCoordinate > rock.XCoordinate && b.XCoordinate < rock.XCoordinate + rock.Height) ||
                                    (b.XCoordinate + 5 > rock.XCoordinate && b.XCoordinate + 5 < rock.XCoordinate + rock.Height)) &&
                                    ((b.YCoordinate > rock.YCoordinate && b.YCoordinate < rock.YCoordinate + rock.Height) ||
                                    (b.YCoordinate + 5 > rock.YCoordinate && b.YCoordinate + 5 < rock.YCoordinate + rock.Height)))
                                {
                                    Player2Ship.Score += Math.Pow((rock.NumberOfHits + 1), 2) * 100;
                                    RemoveSpaceObject(rock);
                                    RemoveSpaceObject(b);
                                }
                                
                            }
                        }

                        if (((Player1Ship.XCoordinate > rock.XCoordinate && Player1Ship.XCoordinate < rock.XCoordinate + rock.Height) ||
                                    (Player1Ship.XCoordinate + Player1Ship.Height > rock.XCoordinate && Player1Ship.XCoordinate + Player1Ship.Height < rock.XCoordinate + rock.Height)) &&
                                    ((Player1Ship.YCoordinate > rock.YCoordinate && Player1Ship.YCoordinate < rock.YCoordinate + rock.Height) ||
                                    (Player1Ship.YCoordinate + Player1Ship.Height > rock.YCoordinate && Player1Ship.YCoordinate + Player1Ship.Height < rock.YCoordinate + rock.Height)))
                        {
                            ShipHit();
                        }
                        if (((Player2Ship.XCoordinate > rock.XCoordinate && Player2Ship.XCoordinate < rock.XCoordinate + rock.Height) ||
                                    (Player2Ship.XCoordinate + Player2Ship.Height > rock.XCoordinate && Player2Ship.XCoordinate + Player2Ship.Height < rock.XCoordinate + rock.Height)) &&
                                    ((Player2Ship.YCoordinate > rock.YCoordinate && Player2Ship.YCoordinate < rock.YCoordinate + rock.Height) ||
                                    (Player2Ship.YCoordinate + Player2Ship.Height > rock.YCoordinate && Player1Ship.YCoordinate + Player1Ship.Height < rock.YCoordinate + rock.Height)))
                        {
                            EnemyShipHit();
                        }
                    }
                }
                foreach (SpaceObject b in Bullets)
                {
                    if (((b.XCoordinate > Player2Ship.XCoordinate && b.XCoordinate < Player2Ship.XCoordinate + Player2Ship.Height) ||
                                        (b.XCoordinate + 5 > Player2Ship.XCoordinate && b.XCoordinate + 5 < Player2Ship.XCoordinate + Player2Ship.Height)) &&
                                        ((b.YCoordinate > Player2Ship.YCoordinate && b.YCoordinate < Player2Ship.YCoordinate + Player2Ship.Height) ||
                                        (b.YCoordinate + 5 > Player2Ship.YCoordinate && b.YCoordinate + 5 < Player2Ship.YCoordinate + Player2Ship.Height)))
                    {
                        RemoveSpaceObject(b);
                        Player1Ship.Score += 500;
                        EnemyShipHit();
                    }
                }
                foreach (SpaceObject b in EnemyBullets)
                if (((b.XCoordinate > Player1Ship.XCoordinate && b.XCoordinate < Player1Ship.XCoordinate + Player1Ship.Height) ||
                                    (b.XCoordinate + 5 > Player1Ship.XCoordinate && b.XCoordinate + 5 < Player1Ship.XCoordinate + Player1Ship.Height)) &&
                                    ((b.YCoordinate > Player1Ship.YCoordinate && b.YCoordinate < Player1Ship.YCoordinate + Player1Ship.Height) ||
                                    (b.YCoordinate + 5 > Player1Ship.YCoordinate && b.YCoordinate + 5 < Player1Ship.YCoordinate + Player1Ship.Height)))
                {
                    RemoveSpaceObject(b);
                    Player2Ship.Score += 5000;
                    ShipHit();
                }

            }
            catch (System.InvalidOperationException)
            {
                return;
            }
            
            // Checking for bullets hitting ships and ships hitting ships.


        }



        public void RemoveSpaceObject(SpaceObject so)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                ListOfSpaceObjects.Remove(so);
                // If the rock is not at its smallest size....
                if(so.Type == 'R' && so.NumberOfHits < 2)
                {
                    var rock = new SpaceObject('R', so.XCoordinate, so.YCoordinate,so.Height/2,so.Velocity*1.5, so.Theta + 30);
                    rock.CollisionBoxSize = (int)so.Height / 2;
                    rock.NumberOfHits = so.NumberOfHits + 1;
                    listOfSpaceObjects.Add(rock);
                    rock = new SpaceObject('R', so.XCoordinate, so.YCoordinate, so.Height / 2, so.Velocity * 1.5, so.Theta - 30);
                    rock.CollisionBoxSize = (int)so.Height / 2;
                    rock.NumberOfHits = so.NumberOfHits + 1;
                    listOfSpaceObjects.Add(rock);
                }
            }));
        }

        public void ShipHit()
        {
            // Maybe some explosion noise, and an explosion graphic?

            {
                Random rand = new Random();
                double temp = WINDOWWIDTH * (rand.NextDouble())+ Player1Ship.CollisionBoxSize;
                Player1Ship.XCoordinate = temp;
                temp = (WINDOWHEIGHT) * (rand.NextDouble()) + Player1Ship.CollisionBoxSize;
                Player1Ship.YCoordinate = temp;
                temp = 360 * (rand.NextDouble());
                Player1Ship.Theta = temp;
                Player1Ship.Velocity = 0;
                Player1Ship.Score = 0;
            }
        }

        public void EnemyShipHit()
        {
            // Maybe some explosion noise, and an explosion graphic?

            {
                Random rand = new Random();
                double temp = WINDOWWIDTH * (rand.NextDouble()) + Player2Ship.CollisionBoxSize;
                Player2Ship.XCoordinate = temp;
                temp = WINDOWHEIGHT * (rand.NextDouble()) + Player2Ship.CollisionBoxSize;
                Player2Ship.YCoordinate = temp;
                temp = 360 * (rand.NextDouble());
                Player2Ship.Theta = temp;
                Player2Ship.Velocity = 0;
                Player2Ship.Score = 0;
            }
        }
        private ObservableCollection<SpaceObject> listOfSpaceObjects;
        public ObservableCollection<SpaceObject> ListOfSpaceObjects
        {
            get { return listOfSpaceObjects; }
        }
        public SpaceObject Player1Ship { get; private set; }
        public SpaceObject Player2Ship { get; private set; }
        public IEnumerable<SpaceObject> EnemyBullets => ListOfSpaceObjects.Where(so => so.Type == 'F');
        public IEnumerable<SpaceObject> Bullets => ListOfSpaceObjects.Where(so => so.Type == 'B');
        public IEnumerable<SpaceObject> Rocks => ListOfSpaceObjects.Where(so => so.Type == 'R');
        //public SpaceObject[] Rock { get; private set; }
        public object listLock { get; private set; }
        public int numAsteroids { get; private set; }
        public int numShips { get; private set; }
        public SpaceObject temp { get; private set; }
        public SpaceObject winGame { get; private set; }
        public bool shoot { get; private set; }
        public bool accel { get; private set; }
        public bool left { get; private set; }
        public bool right { get; private set; }
        public bool enemyAccel { get; private set; }
        public bool enemyLeft { get; private set; }
        public bool enemyRight { get; private set; }
        public bool enemyShoot { get; private set; }

        

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion


    }
}
