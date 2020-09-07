namespace SharpNeat.Windows.App
{
    partial class GenomeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GenomeForm));
            this.genomeCtrl = new SharpNeat.Windows.GenomeControl();
            this.SuspendLayout();
            // 
            // genomeCtrl
            // 
            this.genomeCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.genomeCtrl.Genome = null;
            this.genomeCtrl.Location = new System.Drawing.Point(0, 0);
            this.genomeCtrl.Name = "genomeCtrl";
            this.genomeCtrl.Size = new System.Drawing.Size(397, 366);
            this.genomeCtrl.TabIndex = 0;
            // 
            // GenomeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(397, 366);
            this.Controls.Add(this.genomeCtrl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "GenomeForm";
            this.Text = "GenericForm";
            this.ResumeLayout(false);

        }

        #endregion

        private GenomeControl genomeCtrl;
    }
}