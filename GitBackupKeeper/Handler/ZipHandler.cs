using System;
using System.IO;

namespace GitBackupKeeper.Handler
{
  class ZipHandler
  {

    private GitRepository _repo;

    public ZipHandler(GitRepository repo)
    {
      this._repo = repo;
    }

    public bool checkAndExecuteUnzipping()
    {
      if (!this._repo.settings.zipping) return true;
      if (System.IO.Directory.Exists(this._repo.getLocalPath()))
        deleteSurely(this._repo.getLocalPath());
      if (!System.IO.File.Exists(getLocalZipPath())) return true;
      unzip(getLocalZipPath(), this._repo.getLocalPath());
      System.IO.File.Delete(getLocalZipPath());
      return true;
    }

    public bool checkAndExecuteZipping()
    {
      if (!this._repo.settings.zipping) return true;
      if (System.IO.File.Exists(getLocalZipPath()))
        System.IO.File.Delete(getLocalZipPath());
      if (!System.IO.Directory.Exists(this._repo.getLocalPath())) return false;
      zip(this._repo.getLocalPath(), getLocalZipPath());
      deleteSurely(this._repo.getLocalPath());
      return true;
    }

    private void unzip(String sourceArchive, String destinationDirectory)
    {
      this._repo.taskDescription = "Unzipping directory...";
      this._repo.isIndetermerminate = true;
      using (Ionic.Zip.ZipFile zip = Ionic.Zip.ZipFile.Read(sourceArchive))
      {
        if (this._repo.settings.encryptZipFile)
        {
          zip.Password = this._repo.settings.encryptionPassword;
          zip.Encryption = Ionic.Zip.EncryptionAlgorithm.WinZipAes256;
        }
        foreach (Ionic.Zip.ZipEntry e in zip)
        {
          e.Extract(destinationDirectory);
        }
      }
    }

    private void zip(String sourceDirectory, String destinationZipFile)
    {
      this._repo.taskDescription = "Zipping directory...";
      this._repo.isIndetermerminate = true;
      using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile())
      {
        if (this._repo.settings.encryptZipFile)
        {
          zip.Password = this._repo.settings.encryptionPassword;
          zip.Encryption = Ionic.Zip.EncryptionAlgorithm.WinZipAes256;
        }
        zip.AddDirectory(sourceDirectory);
        zip.Save(destinationZipFile);
      }
    }

    private bool deleteSurely(String dirToDelete)
    {
      string[] files = Directory.GetFiles(dirToDelete);
      string[] dirs = Directory.GetDirectories(dirToDelete);
      foreach (string file in files)
      {
        File.SetAttributes(file, FileAttributes.Normal);
        File.Delete(file);
      }
      foreach (string dir in dirs)
      {
        deleteSurely(dir);
      }
      Directory.Delete(dirToDelete, false);
      return true;
    }
    private String getLocalZipPath()
    {
      return this._repo.getLocalZipPath();
    }
  }
}
