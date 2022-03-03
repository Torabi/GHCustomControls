using GH_IO.Types;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFNumericUpDown;
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
    public class DegreeSlider : GHParameter
    {

        float _min, _max;
        int _radius;
        float _round=1;
        /// <summary>
        /// number of decimals to display the angle 
        /// </summary>
        int _decimals;
        /// <summary>
        /// Construc a Degree slider with min and max limits
        /// </summary>
        /// <param name="min">minimum value</param>
        /// <param name="max">maximum value</param>
        /// <param name="radius">the radius of the contorl</param>
        /// <param name="defualtValue"></param>
        public DegreeSlider(string name,string description,int radius,float min, float max, float defualtValue, int decimals=0 , Bitmap toolTipDiagram = null) : base(name,description,defualtValue, toolTipDiagram) 
        {
            
            _min = min;
            _max = max;
            _radius = radius;
            _decimals = decimals;
            var square = _hotZoneScale * _hotZoneScale;
            var x =   _radius / (square);
            var y =   (float)Math.Sqrt(square - 1) * _radius / (square);
            _hand = new PointF[]
                {
                    new PointF(0,0),
                    new PointF(x,y),
                    new PointF(_radius,0),
                    new PointF(x,-y),
                    new PointF(0,0)
                };
                
                 
        }

        
        
        
        public override sealed GH_Types DataType => GH_Types.gh_decimal;

        public override sealed  GH_ParamAccess Access =>  GH_ParamAccess.item;
        RectangleF _bounds;
        PointF center;
        RectangleF _hotZone;
        RectangleF _square;
        float _hotZoneScale = 3;
        int _titleHeight = 16;
        PointF[] _hand;
    
        bool isMoving = false;
        public override sealed RectangleF Bounds 
        {
            get => _bounds;
            set 
            {
                _bounds = value;
                center = new PointF(_bounds.Left + _bounds.Width / 2f, _bounds.Top + (_bounds.Height+_titleHeight) / 2f);
                _square = new RectangleF(center.X - _radius, center.Y - _radius, 2*_radius, 2* _radius);
                _hotZone = _square.Offset(2*_radius / _hotZoneScale);
            } 
        }
       

        public override sealed int Offset => 8;

        
        internal override int GetHeight()
        {
            return 2*(_radius+Offset)+_titleHeight;
        }

        internal override int GetWidth()
        {
            return 2 * (_radius + Offset);
        }
        /// <summary>
        /// angle of the point in degree between min and max
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private float getAngle(float x,float y)
        {
            double angle = 0;
            
            if (Math.Abs(x) < 0.0001) // pointer on the Y axis
            {
                if (y > 0.0001) 
                    angle = Math.PI / 2.0;
                else if (y < -0.0001)
                    angle = -Math.PI / 2.0;
                else
                {
                    //click on the center 
                    return 0;
                }
            }
            else
            {
                angle = Math.Atan(y / x); // between -pi/2 to pi/2
                if (y > 0.0001) // on positive y
                {

                    if (angle < 0) // x was negative
                        angle += Math.PI;
                }
                else if (y < -0.0001)
                {
                    if (angle > 0) // x was also negative
                        angle -= Math.PI;
                }
                else
                {
                    if (x > 0)
                        angle = 0;
                    else
                        angle = Math.PI;
                }
            }
            angle *= 180 / Math.PI;
            // this angle is between -180 to 180 , if both min and max are positive then we should change the angle 
            if (_min>=0)
            {
                if (angle < 0)
                    angle += 360;
            }
            return (float)Math.Round(angle/_round)*_round ;
        }

        private string AsString(float angle)
        {
            return angle.ToString($"F{_decimals}");
        }
        /// <summary>
        /// returns true if the given angle in 360 is in given min and max range 
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        private bool inRange(float angle)
        {
            if (_min*_max > 0)
                return angle >= _min && angle <= _max;
            else
                return (angle >= 0 && angle <= _max) || (angle <= 0 && angle >= _min);
        }

        public override void MouseLeave(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {

            //if (Highlighted==0)
            //{
            //    //allowMove = true;
            //    Highlighted = -1;
            //    isMoving = false;
            //result = result | GHMouseEventResult.Invalidated | GHMouseEventResult.Handled;
            //}
            base.MouseLeave(sender, customComponent, e, ref result);
            isMoving = false;
        }

        internal override void MouseLeftClick(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {

            
            // calculate the angle by Atan

            if (_square.Contains(e.CanvasLocation))
            {
                //check if the cursor is in valid area 
                var angle = getAngle(e.CanvasX - center.X, center.Y - e.CanvasY);
               
                if (inRange((float)angle))
                {
                    CurrentValue = (float)angle;
                    //allowMove = false;
                    result = result | GHMouseEventResult.UpdateSolution | GHMouseEventResult.Handled;
                    isMoving = true;
                    return;
                }
                result = result | GHMouseEventResult.Handled;

            }
        }

        public override void MouseOver(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {

            // check if cursor is above thes quare 
            if (_square.Contains(e.CanvasLocation))
            {
                //check if the cursor is in valid area 
                var angle = getAngle(e.CanvasX - center.X, center.Y - e.CanvasY);
                //if (Highlighted != 0)
                //{
                //    Highlighted = 0;

                //}
                if (inRange((float)angle) && isMoving)
                {
                    //_highLightedAngle = (float)angle;
                    CurrentValue = (float)angle;

                    result = result | GHMouseEventResult.UpdateSolution;
                    return;

                }
                result = result | GHMouseEventResult.Invalidated | GHMouseEventResult.Handled;
            }
            else /*if (Highlighted==0)*/
            {
                //Highlighted = -1;
                //_highLightedAngle = (float)CurrentValue;
                result = result | GHMouseEventResult.Invalidated | GHMouseEventResult.Handled;
                //allowMove = true;
            }

        }
        public override RectangleF ActiveZone => _square;
        internal override void MouseRightClick(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
            
            if (_square.Contains(e.CanvasLocation))
            {
                decimal d = Convert.ToDecimal(CurrentValue);
                //NumericUpDownData<decimal> numeric = new NumericUpDownData<decimal>(d, Convert.ToDecimal(_min), Convert.ToDecimal(_max),"");
                DecimalNumber number = new DecimalNumber((decimal)CurrentValue, (decimal)_min, (decimal)_max, 1, _decimals+2);
                //if (numeric.GetInput(PointToScreen(sender, center), out decimal val))
                if (Helpers.GetFloatInput(PointToScreen(sender, center),number))
                {
                    CurrentValue = number.Value;
                    result = result | GHMouseEventResult.UpdateSolution | GHMouseEventResult.Handled;
                }
                else
                {
                    result = result | GHMouseEventResult.Handled;
                }
            }
        }

        public override void MouseKeyUp(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
            if (isMoving)
            {
                isMoving = false;
                result = result| GHMouseEventResult.UpdateSolution|GHMouseEventResult.Handled;
            }
        }
        private void transformPath(GraphicsPath path, float angle)
        {
            
            Matrix rotate = new Matrix();
            rotate.Rotate(-angle);
            Matrix translate = new Matrix();
            translate.Translate(center.X, center.Y);
            path.Transform(rotate); path.Transform(translate);

            rotate.Dispose();translate.Dispose();
        }
        internal override  void Render(Graphics graphics, PointF cursorCanvasPosition, bool selected, bool locked, bool hidden)
        {
            bool Highlighted = _square.Contains(cursorCanvasPosition);
            // preparing the brush 
            GraphicsPath border = new GraphicsPath();
            border.AddEllipse(_square);
            PathGradientBrush pathGradientBrush = new PathGradientBrush(border);
            if (Enabled)
            {
                pathGradientBrush.CenterColor = Color.FromArgb(0, Color.AntiqueWhite);
                pathGradientBrush.SurroundColors = new Color[] { Color.AntiqueWhite };
            }
            else
            {
                pathGradientBrush.CenterColor = Color.FromArgb(0, Color.Gray);
                pathGradientBrush.SurroundColors = new Color[] { Color.Gray };
            }
           
            // draw the frame 
            Helpers.DrawFrame(_bounds, graphics, Name, selected, !Enabled, hidden);
            // draw the valid zone 
            if (Highlighted && Enabled)
            {
                
                
                using (PathGradientBrush pieGradientBrush = new PathGradientBrush(border))
                {
                    pieGradientBrush.CenterColor = Color.FromArgb(5, Color.Azure);
                    pieGradientBrush.SurroundColors = new Color[] { Color.AntiqueWhite };
                    float min = -_min;
                    float max =  -(_max-_min);
                    /*
                    if (min < 0 || max < 0)
                    {
                        min += 180; max += 180;
                    }*/
                    graphics.FillPie
                        (
                        pieGradientBrush,
                        Rectangle.Round(_square),
                        min,max                        
                        );
                    using (Pen p = new Pen(Brushes.DarkGray, 0.5f))
                    {
                        graphics.DrawPie(
                            p,
                            _square,
                            min,max
                            );
                    }
                }
                
            }
            //first render the ticks
           for (int i=0;i<360;i+=15)
            {
                double a = -i * Math.PI / 180;
                var x1 =center.X+ _radius * Math.Cos(a);
                var y1 = center.Y+ _radius * Math.Sin(a);
                float size = 0;
                if (i % 90 == 0)
                {
                    size = _radius / 4;
                }
                else if (i % 45 == 0)
                    size = _radius / 8;
                else
                    size = _radius / 16;
                
                var x2 = center.X+ (_radius-size) * Math.Cos(a);
                var y2 = center.Y + (_radius-size) * Math.Sin(a);
                using (Pen p = new Pen((inRange(i) ? Color.Black : Color.FromArgb(50, Color.Black))))
                {
                    graphics.DrawLine(
                        p,
                        (float)x1, (float)y1, (float)x2, (float)y2
                        );
                }
            }

            // draw the current value
            //GraphicsPath highlighted = new GraphicsPath(_hand, new byte[] { 1, 1, 1, 1, 1 });
            GraphicsPath currentValue = new GraphicsPath(_hand, new byte[] { 1, 1, 1, 1, 1 });
            


           
           
            //if (Highlighted ==0 && Enabled)
            //{
                // draw the current value undeneath 
            transformPath(currentValue, (float)CurrentValue);
            graphics.FillPath(pathGradientBrush, currentValue);
               
            //using (Pen p = new Pen(ActiveBrush()))
            //{
            //    graphics.DrawPath(p, currentValue);
            //}
            if (Enabled)
            {
                graphics.FillPath((Highlighted)?Brushes.LightYellow: pathGradientBrush, currentValue);
            }
                
                
            //}

            //float currentAngle =(Highlighted==0) ? _highLightedAngle: (float)CurrentValue;
            float currentAngle =  (float)CurrentValue;

            //transformPath(highlighted, currentAngle);




            //graphics.FillPath((Highlighted == 0) ? Brushes.LightYellow : pathGradientBrush, highlighted);
            using (Pen p = new Pen(ActiveBrush()))
            {

                graphics.DrawLine(p, currentValue.PathPoints[1], currentValue.PathPoints[2]);
                graphics.DrawLine(p, currentValue.PathPoints[3], currentValue.PathPoints[2]);
            }

            graphics.FillEllipse((Highlighted)?Brushes.LightYellow : pathGradientBrush, _hotZone);
            using (Pen p = new Pen(ActiveBrush(), 2))
            {
                graphics.DrawEllipse(p, _hotZone);
            }
            //highlighted.Dispose();
            currentValue.Dispose() ;   pathGradientBrush.Dispose();
            //draw the texts 
            using (Font f = new Font(SmallFont.FontFamily, 4f, FontStyle.Regular))
            using (StringFormat s = new StringFormat() { Alignment = StringAlignment.Center,LineAlignment = StringAlignment.Center})
            {
                graphics.DrawString(AsString(currentAngle),f,Brushes.Black,_hotZone,s);
            }

        }

        internal override void SetupToolTip(PointF canvasPoint, GH_TooltipDisplayEventArgs e)
        {
            if (_square.Contains(canvasPoint))
            {
                e.Title = "Value";
                e.Text = AsString((float)CurrentValue);
                if (ToolTipDiagram != null)
                    e.Diagram = ToolTipDiagram;
            }
            else
            {
                base.SetupToolTip(canvasPoint,e);
            }
        }
    }
}
