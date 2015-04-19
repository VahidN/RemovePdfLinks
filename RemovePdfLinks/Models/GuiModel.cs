using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace RemovePdfLinks.Models
{
    public class GuiModel : INotifyPropertyChanged
    {
        private string _inputFolderPath;
        private string _newUrl = "http://www.site.com";
        private string _outputFolderPath = Path.Combine(Application.StartupPath, "Mod");
        private bool _removeAllLinks = true;

        public event PropertyChangedEventHandler PropertyChanged;

        public string InputFolderPath
        {
            get { return _inputFolderPath; }
            set
            {
                _inputFolderPath = value;
                notifyPropertyChanged("InputFolderPath");
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

        public string OutputFolderPath
        {
            get { return _outputFolderPath; }
            set
            {
                _outputFolderPath = value;
                notifyPropertyChanged("OutputFolderPath");
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