using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using cc65Wrapper;
using cc65Wrapper.Enumerations;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.Win32;

namespace Cc65Wpf
{
	// TODO: Add method to remove files from project
	// TODO: Add project settings dialog
	// TODO: Add WinVICE settings dialog ?

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged 
    {
		#region Private Field and properties

		private Cc65Project? project;
		private Cc65Emulators emulators;
		private string currentFileName = string.Empty;		
		private string projectFile = string.Empty;
		FoldingManager foldingManager;
		object foldingStrategy;
		private TreeViewItem selectedItem = null;

		#endregion

		#region Public Properties

		/// <summary>
		/// Specifies if project currently loaded
		/// </summary>
		public bool ProjectLoaded { get => Project != null; }

		/// <summary>
		/// Specifies if source file currently loaded
		/// </summary>
		public bool CurrentFileLoaded { get => CurrentFileName != String.Empty; }

		/// <summary>
		/// The current project instance
		/// </summary>
        public Cc65Project? Project 
		{ 
			get => project; 
			set 
			{ 
				project = value;
				OnPropertyRaised("Project");
				OnPropertyRaised("ProjectLoaded");
			} 
		}

		/// <summary>
		/// The path to the current source file
		/// </summary>
        public string CurrentFileName 
		{ 
			get => currentFileName;
			set 
			{ 
				currentFileName = value;
				OnPropertyRaised("CurrentFileName");
				OnPropertyRaised("CurrentFileLoaded");
			} 
		}

		/// <summary>
		/// The path to the current project file
		/// </summary>
        public string ProjectFile 
		{ 
			get => projectFile; 
			set
            {
				projectFile = value;
				OnPropertyRaised("ProjectFile");
			} 
		}

        #endregion

        #region Events

        public event PropertyChangedEventHandler? PropertyChanged;

		#endregion

		#region Class Constructor 

		public MainWindow()
        {
            InitializeComponent();
			this.DataContext = this;

			// Load emulator settings ...
			var filepath = Path.Combine(Directory.GetCurrentDirectory(), @"Test Files");
			filepath = Path.Combine(filepath, "emulators.json");
			var json = File.ReadAllText(filepath);
			emulators = Cc65Emulators.FromJson(json);
			outputTextBox.Text = string.Empty;

			// Register handler for text editor caret position changes ...
			textEditor.TextArea.Caret.PositionChanged += TextEditorCaret_PositionChanged;
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

		#region Event Handlers
	
		/// <summary>
		/// Handle changes to caret position in the editor
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TextEditorCaret_PositionChanged(object sender, EventArgs e)
		{
			ICSharpCode.AvalonEdit.Editing.Caret caret = sender as ICSharpCode.AvalonEdit.Editing.Caret;

			// do some stuff
			caretInfo.Text = $"Line {caret.Location.Line}, Column {caret.Location.Column}";
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

		private void saveProjectClick(object sender, RoutedEventArgs e)
		{
			SaveProject();
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
            CloseFile();
        }

        private void SaveFileMenuItem_Click(object sender, RoutedEventArgs e)
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
			// Bail if dodgy item selected ...
			if (e.NewValue == null || projectTreeView.SelectedItem == null)
				return;

			//// Retrieve the tag from the selected tree node ...
			selectedItem = e.NewValue as TreeViewItem;
			// TreeViewItem selected = projectTreeView.SelectedItem as TreeViewItem;
			var tag = selectedItem.Tag as string;

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
                CurrentFileName = currentFile;
                textEditor.IsModified = false;

				// Setup the file context menu
				projectTreeView.ContextMenu = projectTreeView.Resources["FileContext"] as ContextMenu;
			}
			else
            {
				// Nope, so setup the file context menu ...
				switch (selectedItem.Header)
				{
					case "Header Files":
					case "Source Files":
						projectTreeView.ContextMenu = projectTreeView.Resources["FolderContext"] as ContextMenu;
						break;
				}
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

		private void CC65SettingsMenuItem_Click(object sender, RoutedEventArgs e)
		{
			ShowCC65Settings();
		}

		private void AddExistingFileMenuItem_Click(object sender, RoutedEventArgs e)
		{
			AddExistingFile();
		}

		private void AddNewFileMenuItem_Click(object sender, RoutedEventArgs e)
		{
			AddNewFile();
		}

		private void RemoveFileMenuItem_Click(object sender, RoutedEventArgs e)
		{
			// Retrieve filename from current treeview selection ...
			var file = selectedItem.Header as string ?? string.Empty;

			// Close file and clear from the editor ...
			CloseFile();

			// Remove from project ...
			RemoveFileFromProject(file);

			// Re-populate the tree view
			PopulateTreeView();
		}

		#endregion

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

		/// <summary>
		/// Saves the current source file
		/// </summary>
		private void SaveFile()
		{
			if (string.IsNullOrEmpty(CurrentFileName))
			{
				SaveFileDialog dlg = new SaveFileDialog();
				dlg.DefaultExt = ".txt";

				if (dlg.ShowDialog() ?? false)
				{
					CurrentFileName = dlg.FileName;
				}
				else
				{
					return;
				}
			}

			textEditor.Save(CurrentFileName);
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
				CurrentFileName = dlg.FileName;
				textEditor.Load(CurrentFileName);
				textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(CurrentFileName));
			}
		}

		/// <summary>
		/// Closes the current source file
		/// </summary>
		private void CloseFile()
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

			// Reset the current file name ...
			CurrentFileName = string.Empty;
		}		

