using GitBackupKeeper.Handler;
using GitBackupKeeper.Helper;
using System;
using System.Linq;
using System.Windows;
using System.Xml.Serialization;

namespace GitBackupKeeper
{
    public class GitRepository : BaseModel
    {
        private String _url;

        public String url
        {
            get { return _url; }
            set { _url = value; OnPropertyChanged("url"); }
        }

        private Boolean _isBusy;
        [XmlIgnore]
        public Boolean isBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; OnPropertyChanged("isBusy"); }
        }
        private Boolean _isIndetermerminate;
        [XmlIgnore]
        public Boolean isIndetermerminate
        {
            get { return _isIndetermerminate; }
            set { _isIndetermerminate = value; OnPropertyChanged("isIndetermerminate"); }
        }
        
        private Double _progress;
        [XmlIgnore]
        public Double progress
        {
            get { return _progress; }
            set { _progress = value; OnPropertyChanged("progress"); }
        }


        private String _taskDescription;
        [XmlIgnore]
        public String taskDescription
        {
            get { return _taskDescription; }
            set { _taskDescription = value; OnPropertyChanged("taskDescription"); }
        }


        [XmlIgnore]
        public RelayCommand download { get; set; }
        [XmlIgnore]
        public RelayCommand delete { get; set; }
        private MainContext _context;
        private Settings _settings;
        public Settings settings
        {
            get { return this._settings; }
        }
        private GitHandler _gitHandler;
        private ZipHandler _zipHandler;
        private WebDavHandler _webDavHandler;

        public GitRepository() : this("")
        {
            this.download = new RelayCommand(doDownload);
            this.delete = new RelayCommand(doDelete);
        }

        public GitRepository(String url)
        {
            this.url = url;
            this.isBusy = false;
            this.taskDescription = "";
            this.progress = 0;
            this.isIndetermerminate = false;
            this._gitHandler = new GitHandler(this);
            this._zipHandler = new ZipHandler(this);
            this._webDavHandler = new WebDavHandler(this);
        }

        public void init(MainContext context, Settings settings)
        {
            this._context = context;
            this._settings = settings;
        }

        public void doDownload()
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                this.isBusy = true;
                this.progress = 0;
                if (!this._zipHandler.checkAndExecuteUnzipping()) return;
                if (System.IO.Directory.Exists(getLocalPath()))
                {
                    this._gitHandler.reset();
                    this._gitHandler.pull();
                }
                else
                {
                    this._gitHandler.clone();
                }
                if (!this._zipHandler.checkAndExecuteZipping()) return;
                this._webDavHandler.updloadIfChecked();
                this.taskDescription = "Done";
                this.isBusy = false;
                this.isIndetermerminate = false;
            });
        }

        public String getLocalPath()
        {
            String localPath = System.IO.Path.Combine(this._settings.destinationPath, this._url.Split('/').Last().Split('.').First());
            return localPath;
        }

                public String getLocalZipPath()
        {
            return getLocalPath() + ".zip";
        }

        private void doDelete()
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
                this._context.deleteRepository(this);
        }
    }
}
