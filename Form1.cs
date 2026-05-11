using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Mp3_2_AudioBook
{
    public partial class ConverterMainForm : Form
    {
        private const int FormWidth = 600;
        private const int FormHeight = 400;
        private const int DropZoneWidth = 400;
        private const int DropZoneHeight = 200;
        private int CenterX => this.ClientSize.Width / 2; // Computed once, reused everywhere
        private string selectedFilePath = null;
        private static readonly (string Label, int Value)[] BitrateOptions =
{
    ("64 kbps (Low)",       64_000),
    ("96 kbps (Default)",   96_000),
    ("128 kbps (Standard)", 128_000),
    ("192 kbps (High)",     192_000),
    ("256 kbps (Ultra)",    256_000),
};
        private const int DefaultBitrateIndex = 1;
        private int selectedBitrate = BitrateOptions[DefaultBitrateIndex].Value;
        private CancellationTokenSource cancellationTokenSource = null;
        public ConverterMainForm()
        {
            InitializeComponent();
            SetupWindow();       // Only window-level properties
            SetupDropZone();     // Only the lblDrop control
            SetupButtons();      // All buttons
            SetupProgressArea(); // Progress bar + status label
            SetupFileLabel();    // lblFileName (depends on lblDrop, so comes after)
            SetupMenuButtons();
        }

        private void SetupWindow()
        {
            this.Size = new Size(FormWidth, FormHeight);
            this.MinimumSize = this.MaximumSize = new Size(FormWidth, FormHeight);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "MP3 to AudioBook Converter";
        }
        private void SetupDropZone()
        {
            lblDrop.AutoSize = false;
            lblDrop.Visible = true;
            lblDrop.Size = new Size(DropZoneWidth, DropZoneHeight);
            lblDrop.Location = new Point(CenterX - DropZoneWidth / 2, 50);
            lblDrop.TextAlign = ContentAlignment.MiddleCenter;
            lblDrop.Text = "Drag and drop files here";
            lblDrop.BackColor = Color.WhiteSmoke;
            lblDrop.AllowDrop = true;
            lblDrop.Padding = new Padding(60);
            lblDrop.Paint += LblDrop_Paint;
            lblDrop.DragEnter += LblDrop_DragEnter;
            lblDrop.DragLeave += LblDrop_DragLeave;
            lblDrop.DragDrop += LblDrop_DragDrop;
        }
        private void SetupFileLabel()
        {
            lblFileName.Parent = lblDrop;
            lblFileName.AutoSize = false;
            lblFileName.Size = new Size(DropZoneWidth, 20);
            lblFileName.Location = new Point(0, 150);
            lblFileName.BackColor = Color.Transparent;
            lblFileName.TextAlign = ContentAlignment.MiddleCenter;
        }
        private void resetView()
        {
            progressBar.Visible = false;
            lblStatus.Visible = false;
            lblFileName.Text = string.Empty;
            lblDrop.Text = "Drag and drop files here";
            lblDrop.Visible = true;
            selectedFilePath = null;
            btnSelectFile.Visible = true;
            btnCancel.Visible = false;
            btnConvert.Visible = false;
            mnuShowMetadata.Enabled = false;
        }
        private void SetupButtons()
        {
            ConfigureButtonStart(btnSelectFile, width: 150, height: 40, x: CenterX - 75, y: 270, text: "Select File", true);
            ConfigureButtonStart(btnConvert, width: 150, height: 40, x: CenterX - 170, y: 270, text: "Convert");
            ConfigureButtonStart(btnCancel, width: 150, height: 40, x: CenterX + 30, y: 270, text: "Cancel");
            btnSelectFile.Click += SelectFile_Click;
            btnCancel.Click += btnCancel_Click;
            btnConvert.Click += BtnConvert_Click;
            mnuOpen.Click += SelectFile_Click;
            mnuEncoders.Click += mnuEncoders_Click;
            mnuExit.Click += (s, e) => Application.Exit(); // Simple one-line exit
            mnuShowMetadata.Click += MnuShowMetadata_click;
        }
        private void ConfigureButtonStart(Button btn, int width, int height, int x, int y, string text, Boolean visibility = false)
        {
            btn.Size = new Size(width, height);
            btn.Location = new Point(x, y);
            btn.Text = text;
            btn.Visible = visibility;
        }
        private void SetupProgressArea()
        {
            progressBar.Visible = false;
            progressBar.Size = new Size(DropZoneWidth, 25);
            progressBar.Location = new Point(CenterX - DropZoneWidth / 2, 210);
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;

            lblStatus.Visible = false;
            lblStatus.Size = new Size(DropZoneWidth, 20);
            //lblStatus.Location = new Point(CenterX - DropZoneWidth / 2, 310);
            lblStatus.Location = new Point(CenterX - DropZoneWidth / 2, 192);
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            lblStatus.Text = string.Empty;
        }
        private void SetupMenuButtons()
        {
            foreach (var (label, value) in BitrateOptions)
            {
                // Setup bitrate Options
                ToolStripMenuItem item = new ToolStripMenuItem(label);
                item.Tag = value;
                item.CheckOnClick = true;
                item.Click += BitrateItem_Click;
                mnuBitrate.DropDownItems.Add(item);
            }
            ((ToolStripMenuItem)mnuBitrate.DropDownItems[DefaultBitrateIndex]).Checked = true; // Default

            // Setup Metadata menu
            mnuShowMetadata.Enabled = false;
            mnuShowMetadata.ToolTipText = "Select file to enable";
        }
        private void LblDrop_Paint(object sender, PaintEventArgs e)
        {
            Control paintControl = sender as Control;
            Pen dashedPen = new Pen(Color.DarkGray, 2); // 2 pixels wide, grey color
            dashedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash; // Makes it dashed
            // Draw the rectangle just inside the edges of the label
            e.Graphics.DrawRectangle(dashedPen,
                1, 1, paintControl.Width - 2, paintControl.Height - 2);

            dashedPen.Dispose(); // Clean up memory
        }
        private void LblDrop_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
                // Visual Feedback: Change background to a light blue when hovering
                lblDrop.BackColor = Color.LightCyan;
            }
        }
        private void LblDrop_DragLeave(object sender, EventArgs e)
        {
            // Revert to the original background color
            lblDrop.BackColor = Color.WhiteSmoke;
        }
        private void LblDrop_DragDrop(object sender, DragEventArgs e)
        {
            // Revert background color back to normal after dropping
            lblDrop.BackColor = Color.WhiteSmoke;
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0)
            {
                HandleFileSelected(files[0]);
            }
        }
        private string ProcessFileName(string fullPath, int maxLength)
        {    // Extract filename
            string fileName = Path.GetFileName(fullPath);
            // Check if long
            if (fileName.Length > maxLength)
            {
                return fileName.Substring(0, maxLength) + "...";
            }
            return fileName;
        }
        private void HandleFileSelected(string file)
        {
            if (string.IsNullOrEmpty(file)) return;
            selectedFilePath = file;
            lblDrop.Text = "File is ready to convert";
            lblFileName.Text = ProcessFileName(file, 30);
            btnSelectFile.Visible = false;
            btnConvert.Visible = true;
            btnCancel.Visible = true;
            mnuShowMetadata.Enabled = true;
        }
        private void SelectFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    HandleFileSelected(openFileDialog.FileName);
                }
            }
        }
        private void mnuEncoders_Click(object sender, EventArgs e)
        {
            string title = "Encoders in the Device";
            string[] encoders = AudioConverter.GetAvailableEncoders();
            string message = string.Join("\n", encoders);
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private async void btnCancel_Click(object sender, EventArgs e)
        {
            cancellationTokenSource?.Cancel();
            resetView();
        }
        private async void BtnConvert_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFilePath))
            {
                MessageBox.Show("No file selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string outputPath = Path.ChangeExtension(selectedFilePath, ".m4a");
            if (File.Exists(outputPath))
            {
                var result = MessageBox.Show(
                    "Error. File already exists. Do you want to overwrite it?",
                    "File Already Exists",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );
                if (result == DialogResult.No)
                {
                    return;
                }
            }
            btnConvert.Enabled = false;
            IProgress<int> progress = progressBarReporting();
            AudioConverter converter = new AudioConverter();
            cancellationTokenSource = new CancellationTokenSource();
            try
            {
                await Task.Run(() =>
                {
                    converter.Convert(selectedFilePath, outputPath, selectedBitrate, progress, cancellationTokenSource.Token);
                }, cancellationTokenSource.Token);
                lblStatus.Text = "File is ready";
                MessageBox.Show("Conversion was completed sucessfully!", "Great Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Conversion cancelled", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (File.Exists(outputPath))
                {
                    try { File.Delete(outputPath); } catch { /* ignore if still locked */ }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Conversion Failed" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                //Enable UI
                btnConvert.Enabled = true;
                cancellationTokenSource?.Dispose();
                cancellationTokenSource = null;
                resetView();
            }
        }
        private void BitrateItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem clickedItem = sender as ToolStripMenuItem;
            if (clickedItem == null) return;
            foreach (ToolStripItem item in clickedItem.Owner.Items)
            {
                if (item is ToolStripMenuItem menuItem) menuItem.Checked = false;
            }
            clickedItem.Checked = true;
            selectedBitrate = (int)clickedItem.Tag;
        }
        private void MnuShowMetadata_click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFilePath))
            {
                MessageBox.Show("First select a MP3 file.", "No file selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                var metadata = MetadataHandler.ReadMetadata(selectedFilePath);
                string info = $"Title: {metadata.Title}\n" +
                    $"Author: {metadata.Author}\n" +
                    $"Year: {metadata.ReleaseYear}\n";

                info += metadata.Cover != null ? "Contains Cover Art" : "No Cover Art ";
                info += $"\n Number of Chapters: {metadata.Chapters.Count}\n\n";
                if (metadata.Chapters.Count > 0)
                {
                    info += "Chapter List:\n";
                    foreach (var chapter in metadata.Chapters)
                    {
                        TimeSpan start = TimeSpan.FromMilliseconds(chapter.StartTime);
                        TimeSpan end = TimeSpan.FromMilliseconds(chapter.EndTime);
                        info += $"  {chapter.Title}: {start:hh\\:mm\\:ss} - {end:hh\\:mm\\:ss}\n";
                    }
                }
                MessageBox.Show(info, "File Metadata", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reading metadata: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private IProgress<int> progressBarReporting()
        {
            progressBar.Visible = true;
            progressBar.Value = 0;
            lblStatus.Visible = true;
            lblStatus.Text = "Converting...0%";
            lblDrop.Visible = false;
            lblDrop.Text = string.Empty;
            lblFileName .Visible = false;
            var progress = new Progress<int>(percent =>
            {
                progressBar.Value = percent;
                lblStatus.Text = "Converting file..." + percent.ToString() + "%";
            });
            return progress;
        }
    }
}
