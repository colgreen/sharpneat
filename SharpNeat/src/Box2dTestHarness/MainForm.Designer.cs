namespace SharpNeat.Box2dTestHarness
{
    partial class MainForm
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
            if(disposing && (components != null))
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.openGlControl = new Tao.Platform.Windows.SimpleOpenGlControl();
            this.label1 = new System.Windows.Forms.Label();
            this.simTimer = new System.Windows.Forms.Timer(this.components);
            this.cmbSimulationWorlds = new System.Windows.Forms.ComboBox();
            this.lblMouseWorldCoords = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // openGlControl
            // 
            this.openGlControl.AccumBits = ((byte)(0));
            this.openGlControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.openGlControl.AutoCheckErrors = false;
            this.openGlControl.AutoFinish = false;
            this.openGlControl.AutoMakeCurrent = true;
            this.openGlControl.AutoSwapBuffers = true;
            this.openGlControl.BackColor = System.Drawing.Color.Black;
            this.openGlControl.ColorBits = ((byte)(32));
            this.openGlControl.DepthBits = ((byte)(16));
            this.openGlControl.Location = new System.Drawing.Point(0,22);
            this.openGlControl.Name = "openGlControl";
            this.openGlControl.Size = new System.Drawing.Size(804,573);
            this.openGlControl.StencilBits = ((byte)(0));
            this.openGlControl.TabIndex = 1;
            this.openGlControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.openGlControl_MouseDown);
            this.openGlControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.openGlControl_MouseMove);
            this.openGlControl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.openGlControl_MouseUp);
            this.openGlControl.Resize += new System.EventHandler(this.openGlControl_Resize);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0,6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(287,13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Mouse click on an object to drag it around the box2d world.";
            // 
            // simTimer
            // 
            this.simTimer.Interval = 16;
            this.simTimer.Tick += new System.EventHandler(this.simTimer_Tick);
            // 
            // cmbSimulationWorlds
            // 
            this.cmbSimulationWorlds.FormattingEnabled = true;
            this.cmbSimulationWorlds.Location = new System.Drawing.Point(305,0);
            this.cmbSimulationWorlds.Name = "cmbSimulationWorlds";
            this.cmbSimulationWorlds.Size = new System.Drawing.Size(220,21);
            this.cmbSimulationWorlds.TabIndex = 4;
            this.cmbSimulationWorlds.SelectedIndexChanged += new System.EventHandler(this.cmbSimulationWorlds_SelectedIndexChanged);
            // 
            // lblMouseWorldCoords
            // 
            this.lblMouseWorldCoords.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMouseWorldCoords.BackColor = System.Drawing.Color.Transparent;
            this.lblMouseWorldCoords.Font = new System.Drawing.Font("Microsoft Sans Serif",9.75F,System.Drawing.FontStyle.Regular,System.Drawing.GraphicsUnit.Point,((byte)(0)));
            this.lblMouseWorldCoords.ForeColor = System.Drawing.Color.Black;
            this.lblMouseWorldCoords.Location = new System.Drawing.Point(714,1);
            this.lblMouseWorldCoords.Name = "lblMouseWorldCoords";
            this.lblMouseWorldCoords.Size = new System.Drawing.Size(90,20);
            this.lblMouseWorldCoords.TabIndex = 5;
            this.lblMouseWorldCoords.Text = "[0.00]  [0.00]";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F,13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804,595);
            this.Controls.Add(this.lblMouseWorldCoords);
            this.Controls.Add(this.cmbSimulationWorlds);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.openGlControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(637,344);
            this.Name = "MainForm";
            this.Text = "SharpNEAT Box2D Test Harness";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Tao.Platform.Windows.SimpleOpenGlControl openGlControl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer simTimer;
        private System.Windows.Forms.ComboBox cmbSimulationWorlds;
        private System.Windows.Forms.Label lblMouseWorldCoords;
    }
}

