﻿using cc65Wrapper;
using System.Windows;

namespace Cc65Wpf
{
    /// <summary>
    /// Interaction logic for ProjectSettings.xaml
    /// </summary>
    public partial class ProjectSettings : Window
    {
        Cc65Project project;

        public Cc65Project Project { get => project; set => project = value; }
        public string ProjectName { get => project.ProjectName; set => project.ProjectName = value; }
        public string WorkingDirectory { get => project.WorkingDirectory; set => project.WorkingDirectory = value; }
        public string TargetPlatform { get => project.TargetPlatform; set => project.TargetPlatform = value; }
        public string OutputFile { get => project.OutputFile; set => project.OutputFile = value; }
        public string FullOutputFilePath { get => project.FullOutputFilePath; }
        public bool OptimiseCode { get => project.OptimiseCode; set => project.OptimiseCode = value; }
        public int Version { get => project.Version; set => project.Version = value; }

        public ProjectSettings()
        {
            InitializeComponent();
            this.DataContext = this;
        }        

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}