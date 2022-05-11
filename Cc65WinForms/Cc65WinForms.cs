using Cc65WinForms.Enumerations;
using cc65Wrapper;

namespace Cc65WinForms
{
    public partial class Cc65WinForms : Form
    {
        // TODO: Add facility to save project file
        // TODO: Add filter support on 'Add File' dialog

        #region Field and properties

        private Cc65Project? project;
        private Cc65Emulators emulators;
        private string currentFile = string.Empty;
        private string projectFile = string.Empty;

        #endregion

        #region Class Constructor

        /// <summary>Initializes a new instance of the <see cref="Cc65WinForms" /> class.</summary>
        public Cc65WinForms()
        {
            InitializeComponent();

            InitialiseApp();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initialises the application.
        /// </summary>
        private void InitialiseApp()
        {
            projectToolStripStatusLabel.Text = "No project loaded";
            projectToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem.Enabled = false;

            // Load emulator settings ...
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), @"Test Files");
            filepath = Path.Combine(filepath, "emulators.json");
            var json = File.ReadAllText(filepath);
            emulators = Cc65Emulators.FromJson(json);
        }

        /// <summary>
        /// Populates the TreeView.
        /// </summary>
        private void PopulateTreeView()
        {
            ClearTreeView();

            // Add root node ...
            var rootNode = new TreeNode();
            rootNode.Name = $"{project.ProjectName}";
            rootNode.Text = $"{project.ProjectName}";
            rootNode.Tag = string.Empty;
            treeView1.Nodes.Add(rootNode);

            // Add 'Header Files' node ...
            var hdrFiles = new TreeNode();
            hdrFiles.Name = "Header Files";
            hdrFiles.Text = "Header Files";
            hdrFiles.Tag = string.Empty;
            rootNode.Nodes.Add(hdrFiles);

            // Add 'Source Files' node ...
            var srcFiles = new TreeNode();
            srcFiles.Name = "Source Files";
            srcFiles.Text = "Source Files";
            srcFiles.Tag = string.Empty;
            rootNode.Nodes.Add(srcFiles);

            // Add the header files ...
            foreach (var hdrfile in project.HeaderFiles)
            {
                var node = new TreeNode
                {
                    Name = hdrfile,
                    Text = hdrfile,
                    Tag = Path.Combine(project.WorkingDirectory, hdrfile)
                };

                hdrFiles.Nodes.Add(node);
            }

            // Add the source files ...
            foreach (var srcfile in project.InputFiles)
            {
                var node = new TreeNode
                {
                    Name = srcfile,
                    Text = srcfile,
                    Tag = Path.Combine(project.WorkingDirectory, srcfile)
                };
                
                srcFiles.Nodes.Add(node);
            }

            treeView1.ExpandAll();
        }

        /// <summary>
        /// Clears the TreeView.
        /// </summary>
        private void ClearTreeView()
        {
            treeView1.Nodes.Clear();
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
                
            return builtOK;
        }

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
        /// Saves the file.
        /// </summary>
        private void SaveFile()
        {
            File.WriteAllText(currentFile, editTextBox.Text);
            saveToolStripMenuItem.Enabled = false;
        }

        /// <summary>
        /// Editors the text changed.
        /// </summary>
        private void EditorTextChanged()
        {
            if (editTextBox.Text.Length > 0)
            {
                saveToolStripMenuItem.Enabled = true;
            }
        }

        /// <summary>
        /// Closes the project.
        /// </summary>
        private void CloseProject()
        {
            project = null;
            projectToolStripStatusLabel.Text = "No project loaded";
            projectToolStripMenuItem.Enabled = false;
            ClearTreeView();
            editTextBox.Clear();
            saveToolStripMenuItem.Enabled = false;
        }

        /// <summary>
        /// Opens the project.
        /// </summary>
        private void OpenProject()
        {
            DialogResult dr = this.openFileDialog1.ShowDialog();

            if (dr == DialogResult.OK)
            {
                // Load the project JSON ...
                projectFile = openFileDialog1.FileNames.First();
                var json = File.ReadAllText(projectFile);
                project = Cc65Project.FromJson(json);

                projectToolStripStatusLabel.Text = $"Project {project.ProjectName} loaded";
                projectToolStripMenuItem.Enabled = true;
            }

            // Populate the tree view
            PopulateTreeView();
        }

        private void AddNewFileToProject(CC65FileTypes filter = CC65FileTypes.None)
        {
            var addFileWin = new AddFile();
            addFileWin.Project = project;
            addFileWin.TypeFilter = filter;
            addFileWin.ShowDialog();

            while (addFileWin.Visible)
            {
                Thread.Sleep(250);
            }

            // Grab modified project ...
            project = addFileWin.Project;

            // Populate the tree view
            PopulateTreeView();
        }

        #endregion

        #region Event Handlers
        private async void buildToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await BuildProject();
        }

        private void treeView1_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Tag as string != string.Empty)
            {
                currentFile = (string)treeView1.SelectedNode.Tag;

                editTextBox.Clear();

                var text = File.ReadAllText(currentFile);
                editTextBox.Text = text;
                saveToolStripMenuItem.Enabled = false;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var aboutBox = new AboutBox();
            aboutBox.ShowDialog();
        }

        private void editTextBox_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            EditorTextChanged();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseProject();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenProject();
        }

        private void treeView1_Click(object sender, TreeNodeMouseClickEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    // Is selected node header or source file ? ...
                    if (e.Node.Tag as string != string.Empty)
                    {
                        // Yep, so retrieve the file path and clear the editor ...
                        currentFile = (string)e.Node.Tag;
                        editTextBox.Clear();

                        // Read the file and populate the editor ...
                        var text = File.ReadAllText(currentFile);
                        editTextBox.Text = text;
                        saveToolStripMenuItem.Enabled = false;
                    }
                    break;

                case MouseButtons.Right:
                    switch (e.Node.Text)
                    {
                        case "Header Files":
                            AddNewFileToProject(CC65FileTypes.SourceFile);
                            break;

                        case "Source Files":
                            AddNewFileToProject(CC65FileTypes.IncludeFile);
                            break;
                    }
                    break;

                default:
                    break;
            }

        }

        private async void executeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await ExecuteProject();
        }

        private void cC65SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var settings = new Cc65Settings();
            settings.ShowDialog();
        }

        private void projectInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewFileToProject();
        }

        #endregion
    }
}