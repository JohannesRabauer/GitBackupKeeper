using System.ComponentModel;
using System.Windows;

namespace GitBackupKeeper
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private MainContext _context ;
        public MainWindow()
        {
            InitializeComponent();
            this._context = new MainContext();
            this.DataContext = this._context;
            this.Closing += closing;
        }

        private void closing(object o , CancelEventArgs args)
        {
            if(this._context != null)
            {
                this._context.Dispose();
                this._context = null;
            }
        }
    }
}
