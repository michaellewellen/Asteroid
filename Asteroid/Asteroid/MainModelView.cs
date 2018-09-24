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
using System.Windows.Input;

namespace Asteroids
{
    public class MainModelView : INotifyPropertyChanged
    {
        public const int NUM_ROCKS = 5;
        public const double VELOCITY = 4;
        public const int HEIGHT = 100;
        private System.Timers.Timer aTimer;

        public MainModelView()
        {
            Random rand = new Random();
            RockObject = new Rock[NUM_ROCKS];
            Player1Ship = new Ship(400, 400, 90, 25, 0);
            double startX, startY, velX, velY;

            // initialize the asteroids position and direction
            for (int i = 0; i < NUM_ROCKS; i++)
            {
                startX = 700 * (rand.NextDouble());
                startY = 700 * (rand.NextDouble());
                velX = 2 * (rand.NextDouble()) - 1;
                velY = 2 * (rand.NextDouble()) - 1;
                RockObject[i] = new Rock(startX, startY, velX, velY, HEIGHT, VELOCITY);
            }


            SetTimer();

        }



        protected void MainModelView_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A || e.SystemKey == Key.A)
            { Player1Ship.Theta += 1; }


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
                if (i == 0)
                    RockObject[i].XCoordinate += VELOCITY * RockObject[i].DeltaX;
                RockObject[i].YCoordinate += VELOCITY * RockObject[i].DeltaY;
                if (RockObject[i].XCoordinate > 700)
                    RockObject[i].XCoordinate = 0;
                else if (RockObject[i].XCoordinate < 0)
                    RockObject[i].XCoordinate = 700;
                if (RockObject[i].YCoordinate > 700)
                    RockObject[i].YCoordinate = 0;
                else if (RockObject[i].YCoordinate < 0)
                    RockObject[i].YCoordinate = 700;
            }
        }
        public Ship Player1Ship{get; private set;}

        public Rock[] RockObject { get; private set; }
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
