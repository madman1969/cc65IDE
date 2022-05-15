using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using cc65Wrapper;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.Win32;

namespace Cc65Wpf
{
	// TODO: Add mechanism to add source/header files
	// TODO: Add CC65 Settings dialog
	// TODO: Add project settings dialog
	// TODO: Add WinVICE settings dialog ?

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		#region Field and properties

		private Cc65Project? project;
		private Cc65Emulators emulators;
		private string currentFileName = string.Empty;		
		private string projectFile = string.Empty;
		FoldingManager foldingManager;
		object foldingStrategy;

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
			outputTextBox.Text = string.Empty;
		}

		#endregion

		#region Folding

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
                PromptForModifiedFile();

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

		private void projectTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			TreeViewItem selected = e.NewValue as TreeViewItem;
			var tag = selected.Tag as string;

			// Is selected node header or source file ? ...
			if (tag != string.Empty)
			{
				// Yep, so retrieve the file path and clear the editor ...
				var currentFile = tag;
				textEditor.Clear();

				// Read the file and populate the editor ...
				var text = File.ReadAllText(currentFile);
				textEditor.Text = text;

				// Update the currently selected file ...
				currentFileName = currentFile;
				textEditor.IsModified = false;
			}
		}

		private async void BuildButton_Click(object sender, RoutedEventArgs e)
		{
			if (textEditor.IsModified)
				PromptForModifiedFile();

			await BuildProject();
		}

		private async void ExecuteButton_Click(object sender, RoutedEventArgs e)
		{
			if (textEditor.IsModified)
				PromptForModifiedFile();

			await ExecuteProject();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Saves the current source file
		/// </summary>
		private void SaveFile()
		{
			if (string.IsNullOrEmpty(currentFileName))
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

		/// <summary>
		/// Open a source file
		/// </summary>
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

		/// <summary>
		/// Closes the current project
		/// </summary>
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

		/// <summary>
		/// Populate the tree with the project files
		/// </summary>
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

		/// <summary>
		/// Builds the project.
		/// </summary>
		private async Task<bool> BuildProject()
		{
			var builtOK = false;

			outputTextBox.AppendText($"Building {project.InputFiles.Count} files for project [{project.ProjectName}] targeting [{project.TargetPlatform}]...\r\n");

			// Compile the project ...
			var result = await Cc65Build.Compile(project);

			if (result.ExitCode != 0)
			{
				var errorList = Cc65Build.ErrorsAsList(result);

				outputTextBox.AppendText($"Build failed, found {errorList.Count} errors:\r\n");

				foreach (var error in errorList)
				{
					outputTextBox.AppendText($"{error}\r\n");
				}
			}
			else
			{
				builtOK = true;
				outputTextBox.AppendText("Build successful\r\n");
			}

			outputTextBox.ScrollToEnd();

			return builtOK;
		}

		/// <summary>
		/// Launches the project in WinVICE.
		/// </summary>
		private async Task ExecuteProject()
		{
			var builtOK = await BuildProject();

			if (builtOK)
			{
				outputTextBox.AppendText($"Launching {project.ProjectName} in emulator ...\r\n");

				var result = await Cc65Emulators.LaunchEmulator(project, emulators);
			}
		}

		/// <summary>
		/// Prompt user to save modified file
		/// </summary>
		private void PromptForModifiedFile()
		{
			var result = MessageBox.Show("Do you want to save the changes ?", $"{currentFileName} - File Modified !", MessageBoxButton.YesNo);

			switch (result)
			{
				case MessageBoxResult.Yes:
					SaveFile();
					break;

				case MessageBoxResult.No:
					break;
			}
		}


        #endregion        
    }	
}
