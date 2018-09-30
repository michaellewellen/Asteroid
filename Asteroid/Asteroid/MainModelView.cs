using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;

namespace Asteroids
{
    public class MainModelView : INotifyPropertyChanged
    {
        public const int WINDOWHEIGHT = 800;
        public const int WINDOWWIDTH = 1200;
        public const int NUM_ROCKS = 1;
        public const int NUM_BULLETS = 20;
        public const int VELOCITY = 4;
        public const int HEIGHT = 100;
        public const int NUM_SHIPS = 3;
        private System.Timers.Timer aTimer;
        private int numBullet = 0;

       

        public MainModelView()
        {
            Random rand = new Random();
            int numAsteroids = NUM_ROCKS * 4;
            int numShips = NUM_SHIPS;
            // Initialize an array of rocks and bullets each rock will split into 4 smaller ones
            Rock = new SpaceObject[NUM_ROCKS*4];
            Bullet = new SpaceObject[NUM_BULLETS];

            //  Initialize a game as starting with 

            // Initialize a Ship pointing up, no velocity in middle of the board
            Player1Ship = new SpaceObject('S', 600, 400, 25, 0, -90);
            listLock = new object();
            listOfSpaceObjects = new ObservableCollection<SpaceObject>();
            BindingOperations.EnableCollectionSynchronization(listOfSpaceObjects, listLock);

            listOfSpaceObjects.Add(Player1Ship);
            double startX, startY, angle;

            // initialize the asteroids position and direction
            for (int i = 0; i < numAsteroids; i=i+4)
            {
                startX = 1100 * (rand.NextDouble());
                startY = 700 * (rand.NextDouble());
                angle = 360 * (rand.NextDouble());
                Rock[i] = new SpaceObject('R', startX, startY, HEIGHT, VELOCITY, angle);
                listOfSpaceObjects.Add(Rock[i]);
            }
                SetTimer();
            
        }

        private ObservableCollection<SpaceObject> listOfSpaceObjects;
        public ObservableCollection<SpaceObject> ListOfSpaceObjects
        {
            get { return listOfSpaceObjects; }
        }

        private ICommand mainModelView_AKeyDown;
        public ICommand MainModelView_AKeyDown
        {

            get
            {
                return mainModelView_AKeyDown
                    ?? (mainModelView_AKeyDown = new ActionCommand(() =>
                    {
                        MoveShipLeft();
                    }));
            }
        }

        protected void MoveShipLeft()
        {
            Player1Ship.Theta -= 3;
        }

        private ICommand mainModelView_SpaceKeyDown;
        public ICommand MainModelView_SpaceKeyDown
        {

            get
            {
                return mainModelView_SpaceKeyDown
                    ?? (mainModelView_SpaceKeyDown = new ActionCommand(() =>
                    {
                        FireBullet();
                    }));
            }
        }

        public void FireBullet()
        {
            if (numBullet >= NUM_BULLETS)
                numBullet = 0;
            Bullet[numBullet] = new SpaceObject('B', Player1Ship.XCoordinate, Player1Ship.YCoordinate, 10, 4 * VELOCITY, Player1Ship.Theta);
            listOfSpaceObjects.Add(Bullet[numBullet]);
            numBullet++;
        }

        

        private ICommand mainModelView_DKeyDown;
        public ICommand MainModelView_DKeyDown
        {

            get
            {
                return mainModelView_DKeyDown
                    ?? (mainModelView_DKeyDown = new ActionCommand(() =>
                    {
                        MoveShipRight();
                    }));
            }
        }
        protected void MoveShipRight()
        {
            Player1Ship.Theta += 3;
        }

        private ICommand mainModelView_WKeyDown;
        private object _listLock;

