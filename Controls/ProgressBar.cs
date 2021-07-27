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
    public class ProgressBar : GHControl
    {

        //int MinWidth { get; set; } = 100;
        float _value=0;

        
        public float Value
        {
            get => _value;
            set
            {
                bool redraw = _value != value;
                _value = value;
                if (redraw)
                    Grasshopper.Instances.ActiveCanvas?.ScheduleRegen(100);
            }
        }
        public ProgressBar(string name) : base(name, "") { }
        
        public override int Offset => 2;

        internal override int GetHeight()
        {
            return 2 * Offset + GH_FontServer.MeasureString("100%", SmallFont).Height;
        }

        internal override int GetWidth()
        {
            return 2 * Offset + 2 * GH_FontServer.StringWidth("100.00%", SmallFont);
        }
        #region mouse events
        public override void MouseLeave(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
            
        }

        internal override void MouseLeftClick(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
             
        }

        public override void MouseOver(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {

        }

        internal override void MouseRightClick(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
            
        }
        #endregion
        internal override void Render(Graphics graphics, PointF cursorCanvasPosition, bool selected, bool locked, bool hidden)
        {
            GH_PaletteStyle backgroundStyle = new GH_PaletteStyle(
               (!Enabled || locked) ? Color.DarkGray
               :
               Color.LightGray , Color.DimGray);
            GH_PaletteStyle forgroundStyle = new GH_PaletteStyle(
              (!Enabled || locked) ? Color.DarkGray
              :
              Color.LawnGreen, Color.Transparent);

            // background
            GH_Capsule capsule = GH_Capsule.CreateCapsule(Bounds, GH_Palette.Transparent, 2, 1);

            if (!hidden)
            {
                if (!Enabled || locked)
                    capsule.Render(graphics, selected, true, false);
                else
                    capsule.Render(graphics, backgroundStyle);
            }
            capsule.Dispose();
            float w = (Bounds.Width - 2 * Offset) * Value;
            if (w > 1)
            {
                // forground 
                RectangleF rec = new RectangleF((Bounds.Left + Offset), Bounds.Top + Offset, w, Bounds.Height - 2 * Offset);
                capsule = GH_Capsule.CreateCapsule(rec, GH_Palette.Transparent, 2, 1);

                if (!hidden)
                {
                    if (!Enabled || locked)
                        capsule.Render(graphics, selected, true, false);
                    else
                        capsule.Render(graphics, forgroundStyle);
                }
                capsule.Dispose();
            }
            if (Value >= 0 && Value <= 1)
            {
                using (StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    graphics.DrawString(Value.ToString("P0"), SmallFont, ActiveBrush(), Bounds, sf);
                }
            }
        }

        
    }
}
