using GitBackupKeeper.Helper;
using LibGit2Sharp;
using System;
using System.Diagnostics;
using System.Linq;
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
                if (System.IO.Directory.Exists(getLocalPath()))
                {
                    reset();
                    pull();
                }
                else
                {
                    clone();
                }
                this.taskDescription = "Done";
                this.isBusy = false;
                this.isIndetermerminate = false;
            });
        }

        private void clone()
        {
            this.taskDescription = "Cloning repository...";
            this.isIndetermerminate = true;
            var co = new CloneOptions();
            co.CredentialsProvider = myCredentialsProvider;
            co.OnCheckoutProgress = checkoutHandler;
            Repository.Clone(this._url, getLocalPath(), co);
        }

        private void reset()
        {
            this.isIndetermerminate = true;
            this.taskDescription = "Resetting local repository...";
            using (Repository repo = new Repository(getLocalPath()))
            {
                Commit currentCommit = repo.Head.Tip;
                repo.Reset(ResetMode.Hard, currentCommit);
            }
        }

        private void pull()
        {
            this.isIndetermerminate = true;
            this.taskDescription = "Pulling changes...";
            PullOptions options = new PullOptions();
            options.FetchOptions = new FetchOptions();
            options.FetchOptions.CredentialsProvider = myCredentialsProvider;
            options.FetchOptions.TagFetchMode = TagFetchMode.All;
            using (Repository repo = new Repository(getLocalPath()))
            {
                Commands.Pull(repo, new Signature("test", "test", new DateTimeOffset()), options);
            }
        }

        private Credentials myCredentialsProvider(string url, string username, SupportedCredentialTypes types)
        {
            return new UsernamePasswordCredentials { Username = this._settings.username, Password = this._settings.password };
        }

        private void checkoutHandler(String path, int completedSteps, int totalSteps)
        {
            this.isIndetermerminate = false;
            this.progress = 100.0 / totalSteps * completedSteps;
            this.taskDescription = "Cloning repository (" + completedSteps + "/" + totalSteps + ")...";
        }

        private String getLocalPath()
        {
            return System.IO.Path.Combine(this._settings.destinationPath, this._url.Split('/').Last().Split('.').First());
        }

        private void doDelete()
        {
            this._context.deleteRepository(this);
        }
    }
}
