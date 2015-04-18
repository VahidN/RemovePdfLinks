using System.ComponentModel;

namespace RemovePdfLinks.Models
{
    public class GuiModel : INotifyPropertyChanged
    {
        private string _folderPath;
        private string _newUrl = "http://www.site.com";
        private bool _removeAllLinks = true;

        public event PropertyChangedEventHandler PropertyChanged;

        public string FolderPath
        {
            get { return _folderPath; }
            set
            {
                _folderPath = value;
                notifyPropertyChanged("FolderPath");
            }
        }

        public string NewUrl
        {
            get { return _newUrl; }
            set
            {
                _newUrl = value;
                notifyPropertyChanged("NewUrl");
            }
        }

        public bool RemoveAllLinks
        {
            get { return _removeAllLinks; }
            set
            {
                _removeAllLinks = value;
                notifyPropertyChanged("RemoveAllLinks");
            }
        }

        private void notifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}