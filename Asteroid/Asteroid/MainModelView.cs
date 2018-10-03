using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
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
        public const int WINDOWWIDTH = 1200;
        public const int NUM_ROCKS = 20;
        public const int NUM_BULLETS = 1;
        public const int VELOCITY = 4;
        public const int HEIGHT = 100;
        public const int NUM_SHIPS = 3;
        private System.Timers.Timer aTimer;
        private int numBullet = 0;
     



        public MainModelView()
        {
            Random rand = new Random();
            numAsteroids = NUM_ROCKS;
            numShips = NUM_SHIPS;
            // Initialize an array of rocks and bullets each rock will split into 4 smaller ones

            temp = new SpaceObject('G', 400, 250, 0, 0, 0);
            winGame = new SpaceObject('A', 400, 250, 0, 0, 0);

            // Initialize a Ship pointing up, no velocity in middle of the board
            Player1Ship = new SpaceObject('S', 600, 400, 25, 0, -90);
            listLock = new object();
            listOfSpaceObjects = new ObservableCollection<SpaceObject>();
            ListOfSpaceObjects.CollectionChanged += (s, args) => ObjectCount = ListOfSpaceObjects.Count;
            BindingOperations.EnableCollectionSynchronization(listOfSpaceObjects, listLock);

            listOfSpaceObjects.Add(Player1Ship);
            listOfSpaceObjects.Add(temp);
            listOfSpaceObjects.Add(winGame);
            double startX, startY, angle;

            // initialize the asteroids position and direction
            for (int i = 0; i < numAsteroids; i++)
            {
                startX = 1100 * (rand.NextDouble());
                startY = 700 * (rand.NextDouble());
                angle = 360 * (rand.NextDouble());
                var rock = new SpaceObject('R', startX, startY, HEIGHT, VELOCITY / 2, angle);
                listOfSpaceObjects.Add(rock);
            }
            SetTimer();
        }

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
                default: break;
            }
        }

        public void FireBullet(bool change)
        {
            shoot = change;
            if (shoot)
            {
                if (numBullet >= NUM_BULLETS)
                    numBullet = 0;

                var bullet = new SpaceObject('B', Player1Ship.XCoordinate + 8, Player1Ship.YCoordinate + 8, 10, 4 * VELOCITY, Player1Ship.Theta);
                listOfSpaceObjects.Add(bullet);
                numBullet++;
            }
        }

        protected void MoveShipLeft(bool change)
        {
            left = change;
            
        }

        protected void MoveShipRight(bool change)
        {
            right = change;
            
        }

        protected void MoveShip(bool change)
        {
            accel = change;
            if(change == false)
                Player1Ship._Image= new BitmapImage(new Uri(@"Resources\ship.png", UriKind.Relative));
            else
                Player1Ship._Image= new BitmapImage(new Uri(@"Resources\shipwithflame.png", UriKind.Relative));
        }
        private ObservableCollection<SpaceObject> listOfSpaceObjects;
        public ObservableCollection<SpaceObject> ListOfSpaceObjects
        {
            get { return listOfSpaceObjects; }
        }

        private int objectCount;
        public int ObjectCount
        {
            get { return objectCount; }
            set { SetField(ref objectCount, value); }
        }




        private void SetTimer()
        {
            aTimer = new System.Timers.Timer(1);
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
            aTimer.Elapsed += ATimer_Elapsed;
        }

        private void ATimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // These are win/lose graphics that will not be part of the final project.
            //// Check for Game Over
            //if (numShips == 0)
            //    temp.Height = 200;
            //// Check for win
            //if (numAsteroids <= 0)
            //    winGame.Height = 200;
            //// Move the rocks
            foreach (SpaceObject r in Rocks)
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

            //// Move the ship
            if (accel)
            {
                if (Player1Ship.Velocity <= 2*VELOCITY)
                    Player1Ship.Velocity += .1;
            }
            else
            {
                if (Player1Ship.Velocity <= 0)
                    Player1Ship.Velocity = 0;
                else
                    Player1Ship.Velocity -= .1;
            }

            if(left)
            {
                Player1Ship.Theta -= 3;
            }
            if (right)
            {
                Player1Ship.Theta += 3;
            }


            Player1Ship.XCoordinate += Player1Ship.Velocity * Math.Cos((Math.PI * Player1Ship.Theta / 180));
                Player1Ship.YCoordinate += Player1Ship.Velocity * Math.Sin((Math.PI * Player1Ship.Theta / 180));
                if (Player1Ship.XCoordinate > 1150)
                    Player1Ship.XCoordinate = -50;
                else if (Player1Ship.XCoordinate < -50)
                    Player1Ship.XCoordinate = 1150;
                if (Player1Ship.YCoordinate > 750)
                    Player1Ship.YCoordinate = -50;
                else if (Player1Ship.YCoordinate < -50)
                    Player1Ship.YCoordinate = 750;


            ////  Move the bullets
            for (int i =0; i<Bullets.Count();i++)
            
            {
                var b = Bullets.Skip(i).First();
                if (b != null)
                {
                    b.XCoordinate += b.Velocity * Math.Cos(Math.PI * b.Theta / 180);
                    b.YCoordinate += b.Velocity * Math.Sin(Math.PI * b.Theta / 180);
                    if (b.XCoordinate > 1150 || b.XCoordinate < -50 || b.YCoordinate > 750 || b.YCoordinate < -50)
                        RemoveSpaceObject(b);
                }
            }

            // Check for collisions. If bullet hits rock, remove/split rock if rock hits ship, reset ship
            for(int i = 0; i < Rocks.Count(); i++)
            {
                var rock = Rocks.Skip(i).First();
                if (rock != null)
                {
                    // Check against Bullets
                    for (int j = 0; j < Bullets.Count(); j++)
                    {
                        var b = Bullets.Skip(j).First();
                        if (b != null)
                        {
                            if (((b.XCoordinate > rock.XCoordinate && b.XCoordinate < rock.XCoordinate + rock.Height) ||
                                (b.XCoordinate + 5 > rock.XCoordinate && b.XCoordinate + 5 < rock.XCoordinate + rock.Height)) &&
                                ((b.YCoordinate > rock.YCoordinate && b.YCoordinate < rock.YCoordinate + rock.Height) ||
                                (b.YCoordinate + 5 > rock.YCoordinate && b.YCoordinate + 5 < rock.YCoordinate + rock.Height)))
                            {
                                b.Height = 0;
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
                        Player1Ship.XCoordinate = 600;
                        Player1Ship.YCoordinate = 400;
                        Player1Ship.Theta = -90;
                        numShips--;
                        if (numShips == 0)
                        {
                            //Display Game Over Graphic
                        }
                    }
                }
            }
            
        }



        public void RemoveSpaceObject(SpaceObject so)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                ListOfSpaceObjects.Remove(so);
            }));
        }

        public void ShipHit()
        {
            // Maybe some explosion noise, and an explosion graphic?

            {
                Player1Ship.Velocity = 0;
                Player1Ship.XCoordinate = 600;
                Player1Ship.YCoordinate = 400;
                numShips--;
            }
            //else
            //{
            //    listofspaceobjects.remove(player1ship);
            //    spaceobject temp = new spaceobject('g', 400, 300, 200, 0, 0);
            //    listofspaceobjects.add(temp);

            //}
        }
        public SpaceObject Player1Ship { get; private set; }
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

        public class ActionCommand : ICommand
        {
            private readonly Action _action;

            public ActionCommand(Action action)
            {
                _action = action;
            }

            public void Execute(object parameter)
            {
                _action();
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;
        }
        

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
