using GitBackupKeeper.Helper;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GitBackupKeeper
{
    public class MainContext : BaseModel, IDisposable
    {
        private Settings _settings;

        public Settings settings
        {
            get { return _settings; }
            set
            {
                _settings = value;
                base.OnPropertyChanged("settings");
                _settings.init(this);
                if(this._respositories != null)
                {
                    foreach (GitRepository repo in this._respositories)
                    {
                        repo.init(this, this._settings);
                    }
                }
            }
        }

        private ObservableCollection<GitRepository> _respositories;

        public ObservableCollection<GitRepository> repositories
        {
            get { return _respositories; }
            set { _respositories = value; OnPropertyChanged("repositories"); }
        }

        public RelayCommand downloadAllRepositories { get; set; }
        public RelayCommand showSettings { get; set; }
        public RelayCommand addRepository { get; set; }

        public MainContext()
        {
            try
            {
                this.settings = deserializeFromString<Settings>(Properties.Settings.Default.SavedSettings);
            }
            catch (Exception e)
            {
                this.settings = new Settings();
            }
            try
            {
                this.repositories = deserializeFromString<ObservableCollection<GitRepository>>(Properties.Settings.Default.SavedRepositories);
            }
            catch (Exception)
            {
                this.repositories = new ObservableCollection<GitRepository>();
            }
            foreach (GitRepository repository in this.repositories)
            {
                repository.init(this, this._settings);
            }
            this.showSettings = new RelayCommand(doShowSettings);
            this.downloadAllRepositories = new RelayCommand(doDownloadAllRepositories);
            this.addRepository = new RelayCommand(doAddRepository);
        }

        private void doShowSettings()
        {
            Settings tempSettings = deserializeFromString<Settings>(serializeObject(this._settings));
            tempSettings.init(this);
            new WindowService().showWindow("Settings",tempSettings);
        }

        private void doDownloadAllRepositories()
        {
            Parallel.ForEach<GitRepository>(this._respositories, repo => repo.doDownload());
        }

        private void doAddRepository()
        {
            GitRepository newRepository = new GitRepository();
            newRepository.init(this, this._settings);
            this.repositories.Add(newRepository);
        }

        public void deleteRepository(GitRepository repositoryToDelete)
        {
            this.repositories.Remove(repositoryToDelete);
        }

        private string serializeObject<T>(T toSerialize)
        {
            using (StringWriter textWriter = new StringWriter())
            {
                new XmlSerializer(toSerialize.GetType()).Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }

        private T deserializeFromString<T>(string objectData)
        {
            using (TextReader reader = new StringReader(objectData))
                return (T)new XmlSerializer(typeof(T)).Deserialize(reader);
        }

        public void Dispose()
        {
            Properties.Settings.Default.SavedSettings = serializeObject<Settings>(this._settings);
            Properties.Settings.Default.SavedRepositories = serializeObject<ObservableCollection<GitRepository>>(this.repositories);
            Properties.Settings.Default.Save();
        }
    }
}
