namespace Mp3_2_AudioBook
{
    partial class ConverterMainForm
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
            btnSelectFile = new Button();
            lblDrop = new Label();
            lblFileName = new Label();
            btnConvert = new Button();
            btnSelectAnother = new Button();
            progressBar = new ProgressBar();
            lblStatus = new Label();
            btnCancel = new Button();
            menuStrip1 = new MenuStrip();
            mnuFile = new ToolStripMenuItem();
            mnuOpen = new ToolStripMenuItem();
            mnuExit = new ToolStripMenuItem();
            mnuOptions = new ToolStripMenuItem();
            mnuBitrate = new ToolStripMenuItem();
            mnuHelp = new ToolStripMenuItem();
            mnuAbout = new ToolStripMenuItem();
            mnuEncoders = new ToolStripMenuItem();
            mnuShowMetadata = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // btnSelectFile
            // 
            btnSelectFile.Location = new Point(444, 281);
            btnSelectFile.Name = "btnSelectFile";
            btnSelectFile.Size = new Size(94, 29);
            btnSelectFile.TabIndex = 0;
            btnSelectFile.Text = "Select File";
            btnSelectFile.UseVisualStyleBackColor = true;
            // 
            // lblDrop
            // 
            lblDrop.AllowDrop = true;
            lblDrop.AutoSize = true;
            lblDrop.Location = new Point(428, 249);
            lblDrop.Name = "lblDrop";
            lblDrop.Size = new Size(130, 20);
            lblDrop.TabIndex = 1;
            lblDrop.Text = "Drag a file here or";
            // 
            // lblFileName
            // 
            lblFileName.AutoSize = true;
            lblFileName.Location = new Point(469, 352);
            lblFileName.Name = "lblFileName";
            lblFileName.Size = new Size(0, 20);
            lblFileName.TabIndex = 2;
            // 
            // btnConvert
            // 
            btnConvert.Location = new Point(580, 158);
            btnConvert.Name = "btnConvert";
            btnConvert.Size = new Size(94, 29);
            btnConvert.TabIndex = 3;
            btnConvert.Text = "Convert";
            btnConvert.UseVisualStyleBackColor = true;
            btnConvert.Visible = false;
            // 
            // btnSelectAnother
            // 
            btnSelectAnother.Location = new Point(418, 111);
            btnSelectAnother.Name = "btnSelectAnother";
            btnSelectAnother.Size = new Size(159, 29);
            btnSelectAnother.TabIndex = 4;
            btnSelectAnother.Text = "Select Another File";
            btnSelectAnother.UseVisualStyleBackColor = true;
            btnSelectAnother.Visible = false;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(98, 147);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(125, 29);
            progressBar.TabIndex = 5;
            progressBar.Visible = false;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(334, 185);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(50, 20);
            lblStatus.TabIndex = 6;
            lblStatus.Text = "Ready";
            lblStatus.Visible = false;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(179, 292);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(94, 29);
            btnCancel.TabIndex = 7;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Visible = false;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { mnuFile, mnuOptions, mnuHelp });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 28);
            menuStrip1.TabIndex = 8;
            menuStrip1.Text = "menuStrip1";
            // 
            // mnuFile
            // 
            mnuFile.DropDownItems.AddRange(new ToolStripItem[] { mnuOpen, mnuExit });
            mnuFile.Name = "mnuFile";
            mnuFile.Size = new Size(46, 24);
            mnuFile.Text = "File";
            // 
            // mnuOpen
            // 
            mnuOpen.Name = "mnuOpen";
            mnuOpen.Size = new Size(128, 26);
            mnuOpen.Text = "Open";
            // 
            // mnuExit
            // 
            mnuExit.Name = "mnuExit";
            mnuExit.Size = new Size(128, 26);
            mnuExit.Text = "Exit";
            // 
            // mnuOptions
            // 
            mnuOptions.DropDownItems.AddRange(new ToolStripItem[] { mnuBitrate });
            mnuOptions.Name = "mnuOptions";
            mnuOptions.Size = new Size(75, 24);
            mnuOptions.Text = "Options";
            // 
            // mnuBitrate
            // 
            mnuBitrate.Name = "mnuBitrate";
            mnuBitrate.Size = new Size(136, 26);
            mnuBitrate.Text = "Bitrate";
            // 
            // mnuHelp
            // 
            mnuHelp.DropDownItems.AddRange(new ToolStripItem[] { mnuAbout, mnuEncoders, mnuShowMetadata });
            mnuHelp.Name = "mnuHelp";
            mnuHelp.Size = new Size(55, 24);
            mnuHelp.Text = "Help";
            // 
            // mnuAbout
            // 
            mnuAbout.Name = "mnuAbout";
            mnuAbout.Size = new Size(224, 26);
            mnuAbout.Text = "About";
            // 
            // mnuEncoders
            // 
            mnuEncoders.Name = "mnuEncoders";
            mnuEncoders.Size = new Size(224, 26);
            mnuEncoders.Text = "Available Encoders";
            // 
            // mnuShowMetadata
            // 
            mnuShowMetadata.Name = "mnuShowMetadata";
            mnuShowMetadata.Size = new Size(224, 26);
            mnuShowMetadata.Text = "Show File Metadata";
            // 
            // ConverterMainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnCancel);
            Controls.Add(lblStatus);
            Controls.Add(progressBar);
            Controls.Add(btnSelectAnother);
            Controls.Add(btnConvert);
            Controls.Add(lblFileName);
            Controls.Add(lblDrop);
            Controls.Add(btnSelectFile);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "ConverterMainForm";
            Text = "Form1";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnSelectFile;
        private Label lblDrop;
        private Label lblFileName;
        private Button btnConvert;
        private Button btnSelectAnother;
        private ProgressBar progressBar;
        private Label lblStatus;
        private Button btnCancel;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem mnuFile;
        private ToolStripMenuItem mnuHelp;
        private ToolStripMenuItem mnuAbout;
        private ToolStripMenuItem mnuOpen;
        private ToolStripMenuItem mnuExit;
        private ToolStripMenuItem mnuEncoders;
        private ToolStripMenuItem mnuOptions;
        private ToolStripMenuItem mnuBitrate;
        private ToolStripMenuItem mnuShowMetadata;
    }
}