/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2016 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
/******************************************************************************/
/*
  Box2DX Copyright (c) 2008 Ihar Kalasouski http://code.google.com/p/box2dx
  Box2D original C++ version Copyright (c) 2006-2007 Erin Catto http://www.gphysics.com

  This software is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this software.

  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this software must not be misrepresented; you must not
     claim that you wrote the original software. If you use this software
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original software.
  3. This notice may not be removed or altered from any source distribution.
*/

using Box2DX.Common;
using Box2DX.Dynamics;
using Tao.OpenGl;
using SysMath = System.Math;

namespace SharpNeat.DomainsExtra.Box2D
{
    /// <summary>
    /// Box2DX debug drawing class that redirects drawing calls to Open GL.
    /// </summary>
    public class OpenGLDebugDraw : DebugDraw
    {
		/// <summary>
		/// Draw a closed polygon provided in CCW order.
		/// </summary>
		public override void DrawPolygon(Vec2[] vertices, int vertexCount, Color color)
		{
			Gl.glColor3f(color.R, color.G, color.B);
			Gl.glBegin(Gl.GL_LINE_LOOP);
			for (int i = 0; i < vertexCount; ++i)
			{
				Gl.glVertex2f(vertices[i].X, vertices[i].Y);
			}
			Gl.glEnd();			
		}

		/// <summary>
		/// Draw a solid closed polygon provided in CCW order.
		/// </summary>
		public override void DrawSolidPolygon(Vec2[] vertices, int vertexCount, Color color)
		{
			Gl.glEnable(Gl.GL_BLEND);
			Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
			Gl.glColor4f(0.5f * color.R, 0.5f * color.G, 0.5f * color.B, 0.5f);
			Gl.glBegin(Gl.GL_TRIANGLE_FAN);
			for (int i = 0; i < vertexCount; ++i)
			{
				Gl.glVertex2f(vertices[i].X, vertices[i].Y);
			}
			Gl.glEnd();
			Gl.glDisable(Gl.GL_BLEND);

			Gl.glColor4f(color.R, color.G, color.B, 1.0f);
			Gl.glBegin(Gl.GL_LINE_LOOP);
			for (int i = 0; i < vertexCount; ++i)
			{
				Gl.glVertex2f(vertices[i].X, vertices[i].Y);
			}
			Gl.glEnd();
		}

		/// <summary>
		/// Draw a circle.
		/// </summary>
		public override void DrawCircle(Vec2 center, float radius, Color color)
		{
			float k_segments = 16.0f;
			float k_increment = 2.0f * Box2DX.Common.Settings.Pi / k_segments;
			float theta = 0.0f;
			Gl.glColor3f(color.R, color.G, color.B);
			Gl.glBegin(Gl.GL_LINE_LOOP);
			for (int i = 0; i < k_segments; ++i)
			{
				Vec2 v = center + radius * new Vec2((float)SysMath.Cos(theta), (float)SysMath.Sin(theta));
				Gl.glVertex2f(v.X, v.Y);
				theta += k_increment;
			}
			Gl.glEnd();
		}

		/// <summary>
		/// Draw a solid circle.
		/// </summary>
		public override void DrawSolidCircle(Vec2 center, float radius, Vec2 axis, Color color)
		{
			float k_segments = 16.0f;
			float k_increment = 2.0f * Box2DX.Common.Settings.Pi / k_segments;
			float theta = 0.0f;
			Gl.glEnable(Gl.GL_BLEND);
			Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
			Gl.glColor4f(0.5f * color.R, 0.5f * color.G, 0.5f * color.B, 0.5f);
			Gl.glBegin(Gl.GL_TRIANGLE_FAN);
			for (int i = 0; i < k_segments; ++i)
			{
				Vec2 v = center + radius * new Vec2((float)SysMath.Cos(theta), (float)SysMath.Sin(theta));
				Gl.glVertex2f(v.X, v.Y);
				theta += k_increment;
			}
			Gl.glEnd();
			Gl.glDisable(Gl.GL_BLEND);

			theta = 0.0f;
			Gl.glColor4f(color.R, color.G, color.B, 1.0f);
			Gl.glBegin(Gl.GL_LINE_LOOP);
			for (int i = 0; i < k_segments; ++i)
			{
				Vec2 v = center + radius * new Vec2((float)SysMath.Cos(theta), (float)SysMath.Sin(theta));
				Gl.glVertex2f(v.X, v.Y);
				theta += k_increment;
			}
			Gl.glEnd();

			Vec2 p = center + radius * axis;
			Gl.glBegin(Gl.GL_LINES);
			Gl.glVertex2f(center.X, center.Y);
			Gl.glVertex2f(p.X, p.Y);
			Gl.glEnd();
		}

		/// <summary>
		/// Draw a line segment.
		/// </summary>
		public override void DrawSegment(Vec2 p1, Vec2 p2, Color color)
		{
			Gl.glColor3f(color.R, color.G, color.B);
			Gl.glBegin(Gl.GL_LINES);
			Gl.glVertex2f(p1.X, p1.Y);
			Gl.glVertex2f(p2.X, p2.Y);
			Gl.glEnd();
		}

		/// <summary>
		/// Draw a transform. Choose your own length scale.
		/// </summary>
		/// <param name="xf">A transform.</param>
		public override void DrawXForm(XForm xf)
		{
			Vec2 p1 = xf.Position, p2;
			float k_axisScale = 0.4f;
			Gl.glBegin(Gl.GL_LINES);

			Gl.glColor3f(1.0f, 0.0f, 0.0f);
			Gl.glVertex2f(p1.X, p1.Y);
			p2 = p1 + k_axisScale * xf.R.Col1;
			Gl.glVertex2f(p2.X, p2.Y);

			Gl.glColor3f(0.0f, 1.0f, 0.0f);
			Gl.glVertex2f(p1.X, p1.Y);
			p2 = p1 + k_axisScale * xf.R.Col2;
			Gl.glVertex2f(p2.X, p2.Y);

			Gl.glEnd();
		}
    }
}
