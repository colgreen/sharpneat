namespace ActivationFunctionViewer
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
            this.components = new System.ComponentModel.Container();
            this.zed = new ZedGraph.ZedGraphControl();
            this.SuspendLayout();
            // 
            // zed
            // 
            this.zed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.zed.IsAntiAlias = true;
            this.zed.Location = new System.Drawing.Point(0, 0);
            this.zed.Name = "zed";
            this.zed.ScrollGrace = 0D;
            this.zed.ScrollMaxX = 0D;
            this.zed.ScrollMaxY = 0D;
            this.zed.ScrollMaxY2 = 0D;
            this.zed.ScrollMinX = 0D;
            this.zed.ScrollMinY = 0D;
            this.zed.ScrollMinY2 = 0D;
            this.zed.Size = new System.Drawing.Size(713, 591);
            this.zed.TabIndex = 2;
            this.zed.UseExtendedPrintDialog = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(713, 591);
            this.Controls.Add(this.zed);
            this.Name = "Form1";
            this.Text = "Activation Function Viewer";
            this.ResumeLayout(false);

        }

        #endregion

        private ZedGraph.ZedGraphControl zed;
    }
}

