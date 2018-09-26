using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Asteroids
{
    public class MainModelView : INotifyPropertyChanged
    {
        public const int NUM_ROCKS = 5;
        public const int VELOCITY = 4;
        public const int HEIGHT = 100;
        public const int TIME_TO_LIVE = 10;
        private System.Timers.Timer aTimer;
        public int numBullet = 0;

        public MainModelView()
        {
            Random rand = new Random();
            RockObject = new Rock[NUM_ROCKS];
            Player1Ship = new Ship(400, 400, 90, 25, 2*VELOCITY);
            //LaserBullet = new Bullet(400, 400, 10, 20, 3 * VELOCITY);

            Bullet = new Image();
            Canvas.SetTop(Bullet, 500);
            Canvas.SetLeft(Bullet, 500);
            BitmapImage sparkball = new BitmapImage();
            sparkball.BeginInit();
            sparkball.UriSource = new Uri("pack://application:,,,/sparkball.png");
            sparkball.EndInit();
            Bullet.Source = sparkball;

            double startX, startY, velX, velY, angle;

            // initialize the asteroids position and direction
            for (int i = 0; i < NUM_ROCKS; i++)
            {
                startX = 700 * (rand.NextDouble());
                startY = 700 * (rand.NextDouble());
                velX = 2 * (rand.NextDouble()) - 1;
                velY = 2 * (rand.NextDouble()) - 1;
                angle = 0;
                RockObject[i] = new Rock(startX, startY, velX, velY, HEIGHT, VELOCITY, angle);
            }
            SetTimer();
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
                        MoveShip();
                    }));
            }
        }
        protected void MoveShipLeft()
        {
            Player1Ship.Theta -= 3;
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
                        MoveShip();
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

        private ICommand mainModelView_SpaceKeyDown;
        public ICommand MainModelView_SpaceKeyDown
        {

            get
            {
                return mainModelView_SpaceKeyDown
                    ?? (mainModelView_SpaceKeyDown = new ActionCommand(() =>
                    {
                        ShootBullets();
                    }));
            }
        }

        protected void ShootBullets()
        {
                LaserBulletObject[numBullet] = new Bullet(Player1Ship.XCoordinate, Player1Ship.YCoordinate, TIME_TO_LIVE, 20, VELOCITY);
                LaserBulletObject[numBullet].XCoordinate += LaserBulletObject[numBullet].Velocity * Math.Cos((Math.PI * Player1Ship.Theta / 180));
            LaserBulletObject[numBullet].YCoordinate += LaserBulletObject[numBullet].Velocity * Math.Sin((Math.PI * Player1Ship.Theta / 180));
                SetBulletTimer();
            numBullet ++;
        }

        private void SetBulletTimer()
        {
            aTimer = new System.Timers.Timer(1);
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
            aTimer.Elapsed += BulletTimer_Elapsed;
        }

        private void BulletTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            for (int i = 0; i < LaserBulletObject[i].TimeToLive; i++)
            {
            LaserBulletObject[i].XCoordinate += VELOCITY;
            LaserBulletObject[i].YCoordinate += VELOCITY;
            }
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

                RockObject[i].XCoordinate += VELOCITY * RockObject[i].DeltaX;
                RockObject[i].YCoordinate += VELOCITY * RockObject[i].DeltaY;
                RockObject[i].Theta += 3 * RockObject[i].DeltaX;
                
                if (RockObject[i].XCoordinate > 1150)
                    RockObject[i].XCoordinate = -50;
                else if (RockObject[i].XCoordinate <-50)
                    RockObject[i].XCoordinate = 1150;
                if (RockObject[i].YCoordinate > 750)
                    RockObject[i].YCoordinate = -50;
                else if (RockObject[i].YCoordinate < -50)
                    RockObject[i].YCoordinate = 750;
            }
        }
        public Ship Player1Ship { get; private set; }
        public Bullet[] LaserBulletObject { get; private set; }
        public Image Bullet = new Image();
        public Rock[] RockObject { get; private set; }
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

