using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids
{
    public class Bullet: INotifyPropertyChanged
    {
        public Bullet(double xCoordinate, double yCoordinate, double timeToLive, double height, double velocity)
        {
            XCoordinate = xCoordinate;
            YCoordinate = yCoordinate;
            DeltaX = deltaX;
            DeltaY = deltaY;
            Height = height;
            Velocity = velocity;
            Theta = theta;
            TimeToLive = timeToLive;
        }

        private double xCoordinate;
        private double yCoordinate;
        private double deltaX;
        private double deltaY;
        private double timeToLive;
        private double height;
        private double velocity;
        private double theta;

        public double XCoordinate
        {
            get { return xCoordinate; }
            set { SetField(ref xCoordinate, value); }
        }

        public double YCoordinate
        {
            get { return yCoordinate; }
            set { SetField(ref yCoordinate, value); }
        }

        public double DeltaX
        {
            get { return deltaX; }
            set { SetField(ref deltaX, value); }
        }

        public double DeltaY
        {
            get { return deltaY; }
            set { SetField(ref deltaY, value); }
        }

        public double TimeToLive
        {
            get { return timeToLive; }
            set { SetField(ref timeToLive, value); }
        }

        public double Height
        {
            get { return height; }
            set { SetField(ref height, value); }
        }

        public double Velocity
        {
            get { return velocity; }
            set { SetField(ref velocity, value); }
        }

        public double Theta
        {
            get { return theta; }
            set { SetField(ref theta, value); }
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