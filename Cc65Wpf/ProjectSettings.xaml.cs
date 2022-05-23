using cc65Wrapper;
using cc65Wrapper.Enumerations;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace Cc65Wpf
{
    /// <summary>
    /// Interaction logic for ProjectSettings.xaml
    /// </summary>
    public partial class ProjectSettings : Window, INotifyPropertyChanged
    {
        Cc65Project project;

        public Cc65Project Project { get => project; set => project = value; }
        public string ProjectName { get => project.ProjectName; set => project.ProjectName = value; }
        public string WorkingDirectory { get => project.WorkingDirectory; set => project.WorkingDirectory = value; }
        public string TargetPlatform { get => project.TargetPlatform; set => project.TargetPlatform = value; }
        public string OutputFile { get => project.OutputFile; set => project.OutputFile = value; }
        public string FullOutputFilePath { get => Path.Combine(WorkingDirectory, OutputFile); set => FullOutputFilePath = value; }
        public bool OptimiseCode { get => project.OptimiseCode; set => project.OptimiseCode = value; }
        public int Version { get => project.Version; set => project.Version = value; }

        public bool CanSave { get => !string.IsNullOrEmpty(ProjectName) && !string.IsNullOrEmpty(WorkingDirectory) && !string.IsNullOrEmpty(OutputFile); }

        int targetType;
        public int TargetType { get => (int)Enum.Parse(typeof(CC65ProjectTypes), TargetPlatform); set => targetType = value; }

        #region Events

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region Class Constructor 

        public ProjectSettings()
        {
            InitializeComponent();
            this.DataContext = this;

            // Make sure window is modal ...
            this.Owner = App.Current.MainWindow;
        }

        #endregion 

        private void Window_Activated(object sender, EventArgs e)
        {
            // Handle new projects ...
            if (project == null || string.IsNullOrEmpty(ProjectName))
                this.Title = "New Project Settings";
            else
                this.Title = $"{ProjectName} Project Settings";
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Messy conversion of selected target back to string ...
            project.TargetPlatform = ((CC65ProjectTypes)targetType).ToString();

            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void WorkingDirectoryTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            project.WorkingDirectory = WorkingDirectoryTextBox.Text;
            OnPropertyRaised("WorkingDirectory");
            OnPropertyRaised("FullOutputFilePath");
            OnPropertyRaised("CanSave");
        }

        private void OutputFileTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            project.OutputFile = OutputFileTextBox.Text;
            OnPropertyRaised("OutputFile");
            OnPropertyRaised("FullOutputFilePath");
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
    }
}
