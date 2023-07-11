namespace FeaturesOverlayPresentation.Forms
{
    partial class UpdaterForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdaterForm));
            this.lblUpdateAnnoucement = new System.Windows.Forms.Label();
            this.lblOldVersion = new System.Windows.Forms.Label();
            this.lblNewVersion = new System.Windows.Forms.Label();
            this.lblFixedOldVersion = new System.Windows.Forms.Label();
            this.lblFixedNewVersion = new System.Windows.Forms.Label();
            this.changelogTextBox = new System.Windows.Forms.TextBox();
            this.downloadButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.lblFixedChangelogLatestVersion = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblUpdateAnnoucement
            // 
            resources.ApplyResources(this.lblUpdateAnnoucement, "lblUpdateAnnoucement");
            this.lblUpdateAnnoucement.Name = "lblUpdateAnnoucement";
            // 
            // lblOldVersion
            // 
            resources.ApplyResources(this.lblOldVersion, "lblOldVersion");
            this.lblOldVersion.Name = "lblOldVersion";
            // 
            // lblNewVersion
            // 
            resources.ApplyResources(this.lblNewVersion, "lblNewVersion");
            this.lblNewVersion.Name = "lblNewVersion";
            // 
            // lblFixedOldVersion
            // 
            resources.ApplyResources(this.lblFixedOldVersion, "lblFixedOldVersion");
            this.lblFixedOldVersion.Name = "lblFixedOldVersion";
            // 
            // lblFixedNewVersion
            // 
            resources.ApplyResources(this.lblFixedNewVersion, "lblFixedNewVersion");
            this.lblFixedNewVersion.Name = "lblFixedNewVersion";
            // 
            // changelogTextBox
            // 
            this.changelogTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.changelogTextBox, "changelogTextBox");
            this.changelogTextBox.Name = "changelogTextBox";
            this.changelogTextBox.ReadOnly = true;
            this.changelogTextBox.TabStop = false;
            // 
            // downloadButton
            // 
            resources.ApplyResources(this.downloadButton, "downloadButton");
            this.downloadButton.Name = "downloadButton";
            this.downloadButton.UseVisualStyleBackColor = true;
            this.downloadButton.Click += new System.EventHandler(this.DownloadButton_Click);
            // 
            // closeButton
            // 
            resources.ApplyResources(this.closeButton, "closeButton");
            this.closeButton.Name = "closeButton";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // lblFixedChangelogLatestVersion
            // 
            resources.ApplyResources(this.lblFixedChangelogLatestVersion, "lblFixedChangelogLatestVersion");
            this.lblFixedChangelogLatestVersion.Name = "lblFixedChangelogLatestVersion";
            // 
            // UpdateCheckerForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.lblFixedChangelogLatestVersion);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.downloadButton);
            this.Controls.Add(this.changelogTextBox);
            this.Controls.Add(this.lblFixedNewVersion);
            this.Controls.Add(this.lblFixedOldVersion);
            this.Controls.Add(this.lblNewVersion);
            this.Controls.Add(this.lblOldVersion);
            this.Controls.Add(this.lblUpdateAnnoucement);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdateCheckerForm";
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();
            this.Load += new System.EventHandler(this.UpdateCheckerForm_Load);

        }

        #endregion

        private System.Windows.Forms.Label lblUpdateAnnoucement;
        private System.Windows.Forms.Label lblOldVersion;
        private System.Windows.Forms.Label lblNewVersion;
        private System.Windows.Forms.Label lblFixedOldVersion;
        private System.Windows.Forms.Label lblFixedNewVersion;
        private System.Windows.Forms.TextBox changelogTextBox;
        private System.Windows.Forms.Button downloadButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Label lblFixedChangelogLatestVersion;
    }
}