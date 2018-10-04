using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Asteroids
{
    public class SpaceObject :INotifyPropertyChanged
    {
        static private int nextObjectID=0;
        private readonly int objectID = nextObjectID++;
        public char Type { get; }

        public SpaceObject(char type, double xCoordinate, double yCoordinate, double height, double velocity, double theta)
        {
            this.Type = type;
            if (type == 'S')
            {
                _Image = new BitmapImage(new Uri(@"Resources\ship.png", UriKind.Relative));
                CollisionBoxSize = 25;
            }
            else if (type == 'B')
            {
                _Image = new BitmapImage(new Uri(@"Resources\sparkball.png", UriKind.Relative));
                CollisionBoxSize = 5;
            }
            else if (type == 'F')
            {
                _Image = new BitmapImage(new Uri(@"Resources\fireball.png", UriKind.Relative));
                CollisionBoxSize = 5;
            }
            else if (type == 'R')
            {
                _Image = new BitmapImage(new Uri(@"Resources\Asteroid.png", UriKind.Relative));
                CollisionBoxSize = 100;
            }

            else if (type == 'G')
            {
                _Image = new BitmapImage(new Uri(@"Resources\gameover.png", UriKind.Relative));
                CollisionBoxSize = 0;
            }

            else if (type == 'A')
            {
                _Image = new BitmapImage(new Uri(@"Resources\youwin.png", UriKind.Relative));
                CollisionBoxSize = 0;
            }

            else if (type == 'E')
            {
                _Image = new BitmapImage(new Uri(@"Resources\p2.png", UriKind.Relative));
                CollisionBoxSize = 25;
            }
            XCoordinate = xCoordinate;
            YCoordinate = yCoordinate;
            OriginalAngle = theta;
            Theta = theta;
            Height = height;
            CX = Height / 2;
            CY = Height / 2;
            Velocity = velocity;
            NumberOfHits = 0;
        }

        public override string ToString()
        {
            return $"{Type} (# {objectID})";
        }

        private int collisionBoxSize;
        private int numberOfHits;
        private double originalAngle;
        private double xCoordinate;
        private double yCoordinate;
        private double theta;
        private double height;
        private double velocity;
        private BitmapImage _image;
        private double cX;
        private double cY;

        public int CollisionBoxSize
        {
            get { return collisionBoxSize; }
            set { SetField(ref collisionBoxSize, value); }
        }

        public int NumberOfHits
        {
            get { return numberOfHits; }
            set { SetField(ref numberOfHits, value); }
        }

        public double OriginalAngle
        {
            get { return originalAngle; }
            set { SetField(ref originalAngle, value); }
        }

        public double CX
        {
            get { return cX; }
            set { SetField(ref cX, value); }
        }

        public double CY
        {
            get { return cY; }
            set { SetField(ref cY, value); }
        }

        public BitmapImage _Image
        {
            get { return _image; }
            set { SetField(ref _image, value); }
        }

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
