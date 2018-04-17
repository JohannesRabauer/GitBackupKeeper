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

        private bool _useWebDav;
        public bool useWebDav
        {
            get { return _useWebDav; }
            set { _useWebDav = value; OnPropertyChanged("useWebDav"); }
        }

        private String _webDavServerUrl;
        public String webDavServerUrl
        {
            get { return _webDavServerUrl; }
            set { _webDavServerUrl = value; OnPropertyChanged("webDavServerUrl"); }
        }
        private String _webDavBasePath;
        public String webDavBasePath
        {
            get { return _webDavBasePath; }
            set { _webDavBasePath = value; OnPropertyChanged("webDavBasePath"); }
        }
        private String _webDavUsername;
        public String webDavUsername
        {
            get { return _webDavUsername; }
            set { _webDavUsername = value; OnPropertyChanged("webDavUsername"); }
        }

        [XmlIgnore]
        public String webDavPassword { get; set; }
        public String webDavEncryptedPassword
        {
            get { return encrypt(this.webDavPassword); }
            set { this.webDavPassword = decrypt(value); }
        }

        [XmlIgnore]
        public String webDavCompleteServerPath
        {
            get { return webDavServerUrl + webDavBasePath; }
        }

        private bool _encryptZipFile;
        public bool encryptZipFile
        {
            get { return _encryptZipFile; }
            set { _encryptZipFile = value; OnPropertyChanged("encryptZipFile"); }
        }
        private String _encryptionPassword;
        public String encryptionPassword
        {
            get { return _encryptionPassword; }
            set { _encryptionPassword = value; OnPropertyChanged("encryptionPassword"); }
        }

        [XmlIgnore]
        public RelayCommand saveSettings { get; set; }
        [XmlIgnore]
        public RelayCommand cancelSettings { get; set; }
        [XmlIgnore]
        public RelayCommand generateZipPassword { get; set; }
        private MainContext _context;

        public Settings()
        {
            this.serverUrl = "https://try.gogs.io/";
            this.destinationPath = System.IO.Directory.GetCurrentDirectory() + "\\repos";
            this.saveSettings = new RelayCommand(doSaveSettings);
            this.cancelSettings = new RelayCommand(doCancelSettings);
            this.generateZipPassword = new RelayCommand(doGenerateZipPassword);
            this.zipping = false;
            this.useWebDav = false;
            this.webDavServerUrl = "https://myowncloud.com/";
            this._serverUrl = "";
            this._username = "";
            this._webDavBasePath = "";
            this._webDavServerUrl = "";
            this._webDavUsername = "";
            this.password = "";
            this.webDavPassword = "";
            this._encryptZipFile = false;
            doGenerateZipPassword();
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

        private void doGenerateZipPassword()
        {
            this.encryptionPassword = System.Web.Security.Membership.GeneratePassword(40, 20);
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
