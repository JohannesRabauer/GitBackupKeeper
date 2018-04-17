using System;
using System.IO;
using System.Net;
using WebDav;

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
      WebDavClient webDavClient = createClient();
      if (this._repo.settings.zipping)
      {
        this._repo.taskDescription = "Deleting old zip file...";
        WebDavResponse response = webDavClient.Delete(Path.Combine(this._repo.settings.webDavBasePath, Path.GetFileName(this._repo.getLocalZipPath()))).Result;
        using (var fileStream = File.OpenRead(this._repo.getLocalZipPath()))
        {
          this._repo.taskDescription = "Uploading zip file...";
          response = webDavClient.PutFile(Path.Combine(this._repo.settings.webDavBasePath ,Path.GetFileName(this._repo.getLocalZipPath())), fileStream).Result;
        };
      }
    }

    private WebDavClient createClient()
    {
      WebDavClientParams parameter = new WebDavClientParams();
      parameter.BaseAddress = new Uri(this._repo.settings.webDavServerUrl);
      parameter.Credentials = new NetworkCredential(this._repo.settings.webDavUsername, this._repo.settings.webDavPassword);
      return new WebDavClient(parameter);
    }
  }
}
