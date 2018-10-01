using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroid
{
    public class Pictures : INotifyPropertyChanged
    {
        private int id;
        private string type;
        private ObservableCollection<Details> listOfPictures = new ObservableCollection<Details>();

        public Pictures(int id, string type)
        {
            Id = id;
            Type = type;
        }

        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");

            }
        }
        public String Type
        {
            get { return type; }
            set
            {
                type = value;
                OnPropertyChanged("Type");

            }
        }

        public ObservableCollection<Details> ListOfPictures
        {
            get { return listOfPictures; }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

    }







    public class Details : INotifyPropertyChanged
    {
        private int id;
        private string type;
        private string pictureName;

        public Details(int id, String type, String pictureName)
        {
            Id = id;
            Type = type;
            PictureName = pictureName;
        }

        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");

            }
        }

        public string Type
        {
            get { return type; }
            set
            {
                type = value;
                OnPropertyChanged("Type");
            }
        }

        public string PictureName
        {
            get { return pictureName; }
            set
            {
                pictureName = value;
                OnPropertyChanged("PictureName");
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