		/// <summary>
		/// Saves the current project settings
		/// </summary>
		private void SaveProject()
        {
			// Bail if no project loaded ...
			if (Project == null)
				return;

			// Convert project to JSON ...
			var asJSON = Project.AsJson();

			// Do we have a project filepath ? ...
			if (string.IsNullOrEmpty(ProjectFile))
            {
				SaveFileDialog dlg = new SaveFileDialog();
				dlg.DefaultExt = ".json";

				if (dlg.ShowDialog() ?? false)
				{
					ProjectFile = dlg.FileName;
				}
				else
				{
					return;
				}
			}

			// Write project details to file ...
			File.WriteAllText(ProjectFile, asJSON);
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
				ProjectFile = dlg.FileNames[0];
				var json = File.ReadAllText(ProjectFile);
				Project = Cc65Project.FromJson(json);

				projectInfo.Text = $"Project {Project.ProjectName} loaded";
				targetInfo.Text = $"Target: {Project.TargetPlatform}";

			}

			// Populate the tree view
			PopulateTreeView();
		}

		/// <summary>
		/// Closes the current project
		/// </summary>
		private void CloseProject()
        {
			Project = null;
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
                Header = $"{Project.ProjectName}",
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
			foreach (var hdrfile in Project.HeaderFiles)
			{
				var node = new TreeViewItem
				{
					Header = hdrfile,
					Tag = Path.Combine(Project.WorkingDirectory, hdrfile)
				};

				hdrFiles.Items.Add(node);
			}

			// Add the source files ...
			foreach (var srcfile in Project.InputFiles)
			{
				var node = new TreeViewItem
				{
					Header = srcfile,
					Tag = Path.Combine(Project.WorkingDirectory, srcfile)
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

			outputTextBox.AppendText($"Building {Project.InputFiles.Count} files for project [{Project.ProjectName}] targeting [{Project.TargetPlatform}]...\r\n");

			// Compile the project ...
			var result = await Cc65Build.Compile(Project);

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
				outputTextBox.AppendText($"Launching {Project.ProjectName} in emulator ...\r\n");

				var result = await Cc65Emulators.LaunchEmulator(Project, emulators);
			}
		}

		/// <summary>
		/// Prompt user to save modified file
		/// </summary>
		private void PromptForModifiedFile()
		{
			var result = MessageBox.Show("Do you want to save the changes ?", $"{CurrentFileName} - File Modified !", MessageBoxButton.YesNo, MessageBoxImage.Question);

			switch (result)
			{
				case MessageBoxResult.Yes:
					SaveFile();
					break;

				case MessageBoxResult.No:
					break;
			}
		}

		/// <summary>
		/// Display CC65 settings window
		/// </summary>
		private void ShowCC65Settings()
        {
			var dlg = new CC65SettingsWindow();
			dlg.ShowDialog();
        }

		/// <summary>
		/// Adds a new source/header file to the project
		/// </summary>
		private void AddNewFile()
        {
			OpenFileDialog dlg = new OpenFileDialog
			{
				Filter = "Source Files|*.c|Header Files|*.h",
				CheckFileExists = false,
				InitialDirectory = Project.WorkingDirectory
			};

			if (dlg.ShowDialog() ?? false)
			{
				var selectedFile = dlg.FileName;

				// Create empty file ...
				using (var fs = File.Create(selectedFile))
                {
					fs.Flush();
                }

				AddFileToProject(selectedFile);
			}

			// Re-populate the tree view
			PopulateTreeView();
		}

		/// <summary>
		/// Adds an existing source/header file to the project
		/// </summary>
		private void AddExistingFile()
		{
			OpenFileDialog dlg = new OpenFileDialog
			{
				Filter = "Source Files|*.c|Header Files|*.h",
				CheckFileExists = true,
				InitialDirectory = Project.WorkingDirectory
			};

			if (dlg.ShowDialog() ?? false)
			{
				var selectedFile = dlg.FileName;

				AddFileToProject(selectedFile);
			}

			// Re-populate the tree view
			PopulateTreeView();
		}

		/// <summary>
		/// Adds the specified filename to the project 
		/// </summary>
		/// <param name="filename"></param>
		private void AddFileToProject(string filename)
        {
			// TODO: Fix weird issue with saving file changes after initial addition

			var type = EstablishFileType(filename);

			// Add to list of project files ...
			switch (type)
			{
				case CC65FileTypes.SourceFile:
					Project.InputFiles.Add(Path.GetFileName(filename));
					break;

				case CC65FileTypes.IncludeFile:
					Project.HeaderFiles.Add(Path.GetFileName(filename));
					break;
			}
		}

		/// <summary>
		/// Removes the specified header/source file from the project
		/// </summary>
		/// <param name="filename"></param>
		private void RemoveFileFromProject(string filename)
		{
			var type = EstablishFileType(filename);

			switch (type)
            {
				// Remove source file ...
				case CC65FileTypes.SourceFile:
					Project.InputFiles.Remove(filename);
					break;

				// Remove header file ...
				case CC65FileTypes.IncludeFile:
					Project.HeaderFiles.Remove(filename);
					break;

				// Unknown file type, so do nothing ...
				default:
					break;
            }
		}

		/// <summary>
		/// Establish the file type from the filename extension
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		private CC65FileTypes EstablishFileType(string filename)
        {
			CC65FileTypes result = CC65FileTypes.None;
			var ext = Path.GetExtension(filename);

			switch (ext)
            {
				case ".h":
					result = CC65FileTypes.IncludeFile;
					break;

				case ".c":
					result = CC65FileTypes.SourceFile;
					break;

				// Unknown file type ...
				default:
					result = CC65FileTypes.None;
					break;
            }

			return result;
        }

        #endregion


    }	
}
