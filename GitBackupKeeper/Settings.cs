using GitBackupKeeper.Helper;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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

        [XmlIgnore]
        public String password { get; set; }
        public String encryptedPassword
        {
            get { return encrypt(this.password); }
            set { this.password = decrypt(value); }
        }

        private bool _zipping;
        public bool zipping
        {
            get { return _zipping; }
            set { _zipping = value; OnPropertyChanged("zipping"); }
        }

        [XmlIgnore]
        public RelayCommand saveSettings { get; set; }
        [XmlIgnore]
        public RelayCommand cancelSettings { get; set; }
        private MainContext _context;

        public Settings()
        {
            this.serverUrl = "";
            this.username = "Jack";
            this.password = "123";
            this.destinationPath = System.IO.Directory.GetCurrentDirectory() + "\\repos";
            this.saveSettings = new RelayCommand(doSaveSettings);
            this.cancelSettings = new RelayCommand(doCancelSettings);
            this.zipping = false;
        }

        public void init(MainContext context)
        {
            this._context = context;
        }

        private void doSaveSettings(Object passwordBox)
        {
            this.password = ((PasswordBox)passwordBox).Password;
            this._context.settings = this;
        }

        private void doCancelSettings(Object windowToClose)
        {
            ((Window)windowToClose).Close();
        }

        public string encrypt(string text)
        {
            return Convert.ToBase64String(ProtectedData.Protect(Encoding.Unicode.GetBytes(text), null, DataProtectionScope.CurrentUser));
        }

        public string decrypt(string text)
        {
            return Encoding.Unicode.GetString(ProtectedData.Unprotect(Convert.FromBase64String(text),null,DataProtectionScope.CurrentUser));
        }
    }
}
