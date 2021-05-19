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
    public class Label : GHParameter
    {
        int _fixedWidth = 0;
        public Label(string name, string description, string value,int fixedWidth = 0, bool border = false, StringAlignment alignment = StringAlignment.Center, Bitmap toolTipDiagram = null) : base(name, description, value,toolTipDiagram) 
        {
            Border = border;
            Alignment = alignment;
            _fixedWidth = fixedWidth;
        }
        
        public override GH_Types DataType => GH_Types.gh_string;

        public override GH_ParamAccess Access =>  GH_ParamAccess.item;

        public override int Offset => 2;

        public bool Border { get; set; }

        public StringAlignment Alignment { get; set; }

        

        internal override void MouseLeave(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
            
        }

        internal override void MouseLeftClick(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
            
        }

        internal override void MouseOver(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
             
        }

        internal override void MouseRightClick(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
             
        }

        internal override void Render(Graphics graphics, PointF cursorCanvasPosition, bool selected, bool locked, bool hidden)
        {
            
            using (StringFormat s = new StringFormat() { Alignment = Alignment, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter })
            {
                graphics.DrawString((string)CurrentValue, SmallFont, ActiveBrush(), Bounds.Offset(Offset), s);
            }
            if (Border)
            {
                using (Pen p = new Pen(ActiveBrush()))
                {
                    graphics.DrawRectangle(p, Bounds.X,Bounds.Y,Bounds.Width,Bounds.Height);
                }
            }
        }

       

        internal override int GetHeight()
        {
            return 2 * Offset + GH_FontServer.MeasureString((string)CurrentValue, SmallFont).Height;
        }

        internal override int GetWidth()
        {
            return  (_fixedWidth>0)?_fixedWidth: 2 * Offset + GH_FontServer.StringWidth((string)CurrentValue, SmallFont);
        }
    }
}
