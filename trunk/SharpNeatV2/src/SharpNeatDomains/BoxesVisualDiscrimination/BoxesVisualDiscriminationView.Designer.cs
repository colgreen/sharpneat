namespace SharpNeat.Domains
{
    partial class BoxesVisualDiscriminationView
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
            this.btnNextTestCase = new System.Windows.Forms.Button();
            this.cbxResolution = new System.Windows.Forms.ComboBox();
            this.lblResolution = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
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
            this.pbx.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbx_MouseMove);
            // 
            // btnNextTestCase
            // 
            this.btnNextTestCase.Location = new System.Drawing.Point(112, 2);
            this.btnNextTestCase.Name = "btnNextTestCase";
            this.btnNextTestCase.Size = new System.Drawing.Size(110, 23);
            this.btnNextTestCase.TabIndex = 2;
            this.btnNextTestCase.Text = "Random Test Case";
            this.btnNextTestCase.UseVisualStyleBackColor = true;
            this.btnNextTestCase.Click += new System.EventHandler(this.btnNextTestCase_Click);
            // 
            // cbxResolution
            // 
            this.cbxResolution.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxResolution.FormattingEnabled = true;
            this.cbxResolution.Items.AddRange(new object[] {
            "11",
            "22",
            "33",
            "44",
            "55"});
            this.cbxResolution.Location = new System.Drawing.Point(61, 3);
            this.cbxResolution.Name = "cbxResolution";
            this.cbxResolution.Size = new System.Drawing.Size(45, 21);
            this.cbxResolution.TabIndex = 3;
            this.cbxResolution.SelectedIndexChanged += new System.EventHandler(this.cbxResolution_SelectedIndexChanged);
            // 
            // lblResolution
            // 
            this.lblResolution.AutoSize = true;
            this.lblResolution.BackColor = System.Drawing.Color.Transparent;
            this.lblResolution.Location = new System.Drawing.Point(3, 6);
            this.lblResolution.Name = "lblResolution";
            this.lblResolution.Size = new System.Drawing.Size(57, 13);
            this.lblResolution.TabIndex = 4;
            this.lblResolution.Text = "Resolution";
            this.lblResolution.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(3, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(211, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Mouse moves boxes (left=large, right=small)";
            // 
            // BoxesVisualDiscriminationView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblResolution);
            this.Controls.Add(this.cbxResolution);
            this.Controls.Add(this.btnNextTestCase);
            this.Controls.Add(this.pbx);
            this.Controls.Add(this.graphControl1);
            this.Name = "BoxesVisualDiscriminationView";
            this.Size = new System.Drawing.Size(462, 216);
            ((System.ComponentModel.ISupportInitialize)(this.pbx)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private View.GraphControl graphControl1;
        private System.Windows.Forms.PictureBox pbx;
        private System.Windows.Forms.Button btnNextTestCase;
        private System.Windows.Forms.ComboBox cbxResolution;
        private System.Windows.Forms.Label lblResolution;
        private System.Windows.Forms.Label label1;
    }
}
