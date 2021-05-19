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
    public enum Orientation
    {
        Horizontal, Vecrtical
    }

    public class MultiToggleSwitchControl : GHParameter 
    {

        private MultiToggleSwitchItem[] _items ;
        readonly StringFormat _format;
        public MultiToggleSwitchControl(string name, string description, Orientation orientation, bool showTitle, int defaultValue ,Bitmap toolTipDiagram ,  params MultiToggleSwitchItem[] items) : base(name,description, defaultValue, toolTipDiagram)
        {
            _items =items;
            
            _orientation = orientation;

            _format = new StringFormat();
            _format.Alignment = StringAlignment.Near;
            _format.LineAlignment = StringAlignment.Near;
            _format.Trimming = StringTrimming.EllipsisCharacter;
            foreach (MultiToggleSwitchItem item in _items)
                item.ShowTitle = showTitle;
        }
        public MultiToggleSwitchControl(string name, string description, Orientation orientation, bool showTitle, int defaultValue, params MultiToggleSwitchItem[] items) : base(name, description, defaultValue, null)
        {
            _items = items;

            _orientation = orientation;

            _format = new StringFormat();
            _format.Alignment = StringAlignment.Near;
            _format.LineAlignment = StringAlignment.Near;
            _format.Trimming = StringTrimming.EllipsisCharacter;
            foreach (MultiToggleSwitchItem item in _items)
                item.ShowTitle = showTitle;
        }
        int Count => _items.Length;
        public MultiToggleSwitchItem this[int index]
        {
            get => _items[index];
        }
        readonly Orientation _orientation;

     
        public override int Offset => 4;
        const int fontHeight = 24;
        //int _selected = -1;
        public int SelectedItem
        {
            get
            {
                return Array.FindIndex(_items, (MultiToggleSwitchItem item) => item.Value == (int)CurrentValue);
            }
            set
            {
                if (value > -1)
                {
                    CurrentValue =  _items[value].Value;
                }
            }
        }

        int _height = 0;
        internal override int GetHeight()
        {
            if (_height > 0)
                return _height;
            if (_orientation == Orientation.Horizontal)
                _height = Offset + fontHeight + this[0].GetHeight();
            else
                _height = fontHeight + Count * (Offset + this[0].GetHeight());
            return _height;
        }
        int _width = 0; 
        internal override int GetWidth()
        {
            if (_width > 0)
                return _width;
            int nameWidth = Offset + GH_FontServer.StringWidth(Name, StandardFont);

            if (_orientation == Orientation.Horizontal)
            {
                int w = _items.Max(i => i.GetWidth())+Offset;

               
                _width =  Math.Max(nameWidth, w*Count);
            }
            else
            {
                _width = Math.Max(nameWidth, Offset + _items.Max(i => i.GetWidth()));
            }
            return _width;
        }

        
        RectangleF _bounds;
        public override RectangleF Bounds 
        { 
            get => _bounds;
            set
            {
                _bounds = value;
                float shift = Offset;
                if (_orientation == Orientation.Horizontal)
                {
                    float w = _bounds.Width / Count - Offset;
                    foreach (MultiToggleSwitchItem item in _items)
                    {

                        //int w = item.GetWidth();
                        item.Bounds = new RectangleF(_bounds.X + shift, _bounds.Y + Offset / 2.0f+ fontHeight, w, item.GetHeight());
                        shift += w + Offset / 2.0f;
                    }
                }
                else
                {
                    foreach (MultiToggleSwitchItem item in _items)
                    {

                        int h = item.GetHeight();
                        item.Bounds = new RectangleF(_bounds.X+Offset / 2.0f, _bounds.Y + shift+ fontHeight, _bounds.Width-Offset, h);
                        shift += h + Offset / 2.0f;
                    }
                }
            } 
        }

       
        public override GH_Types DataType => GH_Types.gh_int32;

        public override GH_ParamAccess Access =>  GH_ParamAccess.item;



        internal override void Render(Graphics graphics, PointF cursorCanvasPosition, bool selected, bool locked,bool hidden)
        {
            /*
            GH_Capsule capsule = GH_Capsule.CreateCapsule(Bounds, GH_Palette.Transparent,2,0);
            
            capsule.Render(graphics, selected,locked,hidden);
            capsule.Dispose();
            // render the name 
            graphics.DrawString(Name, SmallFont, Brushes.Black,  Bounds.X +  Offset /2.0f, Bounds.Y + Offset / 2.0f, _format);
            */

            Helpers.DrawFrame(Bounds, graphics, Name, selected, locked || !Enabled, hidden);
            // render the options 
            foreach (MultiToggleSwitchItem item in _items)
            {
                item.IsSelected = item.Value == (int)CurrentValue;
                item.Render(graphics, cursorCanvasPosition, selected, locked || !Enabled, hidden);
            }
        }


        #region mouse events
        internal override void MouseOver(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult invalidated)
        {

            base.MouseOver(sender, customComponent, e, ref invalidated);
            foreach (GHControl item in _items)
            {
                if (item.Bounds.Contains(e.CanvasLocation))
                {
                    item.MouseOver(sender, customComponent, e, ref invalidated);
                }
                else
                {
                    item.MouseLeave(sender, customComponent, e, ref invalidated);
                }
            }


        }
        internal override RectangleF ActiveZone => SelectedItem==-1?Bounds: _items[SelectedItem].Bounds;
        //internal override void MouseLeave(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult invalidated)
        //{
        //    base.MouseLeave(sender, customComponent, e, ref invalidated);
        //    foreach (GHCustomControl item in _items)
        //    {


        //        item.MouseLeave(sender, customComponent, e, ref invalidated);

        //    }
        //}

        internal override void MouseLeftClick(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {

            
            for (int i = 0;i<_items.Length;i++)
            {
                MultiToggleSwitchItem toggle = _items[i];
                if (toggle.Bounds.Contains(e.CanvasLocation))
                {
                   
                    if (!toggle.IsSelected)
                    {
                        SelectedItem = i;
                        result = result | GHMouseEventResult.UpdateSolution | GHMouseEventResult.Handled;
                    }
                }
                
            }
            
        }

        internal override void SetupToolTip(PointF canvasPoint, GH_TooltipDisplayEventArgs e)
        {
       
            foreach (GHControl control in _items)
            {
                if (control.Bounds.Contains(canvasPoint))
                {
                    control.SetupToolTip(canvasPoint, e);
                    return;
                }
            }
            base.SetupToolTip(canvasPoint, e);
        }

        internal override void MouseRightClick(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
            
        }
        #endregion
    }
}
