namespace SharpNeatGUI
{
    partial class GenericPlotForm
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
            this.zed.Location = new System.Drawing.Point(0, 0);
            this.zed.Name = "zed";
            this.zed.ScrollGrace = 0;
            this.zed.ScrollMaxX = 0;
            this.zed.ScrollMaxY = 0;
            this.zed.ScrollMaxY2 = 0;
            this.zed.ScrollMinX = 0;
            this.zed.ScrollMinY = 0;
            this.zed.ScrollMinY2 = 0;
            this.zed.Size = new System.Drawing.Size(728, 458);
            this.zed.TabIndex = 0;
            // 
            // SpeciesByComplexityRankForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(728, 458);
            this.Controls.Add(this.zed);
            this.Name = "SpeciesByComplexityRankForm";
            this.Text = "SpeciesByComplexityRankForm";
            this.ResumeLayout(false);

        }

        #endregion

        private ZedGraph.ZedGraphControl zed;
    }
}
