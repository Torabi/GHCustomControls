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
    /// A PushButton value can be 0 or 1, when the button is pressed (Mouse left click) the value changes to 1 and when the mouse leaves the button
    /// or the left button is up the value changes back to the 0. The solution is updated when the button is pressed (only if UpdateSolution is set to true)
    /// </summary>
    public class PushButton : GHParameter
    {

        Image _icon;
        bool _showTitle;
        int _fixedWidth = 0;
        string _text;
        bool _live;
        public string Text { 
            get=>_text;
            set
            {
                bool redraw = _text != value;
                _text = value;
                if (redraw)
                {

                    Grasshopper.Instances.ActiveCanvas?.ScheduleRegen(100);
                }

            }
        
        }

        public PushButton (string name,string description, string text="",bool showTitle=true,Image icon=null, int fixedWidth = 0,bool live = true, Bitmap toolTipDiagram = null) :base(name,description,0,toolTipDiagram)
        {
            _icon = icon;
            _showTitle = showTitle;
            _fixedWidth = fixedWidth;
            Text = text == string.Empty?name:text;
            _live = live;
        }
        public override int Offset => 2;

        public override GH_Types DataType => GH_Types.gh_int32;

        public override GH_ParamAccess Access => GH_ParamAccess.item;

        /// <summary>
        /// true between the mouse down and mouse up event
        /// </summary>
        bool _isPressed = false;
        #region mouse events
        public override void MouseLeave(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
            if (e.WinFormsEventArgs.Button != System.Windows.Forms.MouseButtons.Left)
                _isPressed = false;
            //if (Highlighted==0)
            //{
            //    Highlighted = -1;
                result = result | GHMouseEventResult.Invalidated; 
            //}

        }

        internal override void MouseLeftClick(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
            if (!Enabled)
                return;
            _isPressed = true;
            CurrentValue = 1;
            result = result | GHMouseEventResult.Handled | GHMouseEventResult.Invalidated | GHMouseEventResult.UpdateSolution;
        }

        //internal override void MouseOver(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        //{
            //if (Highlighted == -1)
            //{
            //    Highlighted = 0;
            //    result = result | GHMouseEventResult.Invalidated;
            //}
        //}

        internal override void MouseRightClick(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
            
        }

        public override void MouseKeyUp(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
            base.MouseKeyUp(sender, customComponent, e, ref result);
            if (!Enabled)
                return;
            _isPressed = false;
            CurrentValue = 0;
            result = result | GHMouseEventResult.Invalidated | GHMouseEventResult.Handled;
        }
        #endregion

        internal override void Render(Graphics graphics, PointF cursorCanvasPosition, bool selected, bool locked, bool hidden)
        {
            bool Highlighted = _live? Bounds.Contains(cursorCanvasPosition):true;
                GH_PaletteStyle style = new GH_PaletteStyle(
                (!Enabled || locked || hidden) ? Color.DarkGray                
                :
                (_isPressed ? Color.OrangeRed :
                (Highlighted ) ? Color.LightGray : Color.SlateGray), _isPressed ? Color.LightGoldenrodYellow : Color.DimGray);

            RectangleF rec = Bounds.Offset(Offset);
            
            GH_Capsule capsule = GH_Capsule.CreateCapsule(rec, GH_Palette.Transparent, (!Enabled || locked || hidden)?0:1, 0);

             
            if (!Enabled || locked)
                capsule.Render(graphics, selected, true, false);
            else
                capsule.Render(graphics, style);
             
            capsule.Dispose();
            if (_icon!=null)
            {
                if(_showTitle)
                    graphics.DrawImage(_icon, rec.Left, rec.Top);
                else
                    graphics.DrawImage(_icon, rec.Offset(Offset));
            }
            if (_showTitle)
            {
                using (StringFormat s = new StringFormat() { Alignment = (_icon == null ? StringAlignment.Center : StringAlignment.Near), LineAlignment = (_icon == null ? StringAlignment.Center : StringAlignment.Near), Trimming = StringTrimming.EllipsisCharacter })
                {
                    if (Highlighted && !locked && Enabled)
                    {
                        using (Font f = new Font(SmallFont, FontStyle.Bold))
                        {
                            if (_icon == null)
                                graphics.DrawString(_text, f, ActiveBrush(), rec, s);
                            else
                                graphics.DrawString(_text, f, ActiveBrush(), rec.Left + _icon.Width + Offset,rec.Top, s);
                        }
                    }
                    else
                    {
                        if (_icon == null)
                            graphics.DrawString(_text, SmallFont, ActiveBrush(),rec, s);
                        else
                            graphics.DrawString(_text, SmallFont, ActiveBrush(), rec.Left + _icon.Width + Offset, rec.Top, s);
                    }
                }
            }
        }

        
     

        internal override int GetHeight()
        {
            return (_showTitle? GH_FontServer.MeasureString(_text, StandardFont).Height:0)+(_icon!=null?_icon.Height:0) + 4 * Offset ;
        }

        internal override int GetWidth()
        {
            if (_fixedWidth > 0)
            
                return _fixedWidth;
            else
                return (_showTitle ? GH_FontServer.StringWidth(_text, StandardFont) : 0) + (_icon != null ? _icon.Width : 0) + 4 * Offset;
            
        }
    }
}
