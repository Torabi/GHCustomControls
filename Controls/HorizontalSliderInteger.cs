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
    public class HorizontalSliderInteger : HorizontalSlider
    {
        
        public HorizontalSliderInteger(string name,string description,int value,int min,int max,string suffix = "", bool showTitle = true) 
            : base(name, description, value, min, max,showTitle, "",0, suffix) { }
        public override GH_Types DataType => GH_Types.gh_int32 ;

        public override GH_ParamAccess Access =>  GH_ParamAccess.item;

        public override string GetValueAsString(float x)
        {
            int v = (int)Math.Round( _min + x * (_max - _min),0);
            return v.ToString(FormatString) + Suffix;
            
        }

        public override void UpdatePos()
        {
            X = GetPos((int)CurrentValue);
        }

        public override void UpdateValue()
        {
            CurrentValue = (int)Math.Round(GetCurrentValue(), 0);
        }

        internal override void Render(Graphics graphics, PointF cursorCanvasPosition, bool selected, bool locked, bool hidden)
        {
            base.Render(graphics, cursorCanvasPosition, selected, locked, hidden);
            
            // draw addintional texts and lines 
            if (_max-_min<=20)
            {
                using (StringFormat s= new StringFormat() {Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Far })
                using (Font f = new Font(SmallFont.FontFamily,4))
                using (Pen p = new Pen(ActiveBrush()))
                {
                    for (int i = (int)_min + 1; i < _max; i++)
                    {
                        float x = GetPos(i);
                        float xx = x + Bounds.Left;

                        if (x != X)
                        {
                            graphics.DrawLine(p, xx, _sliderBar.Top, xx, _sliderRec.Top);
                            graphics.DrawString(i.ToString(), f, ActiveBrush(), xx, _sliderRec.Top, s);
                        }
                    }
                }
            }
            //if ((int)CurrentValue>_min && (int)CurrentValue <_max && !Bounds.Contains(cursorCanvasPosition))
            //{
            //    using (StringFormat s = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Far })
                
            //    using (Pen p = new Pen(ActiveBrush()))
            //    {
            //        graphics.DrawString(CurrentValue.ToString(), SmallFont, ActiveBrush(), X+Bounds.X, _sliderRec.Top, s);
            //    }
            //}
        }
        internal override void MouseRightClick(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
            base.MouseRightClick(sender, customComponent, e, ref result);
            if (result.HasFlag(GHMouseEventResult.Handled))
                return;
            int d = (int)CurrentValue;
            //NumericUpDownData<int> numeric = new NumericUpDownData<int>(d,(int) _min,(int) _max, FormatString);
            IntegerNumber number = new IntegerNumber((int)CurrentValue,(int)_min,(int) _max);
            //if (numeric.GetInput(PointToScreen(sender, Pos) , out int val))
            if (Helpers.GetIntegerInput(PointToScreen(sender, Pos),number))
            {
                CurrentValue = number.Value;
                result = result | GHMouseEventResult.UpdateSolution | GHMouseEventResult.Handled;
            }
            else
            {
                result = result | GHMouseEventResult.Handled;
            }
            showLabel = true;
        }
    }
}
