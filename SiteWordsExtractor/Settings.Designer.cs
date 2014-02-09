namespace SiteWordsExtractor
{
    partial class Settings
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
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.extractedAttributes = new System.Windows.Forms.TextBox();
            this.scrappedTags = new System.Windows.Forms.TextBox();
            this.statFilename = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.reportsRoot = new System.Windows.Forms.TextBox();
            this.defaultURL = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.maxSiteDepth = new System.Windows.Forms.NumericUpDown();
            this.maxRedirects = new System.Windows.Forms.NumericUpDown();
            this.minHttpDelay = new System.Windows.Forms.NumericUpDown();
            this.httpTimeoutSec = new System.Windows.Forms.NumericUpDown();
            this.maxPagesPerSite = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.buttonReset = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxSiteDepth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxRedirects)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minHttpDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.httpTimeoutSec)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxPagesPerSite)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(12, 327);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 12;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(93, 327);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 13;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.extractedAttributes);
            this.groupBox1.Controls.Add(this.scrappedTags);
            this.groupBox1.Controls.Add(this.statFilename);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.reportsRoot);
            this.groupBox1.Controls.Add(this.defaultURL);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(8, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(376, 152);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Application Settings";
            // 
            // extractedAttributes
            // 
            this.extractedAttributes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.extractedAttributes.Location = new System.Drawing.Point(109, 124);
            this.extractedAttributes.Name = "extractedAttributes";
            this.extractedAttributes.Size = new System.Drawing.Size(259, 20);
            this.extractedAttributes.TabIndex = 6;
            // 
            // scrappedTags
            // 
            this.scrappedTags.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scrappedTags.Location = new System.Drawing.Point(109, 97);
            this.scrappedTags.Name = "scrappedTags";
            this.scrappedTags.Size = new System.Drawing.Size(259, 20);
            this.scrappedTags.TabIndex = 5;
            // 
            // statFilename
            // 
            this.statFilename.Location = new System.Drawing.Point(109, 71);
            this.statFilename.Name = "statFilename";
            this.statFilename.Size = new System.Drawing.Size(100, 20);
            this.statFilename.TabIndex = 4;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(343, 45);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(25, 20);
            this.button1.TabIndex = 3;
            this.button1.Text = "...";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // reportsRoot
            // 
            this.reportsRoot.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.reportsRoot.Location = new System.Drawing.Point(109, 45);
            this.reportsRoot.Name = "reportsRoot";
            this.reportsRoot.Size = new System.Drawing.Size(229, 20);
            this.reportsRoot.TabIndex = 2;
            // 
            // defaultURL
            // 
            this.defaultURL.Location = new System.Drawing.Point(109, 19);
            this.defaultURL.Name = "defaultURL";
            this.defaultURL.Size = new System.Drawing.Size(256, 20);
            this.defaultURL.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 128);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(97, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "extracted attributes";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 101);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "scrapped tags";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "statistics filename";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "reports root";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "default URL";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.maxSiteDepth);
            this.groupBox2.Controls.Add(this.maxRedirects);
            this.groupBox2.Controls.Add(this.minHttpDelay);
            this.groupBox2.Controls.Add(this.httpTimeoutSec);
            this.groupBox2.Controls.Add(this.maxPagesPerSite);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Location = new System.Drawing.Point(8, 170);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(376, 151);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Crawler Settings";
            // 
            // maxSiteDepth
            // 
            this.maxSiteDepth.Location = new System.Drawing.Point(135, 122);
            this.maxSiteDepth.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.maxSiteDepth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.maxSiteDepth.Name = "maxSiteDepth";
            this.maxSiteDepth.Size = new System.Drawing.Size(74, 20);
            this.maxSiteDepth.TabIndex = 11;
            this.maxSiteDepth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // maxRedirects
            // 
            this.maxRedirects.Location = new System.Drawing.Point(135, 96);
            this.maxRedirects.Name = "maxRedirects";
            this.maxRedirects.Size = new System.Drawing.Size(74, 20);
            this.maxRedirects.TabIndex = 10;
            // 
            // minHttpDelay
            // 
            this.minHttpDelay.Location = new System.Drawing.Point(135, 70);
            this.minHttpDelay.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.minHttpDelay.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.minHttpDelay.Name = "minHttpDelay";
            this.minHttpDelay.Size = new System.Drawing.Size(74, 20);
            this.minHttpDelay.TabIndex = 9;
            this.minHttpDelay.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // httpTimeoutSec
            // 
            this.httpTimeoutSec.Location = new System.Drawing.Point(135, 44);
            this.httpTimeoutSec.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.httpTimeoutSec.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.httpTimeoutSec.Name = "httpTimeoutSec";
            this.httpTimeoutSec.Size = new System.Drawing.Size(74, 20);
            this.httpTimeoutSec.TabIndex = 8;
            this.httpTimeoutSec.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // maxPagesPerSite
            // 
            this.maxPagesPerSite.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.maxPagesPerSite.Location = new System.Drawing.Point(135, 18);
            this.maxPagesPerSite.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.maxPagesPerSite.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.maxPagesPerSite.Name = "maxPagesPerSite";
            this.maxPagesPerSite.Size = new System.Drawing.Size(74, 20);
            this.maxPagesPerSite.TabIndex = 7;
            this.maxPagesPerSite.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(9, 74);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(108, 13);
            this.label10.TabIndex = 4;
            this.label10.Text = "Minimum HTTP delay";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(9, 126);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(100, 13);
            this.label9.TabIndex = 3;
            this.label9.Text = "Maximum site depth";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 100);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(94, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "Maximum redirects";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 48);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(99, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "HTTP timeout (sec)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(120, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Maximum pages per site";
            // 
            // buttonReset
            // 
            this.buttonReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonReset.Location = new System.Drawing.Point(285, 327);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(99, 23);
            this.buttonReset.TabIndex = 14;
            this.buttonReset.Text = "Reset to defaults";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 362);
            this.Controls.Add(this.buttonReset);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(410, 400);
            this.Name = "Settings";
            this.Text = "Application Settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxSiteDepth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxRedirects)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minHttpDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.httpTimeoutSec)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxPagesPerSite)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox extractedAttributes;
        private System.Windows.Forms.TextBox scrappedTags;
        private System.Windows.Forms.TextBox statFilename;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox reportsRoot;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown maxSiteDepth;
        private System.Windows.Forms.NumericUpDown maxRedirects;
        private System.Windows.Forms.NumericUpDown minHttpDelay;
        private System.Windows.Forms.NumericUpDown httpTimeoutSec;
        private System.Windows.Forms.NumericUpDown maxPagesPerSite;
        public System.Windows.Forms.TextBox defaultURL;
        private System.Windows.Forms.Button buttonReset;
    }
}