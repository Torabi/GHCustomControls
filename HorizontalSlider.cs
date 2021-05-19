using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
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
    public abstract class HorizontalSlider : GHParameter
    {
 
        internal int _fontSize ;
        internal float _min;
        internal float _max;
        bool _showTitle;

        internal string FormatString;
        internal string Suffix;
        /// <summary>
        /// the rectangle host the slider grip
        /// </summary>
        internal RectangleF _sliderRec;
        /// <summary>
        /// the horizontal bar
        /// </summary>
        internal RectangleF _sliderBar;
        bool isMoving;
        internal float X;
        /// <summary>
        /// if true then it shows the number above slider
        /// </summary>
        internal bool showLabel=true;

        internal PointF Pos => new PointF((_sliderRec.Right + _sliderRec.Left) / 2, _sliderRec.Top+2);
 
        public  int MinimumWidth
        {
            get
            {
                // calculate the width of the string for the left limit
                int left = GH_FontServer.StringWidth(GetValueAsString(0), SmallFont);
                int right = GH_FontServer.StringWidth(GetValueAsString(1), SmallFont);
                return (left + right) * 2;
            }

        }
        /// <summary>
        /// return the formated string for the given float number between 0 to 1
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public abstract string GetValueAsString(float x);
        /// <summary>
        /// update the current value based on the current position of the slider (X)
        /// </summary>
        public abstract void UpdateValue();
        /// <summary>
        /// updates the current position (X) based on the current value (CurrentValue) 
        /// X must be between Offset and Bounds.Width-Offset
        /// </summary>
        /// <returns></returns>
        public abstract void UpdatePos();
        
        /// <summary>
        /// return the X curresponding to the given value (between min and max)
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        internal float GetPos(float val)
        {
            return ((val - _min) / (_max - _min)) * (Bounds.Width - 2 * Offset) + Offset;
        }
        //internal float GetPos(double val)
        //{
        //    return (((float)val - _min) / (_max - _min)) * (Bounds.Width - 2 * Offset) + Offset;
        //}
        RectangleF _bounds;

        StringFormat _format;

        internal float _radius=6;
        protected HorizontalSlider(string name,string description, object value, float min,float max,bool showTitle, string formatString,string suffix, Bitmap toolTipDiagram = null) :base(name,description,value,toolTipDiagram)
        {
           
            _format = new StringFormat();
            _format.Alignment = StringAlignment.Near;
            _format.LineAlignment = StringAlignment.Near;
            _format.Trimming = StringTrimming.EllipsisCharacter;
            _min = min;
            _max = max;
            FormatString = formatString;
            Suffix = suffix;
            _showTitle = showTitle;
            _fontSize = GH_FontServer.MeasureString(Name, StandardFont).Height;
        }
        
        
        
        /// <summary>
        /// return the value between 0 and 1
        /// </summary>
        /// <returns></returns>
        internal float GetNormalizedValue(float x)
        {
            return (x - Offset) / (Bounds.Width - 2 * Offset);
        }
        /// <summary>
        /// return float number represting the current value 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        internal float GetCurrentValue()
        {
            return _min + GetNormalizedValue(X) * (_max - _min);
        }
        public override RectangleF Bounds { 
            get =>_bounds; 
            set
            {
                _bounds = value;
                
            } 
        }

        public override int Offset => 9;

    

        internal override int GetHeight()
        {
            return _fontSize+3*Offset/2+ (_showTitle?_fontSize:0);
        }

        internal override int GetWidth()
        {
            if (_showTitle)
            {
                
                return Math.Max( MinimumWidth, GH_FontServer.StringWidth(Name, SmallFont)+Offset);
            }
            else
            {
                return MinimumWidth;
            }
        }

        #region Mouse events

        internal override void MouseLeftClick(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
           
            if (_sliderRec.Contains(e.CanvasLocation))
            {
                
                    result = result | GHMouseEventResult.UpdateSolution | GHMouseEventResult.Handled;
                
                    isMoving = true;
                // clicked on the slider , we start dragging 
                
                
            }
            else
            {
                // click on the bar , move the cursor to the point 
                X = e.CanvasLocation.X-Bounds.X;
                if (X < Offset)
                    X = Offset;
                if (X > _bounds.Width - Offset)
                    X = Bounds.Width - Offset;
                result = result | GHMouseEventResult.UpdateSolution| GHMouseEventResult.Handled;
                UpdateValue();
       

            }
        }
        internal override void MouseRightClick(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
            showLabel = false;
            if (!_sliderRec.Contains(e.CanvasLocation))
            {
                result = result | GHMouseEventResult.Handled;
                return;
            }
            Refresh(); 
            result = result | GHMouseEventResult.Invalidated;

            
        }
        internal override void MouseLeave(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
            //check if the slider is in moving state
            base.MouseLeave(sender, customComponent, e, ref result);
            if (isMoving)
            {
                // if the pointer is on the left side 
                if (e.CanvasX<Bounds.Left+Offset)
                {
                    X = Offset;

                }
                else if (e.CanvasX > Bounds.Right - Offset)
                {

                    X = Bounds.Width - Offset;
                }
                result = result | GHMouseEventResult.Handled | GHMouseEventResult.UpdateSolution;
                UpdateValue();
           
                isMoving = false;

            }
            //if (Highlighted==0)
            //{
            //    Highlighted = -1;
                //result = result | GHMouseEventResult.Invalidated;
            //}
        }

        internal override void MouseOver(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {

            base.MouseOver(sender,customComponent,e,ref result);
            result = result | GHMouseEventResult.Highlighted;
            if ( !isMoving)
                return;
            if(isMoving)
            {
                if (e.CanvasX < Bounds.Left + Offset)
                {
                    X = Offset;

                }
                else if (e.CanvasX > Bounds.Right - Offset)
                {

                    X = Bounds.Width - Offset;
                }
                else
                {
                    X = e.CanvasX - Bounds.Left;
                }
                UpdateValue();
                result = result | GHMouseEventResult.Invalidated | GHMouseEventResult.UpdateSolution;

            }
            //if (Highlighted == -1)
            //{
            //    Highlighted = 0;
            //    result = result | GHMouseEventResult.Invalidated;
            //}
            result = result | GHMouseEventResult.Handled;
           
        }

        internal override void MouseKeyUp(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
            base.MouseKeyUp( sender, customComponent, e, ref result);
            if (isMoving)
            {
                isMoving = false;
                result = result | GHMouseEventResult.Handled | GHMouseEventResult.UpdateSolution;
            }

        }
        internal override  void SetupToolTip(PointF canvasPoint, GH_TooltipDisplayEventArgs e)
        {
             
            if (_sliderRec.Contains(canvasPoint))
            {
                e.Title = "Value";
                e.Text = GetValueAsString(GetNormalizedValue(X));
                
            }
            else
            {
                base.SetupToolTip(canvasPoint,e);
            }
        }


        #endregion

        internal override void Render(Graphics graphics, PointF cursorCanvasPosition, bool selected, bool locked, bool hidden)
        {
            // create capsule using bound
            //RectangleF rec = new RectangleF(Bounds.X + Offset / 3f, Bounds.Y + Offset / 3f, Bounds.Width - 2 * Offset / 3f, Bounds.Height - 2 * Offset / 3f);
            /*
            GH_Capsule capsule = GH_Capsule.CreateCapsule(Bounds, GH_Palette.Transparent,2,0);

            capsule.Render(graphics, selected, locked, hidden);
             
            
            capsule.Dispose();
            // render the name 
 
            graphics.DrawString(Name, SmallFont, Brushes.Black, Bounds.X + Offset / 2.0f, Bounds.Y + Offset / 2.0f, _format);
            graphics.DrawLine(new Pen(Brushes.DimGray, 0.5f), Bounds.X + Offset, Bounds.Y + _fontSize, Bounds.Right - Offset, Bounds.Y + _fontSize);
            */

            bool Highlighted = Bounds.Contains(cursorCanvasPosition);
            // render the frame 
            Helpers.DrawFrame(Bounds, graphics,_showTitle? Name:"", selected, !Enabled, hidden);
            // render the min and max 
            string min = GetValueAsString(0);
            string max = GetValueAsString(1);
            graphics.DrawString(min, SmallFont, this.ActiveBrush(), Bounds.X + Offset , Bounds.Y +(_showTitle? _fontSize:0), _format);
            graphics.DrawString(max, SmallFont, this.ActiveBrush(), Bounds.Right -GH_FontServer.StringWidth(max, SmallFont)-Offset, Bounds.Y + (_showTitle? _fontSize:0), _format);
            // render the bar
            float y = Bounds.Bottom-Offset/2f;
            _sliderBar = new RectangleF(Bounds.X + Offset,y-1.5f , Bounds.Width - 2 * Offset , 3f);
            graphics.FillRectangle(Brushes.DimGray, _sliderBar);
            using (Pen p = new Pen(Brushes.DimGray, 0.5f))
            {
                graphics.DrawLine(p, _sliderBar.Left, _sliderBar.Bottom, _sliderBar.Left, _sliderBar.Bottom - _fontSize-Offset/2f);
                graphics.DrawLine(p, _sliderBar.Right, _sliderBar.Bottom, _sliderBar.Right, _sliderBar.Bottom - _fontSize-Offset/2f);
            }
            // render the slider 
            UpdatePos();
            float x = Bounds.X + X;
            _sliderRec = new RectangleF(x - _radius, y - 2*_radius, 2f * _radius, 2f * _radius);
            graphics.FillPolygon(
                Enabled?
                (
                isMoving?
                Brushes.OrangeRed : ((Highlighted)?Brushes.LightYellow: Brushes.Black)
                ):Brushes.Gray ,
                new PointF[]
                {
                new PointF(_sliderRec.Left,_sliderRec.Top),new PointF(_sliderRec.Right,_sliderRec.Top),new PointF(x,_sliderRec.Bottom)
                });
            
            if (Highlighted && showLabel)
            {
                string textValue = GetValueAsString(GetNormalizedValue(X));
                Size size = GH_FontServer.MeasureString(textValue, SmallFont);
                y = y -size.Height - 2*_radius-1; // mobe the point to the top of the slider 
                x = x - size.Width / 2f-1;
                RectangleF border = new RectangleF(x, y, size.Width+2, size.Height+2);
                graphics.FillRectangle(Brushes.LightYellow, border);
                using (Pen p = new Pen(Brushes.Black, 1))
                    graphics.DrawRectangle(p, border.X,border.Y,border.Width,border.Height);
                
                graphics.DrawString(textValue, SmallFont, Brushes.Black,border, _format);
                
            }

        }

         
    }
}
