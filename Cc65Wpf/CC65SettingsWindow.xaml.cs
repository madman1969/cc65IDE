using cc65Wrapper;
using System.Windows;

namespace Cc65Wpf
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class CC65SettingsWindow : Window
    {
        private bool SettingsChanged = false;
        private Cc65CompilerConfiguration configuration = new Cc65CompilerConfiguration();

        public CC65SettingsWindow()
        {
            InitializeComponent();

            CC65PropertyGrid.SelectedObject = configuration;            
        }

        private void CC65PropertyGrid_PropertyValueChanged(object sender, Xceed.Wpf.Toolkit.PropertyGrid.PropertyValueChangedEventArgs e)
        {
            SettingsChanged = true;
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            if (SettingsChanged)
            {
                var result = MessageBox.Show("Do you wish to save the changes ?", "CC65 Settings changed !", MessageBoxButton.YesNo, MessageBoxImage.Question);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        configuration.SaveConfiguration();
                        break;

                    case MessageBoxResult.No:
                        break;
                }
            }
        }
    }
}
