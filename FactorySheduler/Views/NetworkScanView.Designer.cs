namespace FactorySheduler.Views
{
    partial class NetworkScanView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.labelThisDeviceIP = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.labelDvicesIPs = new System.Windows.Forms.Label();
            this.labelCount = new System.Windows.Forms.Label();
            this.buttonNext = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP tohoto zařízení:";
            // 
            // labelThisDeviceIP
            // 
            this.labelThisDeviceIP.AutoSize = true;
            this.labelThisDeviceIP.Location = new System.Drawing.Point(159, 21);
            this.labelThisDeviceIP.Name = "labelThisDeviceIP";
            this.labelThisDeviceIP.Size = new System.Drawing.Size(0, 17);
            this.labelThisDeviceIP.TabIndex = 1;
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Location = new System.Drawing.Point(35, 88);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(848, 547);
            this.listView1.TabIndex = 2;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.List;
            // 
            // labelDvicesIPs
            // 
            this.labelDvicesIPs.Location = new System.Drawing.Point(32, 65);
            this.labelDvicesIPs.Name = "labelDvicesIPs";
            this.labelDvicesIPs.Size = new System.Drawing.Size(506, 20);
            this.labelDvicesIPs.TabIndex = 0;
            this.labelDvicesIPs.Text = "IP nalezených zařízení v síti s prefixem ";
            // 
            // labelCount
            // 
            this.labelCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelCount.AutoSize = true;
            this.labelCount.Location = new System.Drawing.Point(32, 647);
            this.labelCount.Name = "labelCount";
            this.labelCount.Size = new System.Drawing.Size(0, 17);
            this.labelCount.TabIndex = 3;
            // 
            // buttonNext
            // 
            this.buttonNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonNext.Enabled = false;
            this.buttonNext.Location = new System.Drawing.Point(766, 641);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(117, 28);
            this.buttonNext.TabIndex = 4;
            this.buttonNext.Text = "Pokračovat";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // NetworkScanView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonNext);
            this.Controls.Add(this.labelCount);
            this.Controls.Add(this.labelDvicesIPs);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.labelThisDeviceIP);
            this.Controls.Add(this.label1);
            this.Name = "NetworkScanView";
            this.Size = new System.Drawing.Size(920, 724);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelThisDeviceIP;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Label labelDvicesIPs;
        private System.Windows.Forms.Label labelCount;
        private System.Windows.Forms.Button buttonNext;
    }
}
