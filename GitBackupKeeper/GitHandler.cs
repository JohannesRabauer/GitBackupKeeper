using LibGit2Sharp;
using System;

namespace GitBackupKeeper
{
    class GitHandler
    {
        private GitRepository _repo;

        public GitHandler(GitRepository repo)
        {
            this._repo = repo;
        }
        public void clone()
        {
            this._repo.taskDescription = "Cloning repository...";
            this._repo.isIndetermerminate = true;
            var co = new CloneOptions();
            co.CredentialsProvider = myCredentialsProvider;
            co.OnCheckoutProgress = checkoutHandler;
            co.RecurseSubmodules = true;
            Repository.Clone(this._repo.url, this._repo.getLocalPath(), co);
        }

        public void reset()
        {
            this._repo.isIndetermerminate = true;
            this._repo.taskDescription = "Resetting local repository...";
            using (Repository gitRepo = new Repository(this._repo.getLocalPath()))
            {
                Commit currentCommit = gitRepo.Head.Tip;
                gitRepo.Reset(ResetMode.Hard, currentCommit);
            }
        }

        public void pull()
        {
            this._repo.isIndetermerminate = true;
            this._repo.taskDescription = "Pulling changes...";
            PullOptions options = new PullOptions();
            options.FetchOptions = new FetchOptions();
            options.FetchOptions.CredentialsProvider = myCredentialsProvider;
            options.FetchOptions.TagFetchMode = TagFetchMode.All;
            using (Repository gitRepo = new Repository(this._repo.getLocalPath()))
            {
                Commands.Pull(gitRepo, new Signature("test", "test", new DateTimeOffset()), options);
            }
        }

        private Credentials myCredentialsProvider(string url, string username, SupportedCredentialTypes types)
        {
            return new UsernamePasswordCredentials { Username = this._repo.settings.username, Password = this._repo.settings.password };
        }
        private void checkoutHandler(String path, int completedSteps, int totalSteps)
        {
            this._repo.isIndetermerminate = false;
            this._repo.progress = 100.0 / totalSteps * completedSteps;
            this._repo.taskDescription = "Cloning repository (" + completedSteps + "/" + totalSteps + ")...";
        }
    }
}
