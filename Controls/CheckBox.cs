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
    /// <summary>
    /// Display a check box in the gasshopper component, the CurrentValue defines the check status of the checkbox.
    /// </summary>
    public class CheckBox : GHParameter
    {
        /// <summary>
        /// construct an instance of the check box
        /// </summary>
        /// <param name="name">Parameter name also the label displayed next to the checkbox</param>
        /// <param name="description">The description used in tooltip</param>
        /// <param name="isChecked">the initial status of the check box (CurrentValue)</param>
        /// <param name="toolTipDiagram">An optional bitmap to show in the detail area of the tooltip</param>
        public CheckBox(string name,string description, bool isChecked,Bitmap toolTipDiagram = null ):base(name,description, isChecked, toolTipDiagram)
        {
        }
        
        public sealed override GH_Types DataType =>  GH_Types.gh_bool;
        
        public sealed override GH_ParamAccess Access => GH_ParamAccess.item;
        RectangleF _bounds;
        public sealed override RectangleF Bounds { 
            get=>_bounds ;
            set 
            {
                _bounds = value;
                _checkBoxRec = new RectangleF(_bounds.Left + Offset, _bounds.Top + Offset, CheckBoxSize, CheckBoxSize);
            } 
        }
        
        
        public sealed override int Offset => 2;

        /// <summary>
        /// size of the square
        /// </summary>
        public virtual int CheckBoxSize => 12;
        /// <summary>
        /// the rectangle representing the check box
        /// </summary>
        RectangleF _checkBoxRec;

        internal override  int GetHeight()
        {
            Size size = GH_FontServer.MeasureString(Name, SmallFont);
            return Math.Max(CheckBoxSize, size.Height) + Offset;
        }

        internal override  int GetWidth()
        {
            return GH_FontServer.StringWidth(Name, SmallFont) + CheckBoxSize + 2*Offset;
        }

       

        internal override void MouseLeftClick(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
          
            if (_checkBoxRec.Contains(e.CanvasLocation))
            {
                CurrentValue = !(bool)CurrentValue;
                
                result = result | GHMouseEventResult.Handled | GHMouseEventResult.UpdateSolution ;
            }
            else
            {
                result = result | GHMouseEventResult.Handled;
            
            }
        }
        public override RectangleF ActiveZone => _checkBoxRec;

        internal override void MouseRightClick(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
             
        }

        internal override void Render(Graphics graphics, PointF cursorCanvasPosition, bool selected, bool locked, bool hidden)
        {
            bool Highlighted = _checkBoxRec.Contains(cursorCanvasPosition);
            // render the check box 
            
            using (Pen p = new Pen(this.ActiveBrush(), 2))
            {
                graphics.DrawRectangle(p, Rectangle.Round( _checkBoxRec));
            }
            if (Highlighted)
                graphics.FillRectangle(Brushes.AntiqueWhite, _checkBoxRec);
            if ((bool)CurrentValue)
                graphics.FillRectangle(this.ActiveBrush(), _checkBoxRec.Offset(2));
            // render the text 
            using (StringFormat sf = new StringFormat() { Alignment= StringAlignment.Near})
            {
                graphics.DrawString(Name, SmallFont, this.ActiveBrush(), _checkBoxRec.Right+Offset, _checkBoxRec.Top, sf);
            }
            

        }

        
    }
}