        public ICommand MainModelView_WKeyDown
        {

            get
            {
                return mainModelView_WKeyDown
                    ?? (mainModelView_WKeyDown = new ActionCommand(() =>
                    {
                        Player1Ship.Velocity = 2*VELOCITY;
                        MoveShip();
                    }));
            }
        }
        protected void MoveShip()
        {
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

            //// Move the rocks
            foreach (SpaceObject r in Rock)
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


            ////  Move the bullets
            foreach (SpaceObject b in Bullet)
            {
                if (b != null)
                {
                    b.XCoordinate += b.Velocity * Math.Cos(Math.PI * b.Theta / 180);
                    b.YCoordinate += b.Velocity * Math.Sin(Math.PI * b.Theta / 180);
                }
            }

            // Check for collisions. If bullet hits rock, remove/split rock if rock hits ship, reset ship
            for (int i = 0; i < NUM_ROCKS * 4; i++)
            {
                if (Rock[i] != null)
                {
                    // Check against Bullets
                    foreach (SpaceObject b in Bullet)
                    {
                        if (b != null)
                        {
                            if (((b.XCoordinate > Rock[i].XCoordinate && b.XCoordinate < Rock[i].XCoordinate + 100) || (b.XCoordinate + 5 > Rock[i].XCoordinate && b.XCoordinate + 5 < Rock[i].XCoordinate + 100)) &&
                                ((b.YCoordinate > Rock[i].YCoordinate && b.YCoordinate < Rock[i].YCoordinate + 100) || (b.YCoordinate + 5 > Rock[i].YCoordinate && b.YCoordinate + 5 < Rock[i].YCoordinate + 100)))
                            {
                                RemoveSpaceObject(Rock[i], i);
                            }
                        }
                    }

                    //        // Check against Ship
                    //        if ((Player1Ship.XCoordinate < r.XCoordinate + 100 || Player1Ship.XCoordinate > r.XCoordinate) && (Player1Ship.YCoordinate < r.YCoordinate + 100 || Player1Ship.YCoordinate > r.YCoordinate))
                    //        {
                    //            ShipHit(); // either game over, or reset the ship
                    //        }
                }
            }
        }
                


        public void RemoveSpaceObject(SpaceObject so, int i)
        {
            if(so.NumberOfHits == 0) // first hit
            {
                lock(listLock)
                {
                    listOfSpaceObjects.Remove(Rock[i]);
                    Rock[i] = new SpaceObject('R', so.XCoordinate, so.XCoordinate, so.Height / 2, so.Velocity, so.Theta + 30);
                    Rock[i + 2] = new SpaceObject('R', so.XCoordinate, so.XCoordinate, so.Height / 2, so.Velocity, so.Theta - 30);
                    Rock[i].NumberOfHits++;
                    Rock[i + 2].NumberOfHits++;
                    listOfSpaceObjects.Add(Rock[i]);
                    listOfSpaceObjects.Add(Rock[i + 2]);
                }
            }
            else if(so.NumberOfHits == 1) // second hit
            {
                Rock[i] = new SpaceObject('R', so.XCoordinate, so.XCoordinate, so.Height / 2, so.Velocity, so.Theta + 30);
                Rock[i + 1] = new SpaceObject('R', so.XCoordinate, so.XCoordinate, so.Height / 2, so.Velocity, so.Theta - 30);
                Rock[i].NumberOfHits++;
                Rock[i + 1].NumberOfHits++;
            }

            else
            {
                Rock[i].Height = 0;
            }
            
        }

        public void ShipHit()
        {
            // Maybe some explosion noise, and an explosion graphic?
            if (Player1Ship.NumberOfHits < 3)
            {
                Player1Ship.Velocity = 0;
                Player1Ship.XCoordinate = 600;
                Player1Ship.YCoordinate = 400;
            }
            else
            {
                listOfSpaceObjects.Remove(Player1Ship);
                SpaceObject temp = new SpaceObject('G', 400, 300, 200, 0, 0);
                listOfSpaceObjects.Add(temp);

            }
        }
        public SpaceObject Player1Ship { get; private set; }
        public SpaceObject[] Bullet { get; private set; }
        public SpaceObject[] Rock { get; private set; }
        public object listLock { get; private set; }
        
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
