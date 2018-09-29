﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Asteroids
{
    public class MainModelView : INotifyPropertyChanged
    {
        public const int WINDOWHEIGHT = 800;
        public const int WINDOWWIDTH = 1200;
        public const int NUM_ROCKS = 15;
        public const int NUM_BULLETS = 20;
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
            if (numBullet >= NUM_BULLETS)
                return;
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
            listOfSpaceObjects.Remove(Player1Ship);
            Player1Ship.XCoordinate += Player1Ship.Velocity * Math.Cos((Math.PI*Player1Ship.Theta/180));
            Player1Ship.YCoordinate += Player1Ship.Velocity * Math.Sin((Math.PI*Player1Ship.Theta/180));
            if (Player1Ship.XCoordinate > WINDOWWIDTH - 50)
                Player1Ship.XCoordinate = -50;
            else if (Player1Ship.XCoordinate < -50)
                Player1Ship.XCoordinate = WINDOWWIDTH-50;
            if (Player1Ship.YCoordinate > WINDOWHEIGHT - 50)
                Player1Ship.YCoordinate = -50;
            else if (Player1Ship.YCoordinate < -50)
                Player1Ship.YCoordinate = WINDOWHEIGHT - 50;
            listOfSpaceObjects.Add(Player1Ship);
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
            foreach (SpaceObject r in Rock)
            {
                r.XCoordinate += r.Velocity * Math.Cos(Math.PI * r.Theta / 180);
                r.YCoordinate += r.Velocity * Math.Sin(Math.PI * r.Theta / 180);
                r.OriginalAngle += 3 * Math.Cos(Math.PI * r.Theta / 180);
                
                if (r.XCoordinate > WINDOWWIDTH-50)
                    r.XCoordinate = -50;
                else if (r.XCoordinate <-50)
                    r.XCoordinate = WINDOWWIDTH-50;
                if (r.YCoordinate > WINDOWHEIGHT - 50)
                    r.YCoordinate = -50;
                else if (r.YCoordinate < -50)
                    r.YCoordinate = WINDOWHEIGHT-50;
            }
            
            foreach (SpaceObject b in Bullet)
            {
                if (b != null)
                {
                    b.XCoordinate += b.Velocity * Math.Cos(Math.PI * b.Theta / 180);
                    b.YCoordinate += b.Velocity * Math.Sin(Math.PI * b.Theta / 180);
                    b.OriginalAngle += 3 * Math.Cos(Math.PI * b.Theta / 180);
                    // if it hits the edges, remove the bullet from the list
                    //if (b.XCoordinate > WINDOWWIDTH - 50 || b.XCoordinate < -50 || b.YCoordinate > WINDOWHEIGHT - 50 || b.YCoordinate < -50)
                    //{
                        
                    //    listOfSpaceObjects.Remove(b);
                    //    numBullet--;
                    //}
                }
            }
            
        }
        public SpaceObject Player1Ship { get; private set; }
        public SpaceObject[] Bullet { get; private set; }
        public SpaceObject[] Rock { get; private set; }
        
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
