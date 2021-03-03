namespace AddUsersToGroupSpeedChecker
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btnUserByAcctMgmt = new System.Windows.Forms.Button();
            this.lblTimeByAcctMgmt = new System.Windows.Forms.Label();
            this.btnAddUserByAdServices = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblTimeByAdServices = new System.Windows.Forms.Label();
            this.bgWorker = new System.ComponentModel.BackgroundWorker();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnUserByAcctMgmt
            // 
            this.btnUserByAcctMgmt.Location = new System.Drawing.Point(12, 12);
            this.btnUserByAcctMgmt.Name = "btnUserByAcctMgmt";
            this.btnUserByAcctMgmt.Size = new System.Drawing.Size(220, 23);
            this.btnUserByAcctMgmt.TabIndex = 0;
            this.btnUserByAcctMgmt.Text = "Add user - By AccountManagement";
            this.btnUserByAcctMgmt.UseVisualStyleBackColor = true;
            this.btnUserByAcctMgmt.Click += new System.EventHandler(this.btnUserByAcctMgmt_Click);
            // 
            // lblTimeByAcctMgmt
            // 
            this.lblTimeByAcctMgmt.AutoSize = true;
            this.lblTimeByAcctMgmt.Location = new System.Drawing.Point(17, 44);
            this.lblTimeByAcctMgmt.Name = "lblTimeByAcctMgmt";
            this.lblTimeByAcctMgmt.Size = new System.Drawing.Size(60, 15);
            this.lblTimeByAcctMgmt.TabIndex = 1;
            this.lblTimeByAcctMgmt.Text = "Time here";
            this.lblTimeByAcctMgmt.Visible = false;
            // 
            // btnAddUserByAdServices
            // 
            this.btnAddUserByAdServices.Location = new System.Drawing.Point(290, 12);
            this.btnAddUserByAdServices.Name = "btnAddUserByAdServices";
            this.btnAddUserByAdServices.Size = new System.Drawing.Size(220, 23);
            this.btnAddUserByAdServices.TabIndex = 2;
            this.btnAddUserByAdServices.Text = "Add user - By AD Services";
            this.btnAddUserByAdServices.UseVisualStyleBackColor = true;
            this.btnAddUserByAdServices.Click += new System.EventHandler(this.btnAddUserByAdServices_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 82);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(523, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(26, 17);
            this.lblStatus.Text = "Idle";
            // 
            // lblTimeByAdServices
            // 
            this.lblTimeByAdServices.AutoSize = true;
            this.lblTimeByAdServices.Location = new System.Drawing.Point(287, 44);
            this.lblTimeByAdServices.Name = "lblTimeByAdServices";
            this.lblTimeByAdServices.Size = new System.Drawing.Size(60, 15);
            this.lblTimeByAdServices.TabIndex = 4;
            this.lblTimeByAdServices.Text = "Time here";
            this.lblTimeByAdServices.Visible = false;
            // 
            // bgWorker
            // 
            this.bgWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorker_DoWork);
            this.bgWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgWorker_RunWorkerCompleted);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(523, 104);
            this.Controls.Add(this.lblTimeByAdServices);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnAddUserByAdServices);
            this.Controls.Add(this.lblTimeByAcctMgmt);
            this.Controls.Add(this.btnUserByAcctMgmt);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(539, 142);
            this.MinimumSize = new System.Drawing.Size(539, 142);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add users to Group - Speed Checker";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnUserByAcctMgmt;
        private System.Windows.Forms.Label lblTimeByAcctMgmt;
        private System.Windows.Forms.Button btnAddUserByAdServices;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.Label lblTimeByAdServices;
        private System.ComponentModel.BackgroundWorker bgWorker;
    }
}

