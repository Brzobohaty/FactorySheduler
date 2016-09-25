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
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.buttonDetectPoints = new System.Windows.Forms.Button();
            this.mapBox = new System.Windows.Forms.PictureBox();
            this.buttonFinish = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.mapBox)).BeginInit();
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
            this.buttonDetectPoints.Text = "Detekce bodů";
            this.buttonDetectPoints.UseVisualStyleBackColor = true;
            this.buttonDetectPoints.Click += new System.EventHandler(this.buttonDetectPoints_Click);
            // 
            // mapBox
            // 
            this.mapBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mapBox.BackColor = System.Drawing.Color.LightSteelBlue;
            this.mapBox.Location = new System.Drawing.Point(3, 44);
            this.mapBox.Name = "mapBox";
            this.mapBox.Size = new System.Drawing.Size(914, 677);
            this.mapBox.TabIndex = 13;
            this.mapBox.TabStop = false;
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
            // EditMapView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonFinish);
            this.Controls.Add(this.mapBox);
            this.Controls.Add(this.buttonDetectPoints);
            this.Name = "EditMapView";
            this.Size = new System.Drawing.Size(920, 724);
            ((System.ComponentModel.ISupportInitialize)(this.mapBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Timer timerRefresh;
        private System.Windows.Forms.Button buttonDetectPoints;
        private System.Windows.Forms.PictureBox mapBox;
        private System.Windows.Forms.Button buttonFinish;
    }
}
