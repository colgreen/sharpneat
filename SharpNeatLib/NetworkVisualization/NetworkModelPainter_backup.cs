using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SharpNeatLib.NetworkVisualization
{
	/// <summary>
	/// Backwards connections (from a lower down neuron to one higer up - as displayed) are drawn differently
	/// and require dus to keep track of used connection points.
	/// </summary>
	class ConnectionPoints
	{
		public int upperLeft=0;
		public int upperRight=0;
		public int lowerLeft=0;
		public int lowerRight=0;
	}

	public class NetworkModelPainter
	{
		const float NEURON_DIAMETER_BASE = 14;

		static Random random = new Random();
		static Font fontNeuronId = new Font("Microsoft Sans Serif", 7.0F);

		
		static Brush brushBlack = new SolidBrush(Color.Black);
		static Brush brushNeuronCore = new SolidBrush(Color.Red);
		static Pen penBlack = new Pen(Color.Black, 1.0F);
		static Pen penConnection = new Pen(Color.Red);

		// Painting parameters. Hold at class level to prevent having to pass around.
		float zoomFactor;
		Point viewportOrigin;
		Size viewportSize;
		double connectionWeightMin;
		double connectionWeightRange;

		// Stuff derived from the painting parameters. Calculated once to prevent having to re-calc.
		Point viewportBound;
		int neuronDiameter;
		int neuronDiameterHalfed;
		float backConnectionLegLength;
		float connectionWidthFactor;

		public void PaintNetwork(	Graphics g, 
									NetworkModel nm,
									float zoomFactor,
									Point viewportOrigin,
									Size viewportSize,
									double connectionWeightMin,
									double connectionWeightRange)
		{
			// Store painting parameters.
			this.zoomFactor = zoomFactor;
			this.viewportOrigin = viewportOrigin;
			this.viewportSize = viewportSize; 
			this.connectionWeightMin = connectionWeightMin;
			this.connectionWeightRange = connectionWeightRange;

			// Some pre-calculated values.
			this.viewportBound = new Point(	viewportOrigin.X+viewportSize.Width,
											viewportOrigin.Y+viewportSize.Height);

			neuronDiameter = (int)(NEURON_DIAMETER_BASE * zoomFactor);
			neuronDiameterHalfed = (int)((NEURON_DIAMETER_BASE * zoomFactor) / 2.0F);
			backConnectionLegLength = (NEURON_DIAMETER_BASE * zoomFactor * 1.6F);
			connectionWidthFactor= (float)(connectionWeightRange/2.0);
			fontNeuronId = new Font("Microsoft Sans Serif", (float)Math.Min(Math.Max(0.2, 7.0F * zoomFactor),Single.MaxValue));

			// Assign a ConnectionPoints object to each neuron.
			int neuronCount = nm.MasterNeuronList.Count;
			for(int neuronIdx=0; neuronIdx<neuronCount; neuronIdx++)
				nm.MasterNeuronList[neuronIdx].AuxPaintingData = new ConnectionPoints();

		//----- Painting code.
			// Paint all connections first. The neurons are painted over the top to 
			// cover up any loose ends.
			for(int neuronIdx=0; neuronIdx<neuronCount; neuronIdx++)
			{
				ModelNeuron neuron = nm.MasterNeuronList[neuronIdx];

				// Incoming connections.
				int connectionCount = neuron.InConnectionList.Count;
				for(int connectionIdx=0; connectionIdx<connectionCount; connectionIdx++)
					PaintConnection(g, neuron.InConnectionList[connectionIdx]);
			}

			foreach(ModelNeuron neuron in nm.MasterNeuronList)
			{
				PaintNeuron(g, neuron);
			}
		}

		private void PaintNeuron(Graphics g, ModelNeuron neuron)
		{
			Point neuronPos = DocToViewport(neuron.Position);
			// Is the neuron within the viewport area?
			if(!IsPointWithinViewport(neuronPos))
			{	// Don't waste time painting this neuron.
				return;
			}

			Point p = new Point(neuronPos.X-neuronDiameterHalfed, neuronPos.Y-neuronDiameterHalfed);
			Size s = new Size(neuronDiameter, neuronDiameter);
			Rectangle r = new Rectangle(p,s);

			//g.FillEllipse(brushNeuron, r);
			g.FillRectangle(brushNeuronCore, r);
			g.DrawRectangle(penBlack, r);

			// Draw the neuron ID.
			neuronPos.X += neuronDiameterHalfed+1;
			neuronPos.Y -= neuronDiameterHalfed/2;
			g.DrawString(neuron.Id.ToString(), fontNeuronId, brushBlack, neuronPos);
		}

		private void PaintConnection(Graphics g, ModelConnection con)
		{
			Point srcPos = DocToViewport(con.SourceNeuron.Position);
			Point tgtPos = DocToViewport(con.TargetNeuron.Position);

			// Connections leave from the base of the neuron and enter the top.
			srcPos.Y += (int)(neuronDiameterHalfed*0.9);	//  to hide line ends behind neurons.
			tgtPos.Y -= (int)(neuronDiameterHalfed*0.9);

			// Is any part of the connection within the viewport area?
			if(!IsPointWithinViewport(srcPos) && !IsPointWithinViewport(tgtPos))
			{	// Don't waste time painting this connection.
				return;
			}

		//----- Modify a pen for this connection.
			// Color and width based on connection weight. 
			penConnection.Width = (float)Math.Max(1.0, (Math.Abs(con.Weight)/connectionWidthFactor)*3.0*zoomFactor);

			// Color hue. 80% gives rangle of red through to blue.
			float temp = (float)((con.Weight-connectionWeightMin)/connectionWeightRange);
			penConnection.Color = ColorUtilities.FromBlueRedScale(temp);
									// ColorUtilities.FromHls(hue, 1.0F, 0.5F);

		//----- Draw the line.
			if(tgtPos.Y <= srcPos.Y)
			{	// Target is behind source. Draw a back-connection.
				PaintBackConnection(g, penConnection, 
										srcPos, tgtPos, 
										(ConnectionPoints)con.SourceNeuron.AuxPaintingData,
										(ConnectionPoints)con.TargetNeuron.AuxPaintingData);
			}
			else
			{	// Target is ahead of the source. Draw a straight line.
				g.DrawLine(penConnection, srcPos, tgtPos);
			}
		}

		private void PaintBackConnection(	Graphics g,
											Pen pen,
											Point srcPos, 
											Point tgtPos,
											ConnectionPoints srcConData,
											ConnectionPoints tgtConData)
		{
			// Line is described by srcPos,a,b,c,d,tgtPos
			Point a,b,c,d;
			float gradient;
			int length;
			int xDelta, yDelta;

		//----- Point a.
			if(tgtPos.X<=srcPos.X)
			{
				gradient = (float)Math.Min(1.0, 0.2F + (srcConData.lowerLeft*0.23F));
				length = (int)(backConnectionLegLength + (backConnectionLegLength*srcConData.lowerLeft*0.05));
				xDelta = -(int)(length * (1.0F - gradient));
				yDelta = (int)(length * gradient);
			}
			else
			{
				gradient = (float)Math.Min(1.0, 0.2F + (srcConData.lowerRight*0.23F));
				length = (int)(backConnectionLegLength + (backConnectionLegLength*srcConData.lowerRight*0.05));
				xDelta = (int)(length * (1.0F - gradient));
				yDelta = (int)(length * gradient);
			}
			a = new Point(srcPos.X+xDelta, srcPos.Y+yDelta);

		//----- Point b.
			if(tgtPos.X<=srcPos.X)
			{
				length = (int)(backConnectionLegLength/4.0F + (backConnectionLegLength*srcConData.lowerLeft*0.6));
				b = new Point(a.X-length,a.Y);
				srcConData.lowerLeft++;
			}
			else
			{
				length = (int)(backConnectionLegLength/4.0F + (backConnectionLegLength*srcConData.lowerRight*0.6));
				b = new Point(a.X+length,a.Y);
				srcConData.lowerRight++;
			}

		//----- Point d.
			if(srcPos.X>tgtPos.X)
			{
				gradient = (float)Math.Min(1.0, 0.2F + (tgtConData.upperRight*0.23F));
				length = (int)(backConnectionLegLength + (backConnectionLegLength*srcConData.upperRight*0.05));
				xDelta = (int)(length * (1.0F - gradient));
				yDelta = -(int)(length * gradient);
			}
			else
			{
				gradient = (float)Math.Min(1.0, 0.2F + (tgtConData.upperLeft*0.23F));
				length = (int)(backConnectionLegLength + (backConnectionLegLength*srcConData.upperLeft*0.05));
				xDelta = -(int)(length * (1.0F - gradient));
				yDelta = -(int)(length * gradient);
			}
			d = new Point(tgtPos.X+xDelta, tgtPos.Y+yDelta);

		//----- Point c.
			if(srcPos.X>tgtPos.X)
			{
				length = (int)(backConnectionLegLength/4.0F + (backConnectionLegLength*tgtConData.upperRight*0.6));
				c = new Point(d.X+length,d.Y);
				tgtConData.upperRight++;
			}
			else
			{
				length = (int)(backConnectionLegLength/4.0F + (backConnectionLegLength*srcConData.upperLeft*0.6));
				c = new Point(d.X-length,d.Y);
				tgtConData.upperLeft++;
			}



		//----- Paint the lines.
			g.DrawLines(pen, new Point[]{srcPos,a,b,c,d,tgtPos});

//			g.DrawLine(pen, srcPos, a);
//			g.DrawLine(pen, a, b);
//			g.DrawLine(pen, b, c);
//			g.DrawLine(pen, d, c);
//			g.DrawLine(pen, tgtPos, d);
		}

		private bool IsPointWithinViewport(Point p)
		{
			return	(p.X>=0) && (p.Y>=0) &&
					(p.X<viewportSize.Width) && (p.Y<viewportSize.Height);
		}

		#region Coordinate Systems Converstion

		private Point DocToViewport(Point p)
		{
			p.X = (int)((float)p.X * zoomFactor) - viewportOrigin.X;
			p.Y = (int)((float)p.Y * zoomFactor) - viewportOrigin.Y;
			return p;
		}

//		private Point DocToAbs(Point p)
//		{
//			p.X = (int)((float)p.X * zoomFactor);
//			p.Y = (int)((float)p.Y * zoomFactor);
//			return p;
//		}
//
//		private Point AbsToViewport(Point p)
//		{
//			p.X -= viewportOrigin.X;
//			p.Y -= viewportOrigin.Y;
//			return p;
//		}


		#endregion
	}
}
