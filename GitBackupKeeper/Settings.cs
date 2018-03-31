using GitBackupKeeper.Helper;
using System;
using System.Xml.Serialization;

namespace GitBackupKeeper
{
    [XmlRoot("Settings")]
    public class Settings : BaseModel
    {
        private String _serverUrl;
        public String serverUrl
        {
            get { return _serverUrl; }
            set { _serverUrl = value; OnPropertyChanged("serverUrl"); }
        }
        private String _destinationPath;
        public String destinationPath
        {
            get { return _destinationPath; }
            set { _destinationPath = value; OnPropertyChanged("destinationPath"); }
        }
        private String _username;
        public String username
        {
            get { return _username; }
            set { _username = value; OnPropertyChanged("username"); }
        }

        private String _password;
        public String password
        {
            get { return _password; }
            set { _password = value; OnPropertyChanged("password"); }
        }

        [XmlIgnore]
        public RelayCommand saveSettings { get; set; }
        [XmlIgnore]
        public RelayCommand cancelSettings { get; set; }

        public Settings()
        {
            this.serverUrl = "";
            this.username = "Jack";
            this.password = "123";
            this.destinationPath = System.IO.Directory.GetCurrentDirectory() + "\\repos";
            this.saveSettings = new RelayCommand(doSaveSettings);
            this.cancelSettings = new RelayCommand(doCancelSettings);
        }

        private void doSaveSettings()
        {
        }

        private void doCancelSettings()
        {
        }
    }
}
