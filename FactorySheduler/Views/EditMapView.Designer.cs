namespace FactorySheduler.Views
{
    partial class EditMapView
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditMapView));
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.buttonDetectPoints = new System.Windows.Forms.Button();
            this.mapBox = new System.Windows.Forms.PictureBox();
            this.buttonFinish = new System.Windows.Forms.Button();
            this.buttonChooseDevice = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.mapBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.SuspendLayout();
            // 
            // timerRefresh
            // 
            this.timerRefresh.Interval = 300;
            this.timerRefresh.Tick += new System.EventHandler(this.timerRefresh_Tick);
            // 
            // buttonDetectPoints
            // 
            this.buttonDetectPoints.AutoSize = true;
            this.buttonDetectPoints.Location = new System.Drawing.Point(3, 11);
            this.buttonDetectPoints.Name = "buttonDetectPoints";
            this.buttonDetectPoints.Size = new System.Drawing.Size(145, 27);
            this.buttonDetectPoints.TabIndex = 12;
            this.buttonDetectPoints.Text = "Načíst body";
            this.buttonDetectPoints.UseVisualStyleBackColor = true;
            this.buttonDetectPoints.Click += new System.EventHandler(this.buttonDetectPoints_Click);
            // 
            // mapBox
            // 
            this.mapBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mapBox.BackColor = System.Drawing.Color.LightSteelBlue;
            this.mapBox.Cursor = System.Windows.Forms.Cursors.Cross;
            this.mapBox.Location = new System.Drawing.Point(3, 44);
            this.mapBox.Name = "mapBox";
            this.mapBox.Size = new System.Drawing.Size(914, 677);
            this.mapBox.TabIndex = 13;
            this.mapBox.TabStop = false;
            this.mapBox.ClientSizeChanged += new System.EventHandler(this.mapBox_ClientSizeChanged);
            this.mapBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.mapBox_MouseClick);
            this.mapBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mapBox_MouseMove);
            // 
            // buttonFinish
            // 
            this.buttonFinish.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonFinish.AutoSize = true;
            this.buttonFinish.Location = new System.Drawing.Point(772, 11);
            this.buttonFinish.Name = "buttonFinish";
            this.buttonFinish.Size = new System.Drawing.Size(145, 27);
            this.buttonFinish.TabIndex = 14;
            this.buttonFinish.Text = "Ukončit editaci";
            this.buttonFinish.UseVisualStyleBackColor = true;
            this.buttonFinish.Click += new System.EventHandler(this.buttonFinish_Click);
            // 
            // buttonChooseDevice
            // 
            this.buttonChooseDevice.AutoSize = true;
            this.buttonChooseDevice.Location = new System.Drawing.Point(154, 11);
            this.buttonChooseDevice.Name = "buttonChooseDevice";
            this.buttonChooseDevice.Size = new System.Drawing.Size(184, 27);
            this.buttonChooseDevice.TabIndex = 15;
            this.buttonChooseDevice.Text = "Vybrat detekovací zařízení";
            this.buttonChooseDevice.UseVisualStyleBackColor = true;
            this.buttonChooseDevice.Click += new System.EventHandler(this.buttonChooseDevice_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(728, 6);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(38, 32);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 16;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(684, 6);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(38, 32);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 17;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Visible = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox3.Image")));
            this.pictureBox3.Location = new System.Drawing.Point(640, 6);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(38, 32);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox3.TabIndex = 18;
            this.pictureBox3.TabStop = false;
            this.pictureBox3.Visible = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox4.Image")));
            this.pictureBox4.Location = new System.Drawing.Point(596, 6);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(38, 32);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox4.TabIndex = 19;
            this.pictureBox4.TabStop = false;
            this.pictureBox4.Visible = false;
            // 
            // EditMapView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.buttonChooseDevice);
            this.Controls.Add(this.buttonFinish);
            this.Controls.Add(this.mapBox);
            this.Controls.Add(this.buttonDetectPoints);
            this.DoubleBuffered = true;
            this.Name = "EditMapView";
            this.Size = new System.Drawing.Size(920, 724);
            ((System.ComponentModel.ISupportInitialize)(this.mapBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Timer timerRefresh;
        private System.Windows.Forms.Button buttonDetectPoints;
        private System.Windows.Forms.PictureBox mapBox;
        private System.Windows.Forms.Button buttonFinish;
        private System.Windows.Forms.Button buttonChooseDevice;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox4;
    }
}
