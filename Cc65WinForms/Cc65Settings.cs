using cc65Wrapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cc65WinForms
{
    public partial class Cc65Settings : Form
    {
        private Cc65CompilerConfiguration configuration = new Cc65CompilerConfiguration();
        private bool settingsChanged;

        public Cc65Settings()
        {
            InitializeComponent();

            cc65HomeTextBox.Text = configuration.cc65Home;
            cc65IncludeTextBox.Text = configuration.cc65Include;
            ld65CfgTextBox.Text = configuration.ld65Cfg;
            ld65LibTextBox.Text = configuration.ld65Lib;
            makeHomeTextBox.Text = configuration.makeHome;

            settingsChanged = false;
            okButton.Enabled = false;
            cancelButton.Select();
        }

        private void cc65SettingsTextBox_TextChanged(object sender, EventArgs e)
        {
            settingsChanged = true;
            okButton.Enabled = true;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            configuration.SaveConfiguration();
            this.Close();
        }
    }
}
