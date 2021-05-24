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
    public class StackPanel : GHControl, IGHPanel
    {
        public StackPanel (string name,params GHControl[] items):base(name,"")
        {
            _orientation = Orientation.Horizontal; 
            Items = items;
            _showBorder = false;
            
 
         
        }
        public StackPanel(string name, Orientation orientation, params GHControl[] items):base(name,"")
        {
            _orientation = orientation;
            Items = items;
            _titleHeight = GH_FontServer.MeasureString(Name, SmallFont).Height + Offset * 2;
            _showBorder = false;

        }
        public StackPanel(string name, Orientation orientation, bool drawBorder, params GHControl[] items) : base(name, "")
        {
            _orientation = orientation;
            Items = items;
            _titleHeight = GH_FontServer.MeasureString(Name, SmallFont).Height + Offset * 2;
            _showBorder = drawBorder;

        }
        public void Add(GHControl control)
        {
            control.attributes = attributes;
            Items = Items.Append(control).ToArray();
        }
        

        Orientation _orientation;
        int _titleHeight = 0;
        RectangleF _bounds;
        bool _showTitle = false;
        bool _showBorder = false;
        public override RectangleF Bounds { 
            get=>_bounds ;
            set
            {
                _bounds = value;
                this.SetChildrenBounds(0,_showTitle?_titleHeight:0);
            }
        }
 
        public override int Offset => 4;

     
        public GHControl[] Items { get; set ; }
        public Orientation Orientation { 
            get => _orientation; 
            set { _orientation = value; } 
        }

        internal override int GetHeight()
        {
            int h = this.GetChildrenHeight(Items);
            if (_showTitle)
                h += _titleHeight;
            if (_showBorder)
                h += 2 * Offset;
            
            return h;
                      
        }

        internal override int GetWidth()
        {
            int w  =  this.GetChildrenWidth(Items);
            if (_showBorder)
                w += 2 * Offset;
            return w;
        }

        //internal override void MouseLeave(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        //{
        //    base.MouseLeave(sender, customComponent, e, ref result);
        //    //foreach (GHCustomControl control in Items)
        //    //{

        //    //    control.MouseLeave( sender, customComponent, e, ref result);

        //    //}
        //}

        internal override void MouseLeftClick(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
           
            this.MouseClickChildren( sender, customComponent, e, ref result);
        }

        //internal override void MouseOver(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        //{
            
        //    this.MouseOverChildren( sender, customComponent, e, ref result);
        //}

        internal override void MouseRightClick(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
         
            this.MouseClickChildren( sender, customComponent, e, ref result);
        }

        internal override void Render(Graphics graphics, PointF cursorCanvasPosition, bool selected, bool locked, bool hidden)
        {
            if (!IsVisible)
                return;
            if (_showBorder)
            {
                using (Pen p = new Pen(ActiveBrush()))
                {
                    graphics.DrawRectangle(p,Rectangle.Round( Bounds));
                }
            }
            foreach (GHControl control in Items)
                if (control.IsVisible)
                control.Render(graphics, cursorCanvasPosition, selected, locked, hidden);
        }

        internal override void SetupToolTip(PointF canvasPoint, GH_TooltipDisplayEventArgs e)
        {
           
            this.SetChildrenToolTip(canvasPoint, e);                

        }
    }
}
