using CleanTable.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanTable.Model
{
    public class SnapShot : INotifyPropertyChanged
    {
        private string id;
        public string Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
                NotifyPropertyChanged(nameof(Id));
            }
        }

        private string path;
        public string Path
        {
            get
            {
                return path;
            }

            set
            {
                if(path != value)
                {
                    path = value;
                    NotifyPropertyChanged(nameof(Path));
                }
            }
        }

        private string captureDateTime;
        public string CaptureDateTime
        {
            get
            {
                return captureDateTime;
            }

            set
            {
                if (captureDateTime != value)
                {
                    captureDateTime = value;
                    NotifyPropertyChanged(nameof(CaptureDateTime));
                }
            }
        }

        private bool isEmpty;
        public bool IsEmpty
        {
            get
            {
                return isEmpty;
            }

            set
            {
                isEmpty = value;
                NotifyPropertyChanged(nameof(IsEmpty));
            }
        }

        private string category;
        public string Category
        {
            get
            {
                return category;
            }

            set
            {
                category = value;
                NotifyPropertyChanged(nameof(Category));
            }
        }

        private int accuracy;
        public int Accuracy
        {
            get
            {
                return accuracy;
            }

            set
            {
                accuracy = value;
                NotifyPropertyChanged(nameof(Accuracy));
            }
        }

        private float loadingTime;
        public float LoadingTime
        {
            get
            {
                return loadingTime;
            }

            set
            {
                loadingTime = value;
                NotifyPropertyChanged(nameof(LoadingTime));
            }
        }

        private string message;
        public string Message
        {
            get
            {
                return message;
            }

            set
            {
                message = value;
                NotifyPropertyChanged(nameof(Message));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
