using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace SharpNeatLib.NetworkVisualization
{
	public class NetworkControl : System.Windows.Forms.UserControl
	{
		const int VIEWPORT_MARGIN_WIDTH = 40;

		Viewport viewport;
		NetworkModel networkModel;
		float zoomFactor=1.0F;

		#region Component Designer variables

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion

		#region Constructors / Disposal

		public NetworkControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

		//----- Create and attach the viewport.
			viewport = new Viewport();
			// By default we match the size of the containing control.
			viewport.Size = new Size(Width, Height);
			viewport.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			viewport.DoubleClick += new System.EventHandler(this.viewport_DoubleClick);
			this.Controls.Add(viewport);
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// NetworkControl
			// 
			this.AutoScroll = true;
			this.AutoScrollMargin = new System.Drawing.Size(1, 1);
			this.Name = "NetworkControl";
			this.Size = new System.Drawing.Size(352, 336);
			this.Resize += new System.EventHandler(this.NetworkControl_Resize);
		}
		#endregion

		#region Properties

		public NetworkModel NetworkModel
		{
			get
			{
				return viewport.NetworkModel;
			}
			set
			{
				networkModel = value;
				UpdateViewportSize();
				viewport.NetworkModel = value;
			}
		}

		public Size ViewportSize
		{
			get
			{
				return viewport.Size;
			}
		}

		public float ZoomFactor
		{
			get
			{
				return zoomFactor;
			}
			set
			{	
				zoomFactor = Math.Max(1.0F, value);
				UpdateViewportSize();
				viewport.ZoomFactor = zoomFactor;
			}
		}

		#endregion

		#region Public Methods

		public void RefreshControl()
		{
			viewport.RefreshViewport();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Set the viewport size to be at least the size of this control. If the
		/// network model contains model elements outside this area then the viewport
		/// expands to accomodate them - it's a scrollable control.
		/// </summary>
		private void UpdateViewportSize()
		{
			Size tmp;
			if(networkModel!=null)
			{
				tmp = new Size(	(int)Math.Max(networkModel.Bounds.Width*zoomFactor+VIEWPORT_MARGIN_WIDTH*zoomFactor, Width), 
								(int)Math.Max(networkModel.Bounds.Height*zoomFactor+VIEWPORT_MARGIN_WIDTH*zoomFactor, Height));
			}
			else
			{
				tmp = new Size( Width, Height);
			}

			viewport.Size = tmp;
		}

		#endregion

		#region Event Handlers

		private void NetworkControl_Resize(object sender, System.EventArgs e)
		{
			UpdateViewportSize();
		}

		private void viewport_DoubleClick(object sender, System.EventArgs e)
		{
			OnDoubleClick(e);
		}

		#endregion

	}
}
