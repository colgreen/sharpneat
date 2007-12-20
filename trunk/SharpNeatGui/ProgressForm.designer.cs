namespace SharpNeat
{
	partial class ProgressForm
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.fitnessGraph = new ZedGraph.ZedGraphControl();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.complexityGraph = new ZedGraph.ZedGraphControl();
            this.evalsPerSecGraph = new ZedGraph.ZedGraphControl();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.speciesGraph = new ZedGraph.ZedGraphControl();
            this.compatGraph = new ZedGraph.ZedGraphControl();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 522);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(548, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.fitnessGraph);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(548, 522);
            this.splitContainer1.SplitterDistance = 274;
            this.splitContainer1.TabIndex = 1;
            // 
            // fitnessGraph
            // 
            this.fitnessGraph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fitnessGraph.IsAutoScrollRange = false;
            this.fitnessGraph.IsEnableHPan = true;
            this.fitnessGraph.IsEnableHZoom = true;
            this.fitnessGraph.IsEnableVPan = true;
            this.fitnessGraph.IsEnableVZoom = true;
            this.fitnessGraph.IsPrintFillPage = true;
            this.fitnessGraph.IsPrintKeepAspectRatio = true;
            this.fitnessGraph.IsScrollY2 = false;
            this.fitnessGraph.IsShowContextMenu = true;
            this.fitnessGraph.IsShowCopyMessage = true;
            this.fitnessGraph.IsShowCursorValues = false;
            this.fitnessGraph.IsShowHScrollBar = false;
            this.fitnessGraph.IsShowPointValues = false;
            this.fitnessGraph.IsShowVScrollBar = false;
            this.fitnessGraph.IsZoomOnMouseCenter = false;
            this.fitnessGraph.Location = new System.Drawing.Point(0, 0);
            this.fitnessGraph.Name = "fitnessGraph";
            this.fitnessGraph.PanButtons = System.Windows.Forms.MouseButtons.Left;
            this.fitnessGraph.PanButtons2 = System.Windows.Forms.MouseButtons.Middle;
            this.fitnessGraph.PanModifierKeys2 = System.Windows.Forms.Keys.None;
            this.fitnessGraph.PointDateFormat = "g";
            this.fitnessGraph.PointValueFormat = "G";
            this.fitnessGraph.ScrollMaxX = 0;
            this.fitnessGraph.ScrollMaxY = 0;
            this.fitnessGraph.ScrollMaxY2 = 0;
            this.fitnessGraph.ScrollMinX = 0;
            this.fitnessGraph.ScrollMinY = 0;
            this.fitnessGraph.ScrollMinY2 = 0;
            this.fitnessGraph.Size = new System.Drawing.Size(548, 274);
            this.fitnessGraph.TabIndex = 0;
            this.fitnessGraph.ZoomButtons = System.Windows.Forms.MouseButtons.Left;
            this.fitnessGraph.ZoomButtons2 = System.Windows.Forms.MouseButtons.None;
            this.fitnessGraph.ZoomModifierKeys = System.Windows.Forms.Keys.None;
            this.fitnessGraph.ZoomModifierKeys2 = System.Windows.Forms.Keys.None;
            this.fitnessGraph.ZoomStepFraction = 0.1;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer4);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer2.Size = new System.Drawing.Size(548, 244);
            this.splitContainer2.SplitterDistance = 270;
            this.splitContainer2.TabIndex = 0;
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.Location = new System.Drawing.Point(0, 0);
            this.splitContainer4.Name = "splitContainer4";
            this.splitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.complexityGraph);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.evalsPerSecGraph);
            this.splitContainer4.Size = new System.Drawing.Size(270, 244);
            this.splitContainer4.SplitterDistance = 122;
            this.splitContainer4.TabIndex = 0;
            // 
            // complexityGraph
            // 
            this.complexityGraph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.complexityGraph.IsAutoScrollRange = false;
            this.complexityGraph.IsEnableHPan = true;
            this.complexityGraph.IsEnableHZoom = true;
            this.complexityGraph.IsEnableVPan = true;
            this.complexityGraph.IsEnableVZoom = true;
            this.complexityGraph.IsPrintFillPage = true;
            this.complexityGraph.IsPrintKeepAspectRatio = true;
            this.complexityGraph.IsScrollY2 = false;
            this.complexityGraph.IsShowContextMenu = true;
            this.complexityGraph.IsShowCopyMessage = true;
            this.complexityGraph.IsShowCursorValues = false;
            this.complexityGraph.IsShowHScrollBar = false;
            this.complexityGraph.IsShowPointValues = false;
            this.complexityGraph.IsShowVScrollBar = false;
            this.complexityGraph.IsZoomOnMouseCenter = false;
            this.complexityGraph.Location = new System.Drawing.Point(0, 0);
            this.complexityGraph.Name = "complexityGraph";
            this.complexityGraph.PanButtons = System.Windows.Forms.MouseButtons.Left;
            this.complexityGraph.PanButtons2 = System.Windows.Forms.MouseButtons.Middle;
            this.complexityGraph.PanModifierKeys2 = System.Windows.Forms.Keys.None;
            this.complexityGraph.PointDateFormat = "g";
            this.complexityGraph.PointValueFormat = "G";
            this.complexityGraph.ScrollMaxX = 0;
            this.complexityGraph.ScrollMaxY = 0;
            this.complexityGraph.ScrollMaxY2 = 0;
            this.complexityGraph.ScrollMinX = 0;
            this.complexityGraph.ScrollMinY = 0;
            this.complexityGraph.ScrollMinY2 = 0;
            this.complexityGraph.Size = new System.Drawing.Size(270, 122);
            this.complexityGraph.TabIndex = 0;
            this.complexityGraph.ZoomButtons = System.Windows.Forms.MouseButtons.Left;
            this.complexityGraph.ZoomButtons2 = System.Windows.Forms.MouseButtons.None;
            this.complexityGraph.ZoomModifierKeys = System.Windows.Forms.Keys.None;
            this.complexityGraph.ZoomModifierKeys2 = System.Windows.Forms.Keys.None;
            this.complexityGraph.ZoomStepFraction = 0.1;
            // 
            // evalsPerSecGraph
            // 
            this.evalsPerSecGraph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.evalsPerSecGraph.IsAutoScrollRange = false;
            this.evalsPerSecGraph.IsEnableHPan = true;
            this.evalsPerSecGraph.IsEnableHZoom = true;
            this.evalsPerSecGraph.IsEnableVPan = true;
            this.evalsPerSecGraph.IsEnableVZoom = true;
            this.evalsPerSecGraph.IsPrintFillPage = true;
            this.evalsPerSecGraph.IsPrintKeepAspectRatio = true;
            this.evalsPerSecGraph.IsScrollY2 = false;
            this.evalsPerSecGraph.IsShowContextMenu = true;
            this.evalsPerSecGraph.IsShowCopyMessage = true;
            this.evalsPerSecGraph.IsShowCursorValues = false;
            this.evalsPerSecGraph.IsShowHScrollBar = false;
            this.evalsPerSecGraph.IsShowPointValues = false;
            this.evalsPerSecGraph.IsShowVScrollBar = false;
            this.evalsPerSecGraph.IsZoomOnMouseCenter = false;
            this.evalsPerSecGraph.Location = new System.Drawing.Point(0, 0);
            this.evalsPerSecGraph.Name = "evalsPerSecGraph";
            this.evalsPerSecGraph.PanButtons = System.Windows.Forms.MouseButtons.Left;
            this.evalsPerSecGraph.PanButtons2 = System.Windows.Forms.MouseButtons.Middle;
            this.evalsPerSecGraph.PanModifierKeys2 = System.Windows.Forms.Keys.None;
            this.evalsPerSecGraph.PointDateFormat = "g";
            this.evalsPerSecGraph.PointValueFormat = "G";
            this.evalsPerSecGraph.ScrollMaxX = 0;
            this.evalsPerSecGraph.ScrollMaxY = 0;
            this.evalsPerSecGraph.ScrollMaxY2 = 0;
            this.evalsPerSecGraph.ScrollMinX = 0;
            this.evalsPerSecGraph.ScrollMinY = 0;
            this.evalsPerSecGraph.ScrollMinY2 = 0;
            this.evalsPerSecGraph.Size = new System.Drawing.Size(270, 118);
            this.evalsPerSecGraph.TabIndex = 0;
            this.evalsPerSecGraph.ZoomButtons = System.Windows.Forms.MouseButtons.Left;
            this.evalsPerSecGraph.ZoomButtons2 = System.Windows.Forms.MouseButtons.None;
            this.evalsPerSecGraph.ZoomModifierKeys = System.Windows.Forms.Keys.None;
            this.evalsPerSecGraph.ZoomModifierKeys2 = System.Windows.Forms.Keys.None;
            this.evalsPerSecGraph.ZoomStepFraction = 0.1;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.speciesGraph);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.compatGraph);
            this.splitContainer3.Size = new System.Drawing.Size(274, 244);
            this.splitContainer3.SplitterDistance = 122;
            this.splitContainer3.TabIndex = 0;
            // 
            // speciesGraph
            // 
            this.speciesGraph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.speciesGraph.IsAutoScrollRange = false;
            this.speciesGraph.IsEnableHPan = true;
            this.speciesGraph.IsEnableHZoom = true;
            this.speciesGraph.IsEnableVPan = true;
            this.speciesGraph.IsEnableVZoom = true;
            this.speciesGraph.IsPrintFillPage = true;
            this.speciesGraph.IsPrintKeepAspectRatio = true;
            this.speciesGraph.IsScrollY2 = false;
            this.speciesGraph.IsShowContextMenu = true;
            this.speciesGraph.IsShowCopyMessage = true;
            this.speciesGraph.IsShowCursorValues = false;
            this.speciesGraph.IsShowHScrollBar = false;
            this.speciesGraph.IsShowPointValues = false;
            this.speciesGraph.IsShowVScrollBar = false;
            this.speciesGraph.IsZoomOnMouseCenter = false;
            this.speciesGraph.Location = new System.Drawing.Point(0, 0);
            this.speciesGraph.Name = "speciesGraph";
            this.speciesGraph.PanButtons = System.Windows.Forms.MouseButtons.Left;
            this.speciesGraph.PanButtons2 = System.Windows.Forms.MouseButtons.Middle;
            this.speciesGraph.PanModifierKeys2 = System.Windows.Forms.Keys.None;
            this.speciesGraph.PointDateFormat = "g";
            this.speciesGraph.PointValueFormat = "G";
            this.speciesGraph.ScrollMaxX = 0;
            this.speciesGraph.ScrollMaxY = 0;
            this.speciesGraph.ScrollMaxY2 = 0;
            this.speciesGraph.ScrollMinX = 0;
            this.speciesGraph.ScrollMinY = 0;
            this.speciesGraph.ScrollMinY2 = 0;
            this.speciesGraph.Size = new System.Drawing.Size(274, 122);
            this.speciesGraph.TabIndex = 0;
            this.speciesGraph.ZoomButtons = System.Windows.Forms.MouseButtons.Left;
            this.speciesGraph.ZoomButtons2 = System.Windows.Forms.MouseButtons.None;
            this.speciesGraph.ZoomModifierKeys = System.Windows.Forms.Keys.None;
            this.speciesGraph.ZoomModifierKeys2 = System.Windows.Forms.Keys.None;
            this.speciesGraph.ZoomStepFraction = 0.1;
            // 
            // compatGraph
            // 
            this.compatGraph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.compatGraph.IsAutoScrollRange = false;
            this.compatGraph.IsEnableHPan = true;
            this.compatGraph.IsEnableHZoom = true;
            this.compatGraph.IsEnableVPan = true;
            this.compatGraph.IsEnableVZoom = true;
            this.compatGraph.IsPrintFillPage = true;
            this.compatGraph.IsPrintKeepAspectRatio = true;
            this.compatGraph.IsScrollY2 = false;
            this.compatGraph.IsShowContextMenu = true;
            this.compatGraph.IsShowCopyMessage = true;
            this.compatGraph.IsShowCursorValues = false;
            this.compatGraph.IsShowHScrollBar = false;
            this.compatGraph.IsShowPointValues = false;
            this.compatGraph.IsShowVScrollBar = false;
            this.compatGraph.IsZoomOnMouseCenter = false;
            this.compatGraph.Location = new System.Drawing.Point(0, 0);
            this.compatGraph.Name = "compatGraph";
            this.compatGraph.PanButtons = System.Windows.Forms.MouseButtons.Left;
            this.compatGraph.PanButtons2 = System.Windows.Forms.MouseButtons.Middle;
            this.compatGraph.PanModifierKeys2 = System.Windows.Forms.Keys.None;
            this.compatGraph.PointDateFormat = "g";
            this.compatGraph.PointValueFormat = "G";
            this.compatGraph.ScrollMaxX = 0;
            this.compatGraph.ScrollMaxY = 0;
            this.compatGraph.ScrollMaxY2 = 0;
            this.compatGraph.ScrollMinX = 0;
            this.compatGraph.ScrollMinY = 0;
            this.compatGraph.ScrollMinY2 = 0;
            this.compatGraph.Size = new System.Drawing.Size(274, 118);
            this.compatGraph.TabIndex = 0;
            this.compatGraph.ZoomButtons = System.Windows.Forms.MouseButtons.Left;
            this.compatGraph.ZoomButtons2 = System.Windows.Forms.MouseButtons.None;
            this.compatGraph.ZoomModifierKeys = System.Windows.Forms.Keys.None;
            this.compatGraph.ZoomModifierKeys2 = System.Windows.Forms.Keys.None;
            this.compatGraph.ZoomStepFraction = 0.1;
            // 
            // ProgressForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(548, 544);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Name = "ProgressForm";
            this.Text = "Progress Graphs";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProgressForm_FormClosing);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            this.splitContainer4.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private ZedGraph.ZedGraphControl fitnessGraph;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.SplitContainer splitContainer3;
		private ZedGraph.ZedGraphControl speciesGraph;
		private ZedGraph.ZedGraphControl compatGraph;
		private System.Windows.Forms.SplitContainer splitContainer4;
		private ZedGraph.ZedGraphControl complexityGraph;
		private ZedGraph.ZedGraphControl evalsPerSecGraph;

	}
}