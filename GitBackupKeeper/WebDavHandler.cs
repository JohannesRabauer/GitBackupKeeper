using LibGit2Sharp;
using System;
using System.IO;
using System.Net;
using WebDAVClient;
using WebDAVClient.Model;

namespace GitBackupKeeper
{
    class WebDavHandler
    {
        private GitRepository _repo;

        public WebDavHandler(GitRepository repo)
        {
            this._repo = repo;
        }

        public void updloadIfChecked()
        {
            if (!this._repo.settings.useWebDav) return;
            this._repo.isIndetermerminate = true;
            IClient webDavClient = createClient();
            if (!this._repo.settings.zipping)
            {
                this._repo.taskDescription = "Deleting old zip file...";
                webDavClient.DeleteFile(this._repo.getLocalZipPath());
                using (var fileStream = File.OpenRead(this._repo.getLocalZipPath()))
                {
                    this._repo.taskDescription = "Uploading zip file...";
                    webDavClient.Upload(this._repo.settings.webDavCompleteServerPath, fileStream, Path.GetFileName(this._repo.getLocalZipPath()));
                };
            }
        }

        private IClient createClient()
        {
            IClient client = new Client(new NetworkCredential { UserName = this._repo.settings.webDavUsername, Password = this._repo.settings.webDavPassword });
            client.Server = this._repo.settings.webDavServerUrl;
            client.BasePath = this._repo.settings.webDavBasePath;
            return client;
        }
    }
}
