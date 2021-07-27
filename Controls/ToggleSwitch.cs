using GH_IO.Types;
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
    public class ToggleSwitch : GHParameter
    {
        /// <summary>
        /// construict a new instanc of toggle switch 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="defaultValue"></param>
        /// <param name="toolTipDiagram"></param>
        public ToggleSwitch(string name, string description, bool defaultValue, Bitmap toolTipDiagram = null) : base(name, description, defaultValue, toolTipDiagram)
        {

        }

        #region Brushes & Colors
        /// <summary>
        /// the fill color of the background when toggle is on
        /// </summary>
        public Color OnFill { get; set; } = Color.LawnGreen;

        /// <summary>
        /// the fill color of the background when toggle is off
        /// </summary>
        public Color OffFill { get; set; } = Color.DarkGray;
        /// <summary>
        /// the color of the toggle (default is white)
        /// </summary>
        public Brush ToggleBrush { get; set; } = Brushes.White;
        /// <summary>
        /// default color of the border 
        /// </summary>
        public Color BorderColor { get; set; } = Color.Black;
        /// <summary>
        /// the color of the border when it is highlighte (mouse over)
        /// </summary>
        public Color BorderColorHighlighted { get; set; } = Color.OrangeRed;

        #endregion

        public override GH_Types DataType => GH_Types.gh_bool;

        public override GH_ParamAccess Access =>  GH_ParamAccess.item;

        

        RectangleF _toggleRec;
        
        public override RectangleF Bounds { get  ; set  ; }
         
        public override int Offset => 4;

        
        int _height = 14;
        internal override int GetHeight()
        {
            return _height+ Offset;
        }
        int _width = 30; 
        internal override int GetWidth()
        {
            return GH_FontServer.StringWidth(Name,SmallFont)+ 2* _width;
        }

        internal override void MouseLeftClick(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
             
            if (_toggleRec.Contains(e.CanvasLocation))
            {
                // click inside the button
                CurrentValue = !(bool)CurrentValue;
                result = result | GHMouseEventResult.Handled| GHMouseEventResult.Invalidated | GHMouseEventResult.UpdateSolution;
            }
        }

        //internal override void MouseLeave(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        //{
            
        //    //if (Highlighted==0)
        //    //{
        //    //    Highlighted = -1;
        //        result = result | GHMouseEventResult.Invalidated;
        //    //}
        //}

        //internal override void MouseOver(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        //{

        //if (_toggleRec.Contains(e.CanvasLocation))
        //{
        //    if (Highlighted!=0)
        //    {
        //        Highlighted = 0;
        //        result = result | GHMouseEventResult.Invalidated;
        //    }
        //}
        //else
        //{
        //    if (Highlighted==0)
        //    {
        //        Highlighted = -1;
        //        result = result | GHMouseEventResult.Invalidated;
        //    }
        //}
        //}
        public override RectangleF ActiveZone => _toggleRec;

        internal override void Render(Graphics graphics,PointF cursorCanvasPosition, bool selected, bool locked, bool hidden)
        {
            bool Highlighted = Bounds.Contains(cursorCanvasPosition);
            bool _toggle = (bool)CurrentValue;
            // draw the name and the seperator line 
            using (StringFormat s = new StringFormat()
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Near,
                Trimming = StringTrimming.EllipsisCharacter
            })
            {
                graphics.DrawString(Name,SmallFont,this.ActiveBrush(),Bounds.Left + Offset,Bounds.Top + Offset, s);
            }
            using (Pen p = new Pen(Brushes.DimGray, 0.5f))
            {
                graphics.DrawLine(
                   p,
                    Bounds.Left + Offset,
                    Bounds.Top + SmallFont.Height + Offset / 2,
                    Bounds.Right - Offset,
                    Bounds.Top + SmallFont.Height + Offset / 2);
            }
            // draw the button
            _toggleRec = new RectangleF(Bounds.Right - Offset - _width, Bounds.Top + Offset/2, _width, _height);
            GH_Capsule capsule = GH_Capsule.CreateCapsule(_toggleRec, GH_Palette.Normal, (int)_toggleRec.Height / 2, 0);
            capsule.Render(graphics, new GH_PaletteStyle(
                (Enabled)?(_toggle ? OnFill : OffFill): Color.Gray,
                (Enabled) ? ((Highlighted)?BorderColorHighlighted:BorderColor): Color.Gray));
            capsule.Dispose();
            graphics.FillEllipse(
                (Enabled)?ToggleBrush:Brushes.DarkGray,
                (_toggle) ? _toggleRec.Right-_height : _toggleRec.Left+2,
                _toggleRec.Top+2 ,
                _height-4, _height-4);
             
        }

   

        internal override void MouseRightClick(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
            
        }
    }
}
