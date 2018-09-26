using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Asteroids
{
    public class Ship : INotifyPropertyChanged
    {

        public Ship(double xCoordinate, double yCoordinate, double theta, double height, double velocity)
        {
            XCoordinate = xCoordinate;
            YCoordinate = yCoordinate;
            Theta = theta;
            Height = height;
            Velocity = velocity;
        }

        private double xCoordinate;
        private double yCoordinate;
        private double theta;
        private double height;
        private double velocity;

                     
        
        public double XCoordinate
        {
            get { return xCoordinate; }
            set { SetField(ref xCoordinate, value); }
        }
        public double Theta
        {
            get { return theta; }
            set { SetField(ref theta, value); }
        }
        public double YCoordinate
        {
            get { return yCoordinate; }
            set { SetField(ref yCoordinate, value); }
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


