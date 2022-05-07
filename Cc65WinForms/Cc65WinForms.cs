using cc65Wrapper;

namespace Cc65WinForms
{
    public partial class Cc65WinForms : Form
    {
        Cc65Project? project;
        string currentFile = string.Empty;

        public Cc65WinForms()
        {
            InitializeComponent();

            InitialiseApp();
        }
        public void InitialiseApp()
        {
            toolStripStatusLabel1.Text = "No project loaded";
            projectToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            project = null;
            toolStripStatusLabel1.Text = "No project loaded";
            projectToolStripMenuItem.Enabled = false;
            ClearTreeView();
            editTextBox.Clear();
            saveToolStripMenuItem.Enabled=false;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = this.openFileDialog1.ShowDialog();

            if (dr == DialogResult.OK)
            {
                // Load the project JSON ...
                var projectFile = openFileDialog1.FileNames.First();                
                var json = File.ReadAllText(projectFile);
                project = Cc65Project.FromJson(json);

                toolStripStatusLabel1.Text = $"Project {project.ProjectName} loaded";
                projectToolStripMenuItem.Enabled = true;
            }

            // Populate the tree view
            PopulateTreeView();
        }

        private void PopulateTreeView()
        {
            ClearTreeView();

            var rootNode = new TreeNode();
            rootNode.Name = $"{project.ProjectName}";
            rootNode.Text = $"{project.ProjectName}";
            rootNode.Tag = string.Empty;
            treeView1.Nodes.Add(rootNode);

            var hdrFiles = new TreeNode();
            hdrFiles.Name = "Header Files";
            hdrFiles.Text = "Header Files";
            hdrFiles.Tag = string.Empty;
            rootNode.Nodes.Add(hdrFiles);

            var srcFiles = new TreeNode();
            srcFiles.Name = "Source Files";
            srcFiles.Text = "Source Files";
            srcFiles.Tag = string.Empty;
            rootNode.Nodes.Add(srcFiles);

            foreach (var srcfile in project.InputFiles)
            {
                var node = new TreeNode
                {
                    Name = srcfile,
                    Text = srcfile,
                    Tag = Path.Combine(project.WorkingDirectory, srcfile)
                };
                // treeView1.Nodes.Add(node);
                srcFiles.Nodes.Add(node);
            }

            treeView1.ExpandAll();
        }

        private void ClearTreeView()
        {
            treeView1.Nodes.Clear();
        }

        private async void buildToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Compile the project ...
            var result = await Cc65Build.Compile(project);

            if (result.ExitCode != 0)
            {
                var errorList = Cc65Build.ErrorsAsList(result);
                foreach (var error in errorList)
                {
                    outputTextBox.AppendText($"{error}\r\n");
                }
            }
            else
                outputTextBox.AppendText("Build successful\r\n");

        }

        private void treeView1_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Tag != string.Empty)
            {
                currentFile = (string)treeView1.SelectedNode.Tag;

                editTextBox.Clear();

                var text = File.ReadAllText(currentFile);
                editTextBox.Text = text;
                saveToolStripMenuItem.Enabled = false;
            }                
        }

        private void editTextBox_TextChanged(object sender, EventArgs e)
        {
            if (editTextBox.Text.Length > 0)
            {
                saveToolStripMenuItem.Enabled = true;
            }
                
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            File.WriteAllText(currentFile, editTextBox.Text);
            saveToolStripMenuItem.Enabled = false;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var aboutBox = new AboutBox();
            aboutBox.ShowDialog();
        }
    }
}