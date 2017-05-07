namespace FactorySheduler.Views
{
    partial class MapView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapView));
            this.labelDvicesIPs = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonRotateLeft = new System.Windows.Forms.Button();
            this.buttonBack = new System.Windows.Forms.Button();
            this.buttonRotateRight = new System.Windows.Forms.Button();
            this.buttonLeft = new System.Windows.Forms.Button();
            this.buttonSelectPath = new System.Windows.Forms.Button();
            this.buttonFront = new System.Windows.Forms.Button();
            this.buttonReinicializeCart = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonRight = new System.Windows.Forms.Button();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.buttonsLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.mapBox = new System.Windows.Forms.PictureBox();
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.buttonSearchNextDevices = new System.Windows.Forms.Button();
            this.buttonReinicializeStaticBeacons = new System.Windows.Forms.Button();
            this.buttonEditMap = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mapBox)).BeginInit();
            this.SuspendLayout();
            // 
            // labelDvicesIPs
            // 
            this.labelDvicesIPs.Location = new System.Drawing.Point(13, 21);
            this.labelDvicesIPs.Name = "labelDvicesIPs";
            this.labelDvicesIPs.Size = new System.Drawing.Size(506, 20);
            this.labelDvicesIPs.TabIndex = 5;
            this.labelDvicesIPs.Text = "Mapa nalezených zařízení";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Controls.Add(this.propertyGrid);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(220, 547);
            this.panel1.TabIndex = 9;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.Controls.Add(this.buttonRotateLeft, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.buttonBack, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.buttonRotateRight, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.buttonLeft, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.buttonSelectPath, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonFront, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonReinicializeCart, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonStop, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.buttonRight, 2, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 417);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(220, 130);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // buttonRotateLeft
            // 
            this.buttonRotateLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonRotateLeft.Image = ((System.Drawing.Image)(resources.GetObject("buttonRotateLeft.Image")));
            this.buttonRotateLeft.Location = new System.Drawing.Point(3, 89);
            this.buttonRotateLeft.Name = "buttonRotateLeft";
            this.buttonRotateLeft.Size = new System.Drawing.Size(67, 38);
            this.buttonRotateLeft.TabIndex = 6;
            this.buttonRotateLeft.UseVisualStyleBackColor = true;
            this.buttonRotateLeft.Click += new System.EventHandler(this.buttonRotateLeft_Click);
            // 
            // buttonBack
            // 
            this.buttonBack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonBack.Image = ((System.Drawing.Image)(resources.GetObject("buttonBack.Image")));
            this.buttonBack.Location = new System.Drawing.Point(76, 89);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(67, 38);
            this.buttonBack.TabIndex = 5;
            this.buttonBack.UseVisualStyleBackColor = true;
            this.buttonBack.Click += new System.EventHandler(this.buttonBack_Click);
            // 
            // buttonRotateRight
            // 
            this.buttonRotateRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonRotateRight.Image = ((System.Drawing.Image)(resources.GetObject("buttonRotateRight.Image")));
            this.buttonRotateRight.Location = new System.Drawing.Point(149, 89);
            this.buttonRotateRight.Name = "buttonRotateRight";
            this.buttonRotateRight.Size = new System.Drawing.Size(68, 38);
            this.buttonRotateRight.TabIndex = 4;
            this.buttonRotateRight.UseVisualStyleBackColor = true;
            this.buttonRotateRight.Click += new System.EventHandler(this.buttonRotateRight_Click);
            // 
            // buttonLeft
            // 
            this.buttonLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonLeft.Image = ((System.Drawing.Image)(resources.GetObject("buttonLeft.Image")));
            this.buttonLeft.Location = new System.Drawing.Point(3, 46);
            this.buttonLeft.Name = "buttonLeft";
            this.buttonLeft.Size = new System.Drawing.Size(67, 37);
            this.buttonLeft.TabIndex = 3;
            this.buttonLeft.UseVisualStyleBackColor = true;
            this.buttonLeft.Click += new System.EventHandler(this.buttonLeft_Click);
            // 
            // buttonSelectPath
            // 
            this.buttonSelectPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonSelectPath.Location = new System.Drawing.Point(149, 3);
            this.buttonSelectPath.Name = "buttonSelectPath";
            this.buttonSelectPath.Size = new System.Drawing.Size(68, 37);
            this.buttonSelectPath.TabIndex = 2;
            this.buttonSelectPath.Text = "Vybrat cíl";
            this.buttonSelectPath.UseVisualStyleBackColor = true;
            this.buttonSelectPath.Click += new System.EventHandler(this.buttonSelectPath_Click);
            // 
            // buttonFront
            // 
            this.buttonFront.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonFront.Image = ((System.Drawing.Image)(resources.GetObject("buttonFront.Image")));
            this.buttonFront.Location = new System.Drawing.Point(76, 3);
            this.buttonFront.Name = "buttonFront";
            this.buttonFront.Size = new System.Drawing.Size(67, 37);
            this.buttonFront.TabIndex = 1;
            this.buttonFront.UseVisualStyleBackColor = true;
            this.buttonFront.Click += new System.EventHandler(this.buttonFront_Click);
            // 
            // buttonReinicializeCart
            // 
            this.buttonReinicializeCart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonReinicializeCart.Location = new System.Drawing.Point(3, 3);
            this.buttonReinicializeCart.Name = "buttonReinicializeCart";
            this.buttonReinicializeCart.Size = new System.Drawing.Size(67, 37);
            this.buttonReinicializeCart.TabIndex = 0;
            this.buttonReinicializeCart.Text = "Reinicializovat";
            this.buttonReinicializeCart.UseVisualStyleBackColor = true;
            this.buttonReinicializeCart.Click += new System.EventHandler(this.buttonReinicializeCart_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonStop.Image = ((System.Drawing.Image)(resources.GetObject("buttonStop.Image")));
            this.buttonStop.Location = new System.Drawing.Point(76, 46);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(67, 37);
            this.buttonStop.TabIndex = 7;
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonRight
            // 
            this.buttonRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonRight.Image = ((System.Drawing.Image)(resources.GetObject("buttonRight.Image")));
            this.buttonRight.Location = new System.Drawing.Point(149, 46);
            this.buttonRight.Name = "buttonRight";
            this.buttonRight.Size = new System.Drawing.Size(68, 37);
            this.buttonRight.TabIndex = 8;
            this.buttonRight.UseVisualStyleBackColor = true;
            this.buttonRight.Click += new System.EventHandler(this.buttonRight_Click);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid.Margin = new System.Windows.Forms.Padding(3, 3, 3, 50);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.propertyGrid.Size = new System.Drawing.Size(220, 410);
            this.propertyGrid.TabIndex = 0;
            this.propertyGrid.ToolbarVisible = false;
            this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
            // 
            // buttonsLayout
            // 
            this.buttonsLayout.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonsLayout.AutoScroll = true;
            this.buttonsLayout.Location = new System.Drawing.Point(16, 597);
            this.buttonsLayout.Name = "buttonsLayout";
            this.buttonsLayout.Size = new System.Drawing.Size(888, 124);
            this.buttonsLayout.TabIndex = 10;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(16, 44);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.splitContainer1.Panel1.Controls.Add(this.mapBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Size = new System.Drawing.Size(888, 547);
            this.splitContainer1.SplitterDistance = 664;
            this.splitContainer1.TabIndex = 11;
            // 
            // mapBox
            // 
            this.mapBox.BackColor = System.Drawing.Color.LightSteelBlue;
            this.mapBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapBox.Location = new System.Drawing.Point(0, 0);
            this.mapBox.Name = "mapBox";
            this.mapBox.Size = new System.Drawing.Size(664, 547);
            this.mapBox.TabIndex = 0;
            this.mapBox.TabStop = false;
            this.mapBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.mapBox_MouseClick);
            // 
            // timerRefresh
            // 
            this.timerRefresh.Interval = 300;
            this.timerRefresh.Tick += new System.EventHandler(this.timerRefresh_Tick);
            // 
            // buttonSearchNextDevices
            // 
            this.buttonSearchNextDevices.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSearchNextDevices.AutoSize = true;
            this.buttonSearchNextDevices.Location = new System.Drawing.Point(759, 11);
            this.buttonSearchNextDevices.Name = "buttonSearchNextDevices";
            this.buttonSearchNextDevices.Size = new System.Drawing.Size(145, 27);
            this.buttonSearchNextDevices.TabIndex = 12;
            this.buttonSearchNextDevices.Text = "Hledat další zařízení";
            this.buttonSearchNextDevices.UseVisualStyleBackColor = true;
            this.buttonSearchNextDevices.Click += new System.EventHandler(this.buttonSearchNextDevices_Click);
            // 
            // buttonReinicializeStaticBeacons
            // 
            this.buttonReinicializeStaticBeacons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonReinicializeStaticBeacons.AutoSize = true;
            this.buttonReinicializeStaticBeacons.Location = new System.Drawing.Point(535, 11);
            this.buttonReinicializeStaticBeacons.Name = "buttonReinicializeStaticBeacons";
            this.buttonReinicializeStaticBeacons.Size = new System.Drawing.Size(219, 27);
            this.buttonReinicializeStaticBeacons.TabIndex = 13;
            this.buttonReinicializeStaticBeacons.Text = "Reinicializace statickcýh majáků";
            this.buttonReinicializeStaticBeacons.UseVisualStyleBackColor = true;
            this.buttonReinicializeStaticBeacons.Click += new System.EventHandler(this.buttonReinicializeStaticBeacons_Click);
            // 
            // buttonEditMap
            // 
            this.buttonEditMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonEditMap.AutoSize = true;
            this.buttonEditMap.Location = new System.Drawing.Point(310, 11);
            this.buttonEditMap.Name = "buttonEditMap";
            this.buttonEditMap.Size = new System.Drawing.Size(219, 27);
            this.buttonEditMap.TabIndex = 14;
            this.buttonEditMap.Text = "Návrh mapy";
            this.buttonEditMap.UseVisualStyleBackColor = true;
            this.buttonEditMap.Click += new System.EventHandler(this.buttonEditMap_Click);
            // 
            // MapView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonEditMap);
            this.Controls.Add(this.buttonReinicializeStaticBeacons);
            this.Controls.Add(this.buttonSearchNextDevices);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.buttonsLayout);
            this.Controls.Add(this.labelDvicesIPs);
            this.Name = "MapView";
            this.Size = new System.Drawing.Size(920, 724);
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mapBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label labelDvicesIPs;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.FlowLayoutPanel buttonsLayout;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Timer timerRefresh;
        private System.Windows.Forms.Button buttonSearchNextDevices;
        private System.Windows.Forms.PictureBox mapBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button buttonReinicializeCart;
        private System.Windows.Forms.Button buttonRotateLeft;
        private System.Windows.Forms.Button buttonBack;
        private System.Windows.Forms.Button buttonRotateRight;
        private System.Windows.Forms.Button buttonLeft;
        private System.Windows.Forms.Button buttonSelectPath;
        private System.Windows.Forms.Button buttonFront;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonRight;
        private System.Windows.Forms.Button buttonReinicializeStaticBeacons;
        private System.Windows.Forms.Button buttonEditMap;
    }
}
