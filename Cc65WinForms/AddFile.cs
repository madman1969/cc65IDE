using Cc65WinForms.Enumerations;
using cc65Wrapper;

namespace Cc65WinForms
{
    public partial class AddFile : Form
    {
        #region Fields and properties

        Cc65Project _project;
        CC65FileTypes _typeFilter;

        public Cc65Project Project { get => _project; set => _project = value; }
        public CC65FileTypes TypeFilter { get => _typeFilter; set => _typeFilter = value; }

        #endregion

        public AddFile()
        {
            InitializeComponent();

            // Default to Source file ...
            fileTypeComboBox.SelectedIndex = 0;
            newFileTextBox.Select();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            CreateNewProjectFile();

            this.Close();
        }

        private void CreateNewProjectFile()
        {
            // Retrieve new file name ...
            var newFile = newFileTextBox.Text;


            // Ensure extension is present ...
            switch (fileTypeComboBox.SelectedIndex)
            {
                case 0:
                    if (Path.GetExtension(newFile) != ".c")
                        newFile = $"{newFile}.c";
                    break;

                case 1:
                    if (Path.GetExtension(newFile) != ".h")
                        newFile = $"{newFile}.h";
                    break;
            }

            // Attempt to create the new file ...
            try
            {
                var newPath = Path.Combine(Project.WorkingDirectory, newFile);

                if (!File.Exists(newPath))
                {
                    using (var sw = File.CreateText(newPath))
                    {
                        sw.WriteLine("");
                    }
                        
                }

                // Add to list of project files ...
                switch (fileTypeComboBox.SelectedIndex)
                {
                    case 0:
                        Project.InputFiles.Add(newFile);
                        break;

                    case 1:
                        Project.HeaderFiles.Add(newFile);  
                        break;
                }


            } catch { }
        }
    }
}
