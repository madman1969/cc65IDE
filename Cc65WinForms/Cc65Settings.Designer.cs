namespace Cc65WinForms
{
    partial class Cc65Settings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Cc65Settings));
            this.cc65CompilerConfigurationBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.cc65HomeTextBox = new System.Windows.Forms.TextBox();
            this.cc65IncludeTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ld65CfgTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ld65LibTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.makeHomeTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.cc65CompilerConfigurationBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // cc65CompilerConfigurationBindingSource
            // 
            this.cc65CompilerConfigurationBindingSource.DataSource = typeof(cc65Wrapper.Cc65CompilerConfiguration);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "CC65_HOME";
            // 
            // cc65HomeTextBox
            // 
            this.cc65HomeTextBox.Location = new System.Drawing.Point(158, 26);
            this.cc65HomeTextBox.Name = "cc65HomeTextBox";
            this.cc65HomeTextBox.Size = new System.Drawing.Size(327, 31);
            this.cc65HomeTextBox.TabIndex = 1;
            this.cc65HomeTextBox.TextChanged += new System.EventHandler(this.cc65SettingsTextBox_TextChanged);
            // 
            // cc65IncludeTextBox
            // 
            this.cc65IncludeTextBox.Location = new System.Drawing.Point(158, 82);
            this.cc65IncludeTextBox.Name = "cc65IncludeTextBox";
            this.cc65IncludeTextBox.Size = new System.Drawing.Size(327, 31);
            this.cc65IncludeTextBox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(58, 85);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 25);
            this.label2.TabIndex = 2;
            this.label2.Text = "CC65_INC";
            // 
            // ld65CfgTextBox
            // 
            this.ld65CfgTextBox.Location = new System.Drawing.Point(158, 138);
            this.ld65CfgTextBox.Name = "ld65CfgTextBox";
            this.ld65CfgTextBox.Size = new System.Drawing.Size(327, 31);
            this.ld65CfgTextBox.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(56, 141);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 25);
            this.label3.TabIndex = 4;
            this.label3.Text = "LD65_CFG";
            // 
            // ld65LibTextBox
            // 
            this.ld65LibTextBox.Location = new System.Drawing.Point(158, 194);
            this.ld65LibTextBox.Name = "ld65LibTextBox";
            this.ld65LibTextBox.Size = new System.Drawing.Size(327, 31);
            this.ld65LibTextBox.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(65, 197);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 25);
            this.label4.TabIndex = 6;
            this.label4.Text = "LD65_LIB";
            // 
            // makeHomeTextBox
            // 
            this.makeHomeTextBox.Location = new System.Drawing.Point(158, 250);
            this.makeHomeTextBox.Name = "makeHomeTextBox";
            this.makeHomeTextBox.Size = new System.Drawing.Size(327, 31);
            this.makeHomeTextBox.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(30, 253);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(118, 25);
            this.label5.TabIndex = 8;
            this.label5.Text = "MAKE_HOME";
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(240, 303);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(112, 34);
            this.okButton.TabIndex = 10;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(373, 303);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(112, 34);
            this.cancelButton.TabIndex = 11;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // Cc65Settings
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(540, 362);
            this.ControlBox = false;
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.makeHomeTextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.ld65LibTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ld65CfgTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cc65IncludeTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cc65HomeTextBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Cc65Settings";
            this.Text = "Cc65 Settings";
            ((System.ComponentModel.ISupportInitialize)(this.cc65CompilerConfigurationBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private BindingSource cc65CompilerConfigurationBindingSource;
        private Label label1;
        private TextBox cc65HomeTextBox;
        private TextBox cc65IncludeTextBox;
        private Label label2;
        private TextBox ld65CfgTextBox;
        private Label label3;
        private TextBox ld65LibTextBox;
        private Label label4;
        private TextBox makeHomeTextBox;
        private Label label5;
        private Button okButton;
        private Button cancelButton;
    }
}