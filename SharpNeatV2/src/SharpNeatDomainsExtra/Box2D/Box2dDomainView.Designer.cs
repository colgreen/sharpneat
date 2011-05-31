namespace SharpNeat.DomainsExtra.Box2D
{
    abstract partial class Box2dDomainView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblMouseWorldCoords = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.openGlControl = new Tao.Platform.Windows.SimpleOpenGlControl();
            this.SuspendLayout();
            // 
            // lblMouseWorldCoords
            // 
            this.lblMouseWorldCoords.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMouseWorldCoords.BackColor = System.Drawing.Color.Transparent;
            this.lblMouseWorldCoords.Font = new System.Drawing.Font("Microsoft Sans Serif",9.75F,System.Drawing.FontStyle.Regular,System.Drawing.GraphicsUnit.Point,((byte)(0)));
            this.lblMouseWorldCoords.ForeColor = System.Drawing.Color.Black;
            this.lblMouseWorldCoords.Location = new System.Drawing.Point(782,0);
            this.lblMouseWorldCoords.Name = "lblMouseWorldCoords";
            this.lblMouseWorldCoords.Size = new System.Drawing.Size(90,20);
            this.lblMouseWorldCoords.TabIndex = 6;
            this.lblMouseWorldCoords.Text = "[0.00]  [0.00]";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0,2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(287,13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Mouse click on an object to drag it around the box2d world.";
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
            this.openGlControl.Location = new System.Drawing.Point(0,19);
            this.openGlControl.Name = "openGlControl";
            this.openGlControl.Size = new System.Drawing.Size(872,648);
            this.openGlControl.StencilBits = ((byte)(0));
            this.openGlControl.TabIndex = 8;
            this.openGlControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.openGlControl_MouseDown);
            this.openGlControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.openGlControl_MouseMove);
            this.openGlControl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.openGlControl_MouseUp);
            this.openGlControl.Resize += new System.EventHandler(this.openGlControl_Resize);
            // 
            // Box2dDomainView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F,13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.openGlControl);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblMouseWorldCoords);
            this.Name = "Box2dDomainView";
            this.Size = new System.Drawing.Size(872,667);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMouseWorldCoords;
        private System.Windows.Forms.Label label1;
        private Tao.Platform.Windows.SimpleOpenGlControl openGlControl;
    }
}
