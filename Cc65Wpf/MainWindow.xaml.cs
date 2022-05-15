using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using cc65Wrapper;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Search;
using Microsoft.Win32;

namespace Cc65Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		#region Field and properties

		private Cc65Project? project;
		private Cc65Emulators emulators;
		private string currentFileName = string.Empty;
		private string currentFile = string.Empty;
		private string projectFile = string.Empty;

        #endregion

        #region Class Constructor 

        public MainWindow()
        {
            InitializeComponent();

			// Load emulator settings ...
			var filepath = Path.Combine(Directory.GetCurrentDirectory(), @"Test Files");
			filepath = Path.Combine(filepath, "emulators.json");
			var json = File.ReadAllText(filepath);
			emulators = Cc65Emulators.FromJson(json);
		}

		#endregion

		#region Folding
		FoldingManager foldingManager;
		object foldingStrategy;

		void HighlightingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (textEditor.SyntaxHighlighting == null)
			{
				foldingStrategy = null;
			}
			else
			{
				switch (textEditor.SyntaxHighlighting.Name)
				{
					case "XML":
						foldingStrategy = new XmlFoldingStrategy();
						textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
						break;
					case "C#":
					case "C++":
					case "PHP":
					case "Java":
						textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(textEditor.Options);
						foldingStrategy = new BraceFoldingStrategy();
						break;
					default:
						textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
						foldingStrategy = null;
						break;
				}
			}
			if (foldingStrategy != null)
			{
				if (foldingManager == null)
					foldingManager = FoldingManager.Install(textEditor.TextArea);
				UpdateFoldings();
			}
			else
			{
				if (foldingManager != null)
				{
					FoldingManager.Uninstall(foldingManager);
					foldingManager = null;
				}
			}
		}

		void UpdateFoldings()
		{
			if (foldingStrategy is BraceFoldingStrategy)
			{
				((BraceFoldingStrategy)foldingStrategy).UpdateFoldings(foldingManager, textEditor.Document);
			}
			if (foldingStrategy is XmlFoldingStrategy)
			{
				((XmlFoldingStrategy)foldingStrategy).UpdateFoldings(foldingManager, textEditor.Document);
			}
		}
        #endregion

        #region Menu Handlers

        private void openFileClick(object sender, RoutedEventArgs e)
        {
            OpenFile();
        }

        private void saveFileClick(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
			textEditor.Clear();
			Application.Current.Shutdown();			
		}

        private void OpenFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
			OpenFile();
		}

        private void CloseFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
			// File unmodified, so just clear the edit control ...
			if (!textEditor.IsModified)
            {
				textEditor.Clear();
            }
			else
            {
				var result = MessageBox.Show("Do you want to save the changes ?", "File Modified !", MessageBoxButton.YesNo);

				switch (result)
                {
					case MessageBoxResult.Yes:
						SaveFile();
						break;

					case MessageBoxResult.No:
						break;
                }

				textEditor.Clear();
            }

        }

        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
			SaveFile();
        }

        private void OpenProjectMenuItem_Click(object sender, RoutedEventArgs e)
        {
			OpenProject();
        }

		private void CloseProjectMenuItem_Click(object sender, RoutedEventArgs e)
		{
			CloseProject();
		}

		#endregion

		#region Private Methods

		private void SaveFile()
		{
			if (currentFileName == null)
			{
				SaveFileDialog dlg = new SaveFileDialog();
				dlg.DefaultExt = ".txt";

				if (dlg.ShowDialog() ?? false)
				{
					currentFileName = dlg.FileName;
				}
				else
				{
					return;
				}
			}

			textEditor.Save(currentFileName);
		}

		private void OpenFile()
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "Source Files|*.c|Header Files|*.h";
			dlg.CheckFileExists = true;

			if (dlg.ShowDialog() ?? false)
			{
				currentFileName = dlg.FileName;
				textEditor.Load(currentFileName);
				textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(currentFileName));
			}
		}

		/// <summary>
		/// Opens the project.
		/// </summary>
		private void OpenProject()
		{
			var dlg = new OpenFileDialog();
			dlg.Filter = "Project Files|*.json";
			dlg.CheckFileExists = true;

			if (dlg.ShowDialog() ?? false)
			{
				// Load the project JSON ...
				projectFile = dlg.FileNames[0];
				var json = File.ReadAllText(projectFile);
				project = Cc65Project.FromJson(json);

				projectInfo.Text = $"Project {project.ProjectName} loaded";

			}

			// Populate the tree view
			PopulateTreeView();
		}

		private void CloseProject()
        {
			project = null;
			projectInfo.Text = "No project loaded";
			ClearTreeView();
			textEditor.Clear();			
		}

		/// <summary>
		/// Clears the TreeView.
		/// </summary>
		private void ClearTreeView()
		{
			projectTreeView.Items.Clear();
		}

		private void PopulateTreeView()
        {
			ClearTreeView();

            // Add root node ...
            var rootNode = new TreeViewItem
            {
                // Name = $"{project.ProjectName}",
                Header = $"{project.ProjectName}",
                Tag = string.Empty
            };
            projectTreeView.Items.Add(rootNode);

            // Add 'Header Files' node ...
            var hdrFiles = new TreeViewItem
            {
                Header = "Header Files",
                Tag = string.Empty,
				IsExpanded = true
			};
            rootNode.Items.Add(hdrFiles);

            // Add 'Source Files' node ...
            var srcFiles = new TreeViewItem
            {
                Header = "Source Files",
                Tag = string.Empty,
				IsExpanded = true
            };
            rootNode.Items.Add(srcFiles);

			// Add the header files ...
			foreach (var hdrfile in project.HeaderFiles)
			{
				var node = new TreeViewItem
				{
					Header = hdrfile,
					Tag = Path.Combine(project.WorkingDirectory, hdrfile)
				};

				hdrFiles.Items.Add(node);
			}

			// Add the source files ...
			foreach (var srcfile in project.InputFiles)
			{
				var node = new TreeViewItem
				{
					Header = srcfile,
					Tag = Path.Combine(project.WorkingDirectory, srcfile)
				};

				srcFiles.Items.Add(node);
			}
		}

        #endregion
    }	
}
