using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Asteroids
{
    public class MainModelView : INotifyPropertyChanged
    {
        public const int NUM_ROCKS = 15;
        public const int NUM_BULLETS = 10000;
        public const int VELOCITY = 4;
        public const int HEIGHT = 100;
        private System.Timers.Timer aTimer;
        private int numBullet = 0;
        public MainModelView()
        {
            Random rand = new Random();
            Rock = new SpaceObject[NUM_ROCKS];
            Bullet = new SpaceObject[NUM_BULLETS];
            Player1Ship = new SpaceObject('S',600, 400, 25, 2*VELOCITY, -90);
            listOfSpaceObjects = new ObservableCollection<SpaceObject>();
            listOfSpaceObjects.Add(Player1Ship); 
           
            double startX, startY, angle;
            
            // initialize the asteroids position and direction
            for (int i = 0; i < NUM_ROCKS; i++)
            {
                startX = 1100 * (rand.NextDouble());
                startY = 700 * (rand.NextDouble());
                angle = 360 * (rand.NextDouble());
                Rock[i] = new SpaceObject('R',startX, startY, HEIGHT, VELOCITY, angle);
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
        public ICommand MainModelView_WKeyDown
        {

            get
            {
                return mainModelView_WKeyDown
                    ?? (mainModelView_WKeyDown = new ActionCommand(() =>
                    {
                        MoveShip();
                    }));
            }
        }
        protected void MoveShip()
        {
            Player1Ship.XCoordinate += Player1Ship.Velocity * Math.Cos((Math.PI*Player1Ship.Theta/180));
            Player1Ship.YCoordinate += Player1Ship.Velocity * Math.Sin((Math.PI*Player1Ship.Theta/180));
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
            for (int i = 0; i < NUM_ROCKS; i++)
            {

                Rock[i].XCoordinate += VELOCITY * Math.Cos(Math.PI * Rock[i].Theta / 180);
                Rock[i].YCoordinate += VELOCITY * Math.Sin(Math.PI * Rock[i].Theta / 180);
                Rock[i].OriginalAngle += 3 * Math.Cos(Math.PI * Rock[i].Theta / 180);
                
                if (Rock[i].XCoordinate > 1150)
                    Rock[i].XCoordinate = -50;
                else if (Rock[i].XCoordinate <-50)
                    Rock[i].XCoordinate = 1150;
                if (Rock[i].YCoordinate > 750)
                    Rock[i].YCoordinate = -50;
                else if (Rock[i].YCoordinate < -50)
                    Rock[i].YCoordinate = 750;
            }
        }
        public SpaceObject Player1Ship { get; private set; }
        public SpaceObject[] Bullet { get; private set; }
        public SpaceObject[] Rock { get; private set; }
        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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
