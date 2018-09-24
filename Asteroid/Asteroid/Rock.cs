using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Asteroids
{
    public class Rock : INotifyPropertyChanged
    {
        public Rock(double xCoordinate, double yCoordinate, double deltaX, double deltaY, double height, double velocity)
        {
            XCoordinate = xCoordinate;
            YCoordinate = yCoordinate;
            DeltaX = deltaX;
            DeltaY = deltaY;
            Height = height;
            Velocity = velocity;
        }

        private double xCoordinate;
        private double yCoordinate;
        private double deltaX;
        private double deltaY;
        private double height;
        private double velocity;

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
