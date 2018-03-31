using System.Windows;

namespace GitBackupKeeper.Helper
{
    class WindowService 
    {
        public void showWindow(object viewModel)
        {
            var win = new Window();
            win.Content = viewModel;
            win.Show();
        }
    }
}
