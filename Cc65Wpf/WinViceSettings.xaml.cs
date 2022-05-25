using cc65Wrapper;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Cc65Wpf
{
    /// <summary>
    /// Interaction logic for WinViceSettings.xaml
    /// </summary>
    public partial class WinViceSettings : Window, INotifyPropertyChanged
    {
        private Cc65Emulators emulators;
        private bool canSave;
        public Cc65Emulators Emulators { get => emulators; set => emulators = value; }
        public bool CanSave { get => canSave; set => canSave = value; }

        #region Events

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        public WinViceSettings()
        {
            InitializeComponent();

            this.DataContext = this;

            // Make sure window is modal ...
            this.Owner = App.Current.MainWindow;

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CanSave = true;
            OnPropertyRaised("CanSave");
        }

        #region Private Methods

        /// <summary>
        /// Raise events when properties changed
        /// </summary>
        /// <param name="propertyname"></param>
        private void OnPropertyRaised(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        #endregion

        private void Window_Activated(object sender, System.EventArgs e)
        {
            CanSave = false;
            OnPropertyRaised("CanSave");
        }
    }
}
