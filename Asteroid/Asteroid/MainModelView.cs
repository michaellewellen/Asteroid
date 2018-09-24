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

        private void SetTimer()
        {
            aTimer = new System.Timers.Timer(10);
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
            aTimer.Elapsed += ATimer_Elapsed;
        }

        private void ATimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            for (int i = 0; i < NUM_ROCKS; i++)
            {
                if (i == 0)
                    Debug.WriteLine($"Rock0 x={RockObject[0].XCoordinate} y={RockObject[0].YCoordinate}");
                RockObject[i].XCoordinate += VELOCITY * RockObject[i].DeltaX;
                RockObject[i].YCoordinate += VELOCITY * RockObject[i].DeltaY;
                if (RockObject[i].XCoordinate > 700)
                    RockObject[i].XCoordinate = 0;
                if (RockObject[i].YCoordinate > 700)
                    RockObject[i].YCoordinate = 0;
            }
        }

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
