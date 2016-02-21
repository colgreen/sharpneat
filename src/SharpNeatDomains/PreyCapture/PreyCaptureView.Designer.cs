namespace SharpNeat.Domains.PreyCapture
{
    partial class PreyCaptureView
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
            this.graphControl1 = new SharpNeat.View.GraphControl();
            this.pbx = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbx)).BeginInit();
            this.SuspendLayout();
            // 
            // graphControl1
            // 
            this.graphControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graphControl1.Location = new System.Drawing.Point(0, 0);
            this.graphControl1.Name = "graphControl1";
            this.graphControl1.Size = new System.Drawing.Size(462, 216);
            this.graphControl1.TabIndex = 0;
            this.graphControl1.ViewportPainter = null;
            // 
            // pbx
            // 
            this.pbx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbx.Location = new System.Drawing.Point(0, 0);
            this.pbx.Name = "pbx";
            this.pbx.Size = new System.Drawing.Size(462, 216);
            this.pbx.TabIndex = 1;
            this.pbx.TabStop = false;
            this.pbx.SizeChanged += new System.EventHandler(this.pbx_SizeChanged);
            // 
            // PreyCaptureView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pbx);
            this.Controls.Add(this.graphControl1);
            this.Name = "PreyCaptureView";
            this.Size = new System.Drawing.Size(462, 216);
            ((System.ComponentModel.ISupportInitialize)(this.pbx)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private View.GraphControl graphControl1;
        private System.Windows.Forms.PictureBox pbx;
    }
}
