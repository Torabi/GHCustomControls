using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
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
    public abstract class GHCustomControl : GHControl
    {
        
        public GHCustomControl (string name,string description) :base(name, description) { }

        public abstract int Height { get; }

        public abstract int Width { get; }




        internal override int GetHeight()
        {
            return Height;
        }

        internal override int GetWidth()
        {
            return Width;
        }

        internal override void MouseLeftClick(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
             
            OnMouseClick(sender, customComponent, e, ref result);
        }

        internal override void MouseRightClick(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
             
            OnMouseClick(sender, customComponent, e, ref result);
        }

        internal override void Render(Graphics graphics, PointF cursorCanvasPosition, bool selected, bool locked, bool hidden)
        {
            OnRender(graphics, cursorCanvasPosition, selected, locked, hidden);
        }

        public abstract void OnMouseClick(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result);

        public abstract void OnRender(Graphics graphics, PointF cursorCanvasPosition, bool selected, bool locked, bool hidden);


    }
}
