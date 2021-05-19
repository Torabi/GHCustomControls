using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
Original work Copyright (c) 2021 Ali Torabi (ali@parametriczoo.com)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.
*/
namespace GHCustomControls
{
    public static class Helpers
    {
        public static void DropShadow(Graphics G, Color c, Color backColor, GraphicsPath GP, int d)
        {
            Color[] colors = getColorVector(c, backColor, d).ToArray();
            var t = G.Transform;
            for (int i = 0; i < d; i++)
            {
                G.TranslateTransform(1f, 0.75f);                // <== shadow vector!
                using (Pen pen = new Pen(colors[i], 1.75f))  // <== pen width (*)
                    G.DrawPath(pen, GP);
            }
            G.Transform = t;
        }
        public static void InShadow(Graphics G, Color c, Color backColor, GraphicsPath GP, int d)
        {
            Color[] colors = getColorVector(c, backColor, d).ToArray();
            RectangleF rec = GP.GetBounds();
            for (int i = 0; i < d; i++)
            {
                G.TranslateTransform(1f, 1f);                // <== shadow vector!


                G.ScaleTransform(1f - 2f / rec.Width, 1f - 2f / rec.Height);
                using (Pen pen = new Pen(colors[i], 1.75f))  // <== pen width (*)
                    G.DrawPath(pen, GP);
            }
            G.ResetTransform();
        }



        public static List<Color> getColorVector(Color fc, Color bc, int depth)
        {
            List<Color> cv = new List<Color>();
            float dRed = 1f * (bc.R - fc.R) / depth;
            float dGreen = 1f * (bc.G - fc.G) / depth;
            float dBlue = 1f * (bc.B - fc.B) / depth;
            for (int d = 1; d <= depth; d++)
                cv.Add(Color.FromArgb(255, (int)(fc.R + dRed * d),
                  (int)(fc.G + dGreen * d), (int)(fc.B + dBlue * d)));
            return cv;
        }

        public static GraphicsPath getRectPath(RectangleF R)
        {
            byte[] fm = new byte[3];
            for (int b = 0; b < 3; b++) fm[b] = 1;
            List<PointF> points = new List<PointF>();
            points.Add(new PointF(R.Left, R.Bottom));
            points.Add(new PointF(R.Right, R.Bottom));
            points.Add(new PointF(R.Right, R.Top));
            return new GraphicsPath(points.ToArray(), fm);
        }
        /// <summary>
        /// Create new rectangeF by offseting this rectangle inside.(negative value offsets the rectangle outside)
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static RectangleF Offset(this RectangleF rectangle,float f)
        {
            return new RectangleF(rectangle.X + f, rectangle.Y + f, rectangle.Width - 2 * f, rectangle.Height - 2 * f);
        }
        /// <summary>
        /// Create a new rectangleF in given size at the center of this rectangle
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static RectangleF Center(this RectangleF rectangle, Size size)
        {
            return new RectangleF(
                rectangle.Left + (rectangle.Width - size.Width) / 2f,
                rectangle.Top + (rectangle.Height - size.Height) / 2f,
                size.Width,
                size.Height);
        }

        
        

        public static void DrawFrame(RectangleF bound, Graphics G,string title, bool selected,bool locked,bool hidden)
        {
            GH_Capsule capsule = GH_Capsule.CreateCapsule(bound, GH_Palette.Transparent, 2, 0);

            capsule.Render(G, selected, locked, hidden);
            if (title == string.Empty)
            {
                capsule.Dispose();
                return;
            }
            using (StringFormat s = new StringFormat()
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Near,
                Trimming = StringTrimming.EllipsisCharacter
            })
                
            {
                G.DrawString(
                   title,
                   GHControl.SmallFont,
                   (locked)?Brushes.Gray: Brushes.Black,
                   capsule.Box.Left + capsule.MaxRadius,
                   capsule.Box.Top + capsule.MaxRadius,
                   s);

            }
            if (title != string.Empty)
            {
                using (Pen p = new Pen(Brushes.DimGray, 0.5f))
                {
                    G.DrawLine(
                        p,
                        capsule.Box.Left + 3,
                        capsule.Box.Top + GHControl.SmallFont.Height + 1,
                        capsule.Box.Right - 3,
                        capsule.Box.Top + GHControl.SmallFont.Height + 1);
                }
            }

            capsule.Dispose();
        }

      


        

        
             
    }
}
