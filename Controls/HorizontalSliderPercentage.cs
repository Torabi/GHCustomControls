using GH_IO.Types;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
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
    /// <summary>
    /// construct a horizontal slider representing percentage value 
    /// the value varies between 0 and 1 . 
    /// </summary>
    public class HorizontalSliderPercentage : HorizontalSlider
    {
        
        /// <summary>
        /// create a horizontal slider for percentage input , the actual value is between 0 to 1
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="decimals"></param>
        /// <param name="suffix"></param>
        public HorizontalSliderPercentage(string title, string description, float value,float min=0,float max=1, int decimals = 0, string suffix="", bool showTitle = true) : base(title, description, value, min, max,showTitle,$"P{decimals}",decimals,suffix)
        {
  

        }
        
 
        public override GH_Types DataType => GH_Types.gh_decimal;

        public override GH_ParamAccess Access => GH_ParamAccess.item;

        public override string GetValueAsString(float x)
        {
            double v = _min + x * (_max - _min);
            
            return v.ToString(FormatString, CultureInfo.InvariantCulture);
        }


        public override void UpdateValue()
        {
            CurrentValue = (float)Math.Round(GetCurrentValue(), 2);
        }

        internal override void Render(Graphics graphics, PointF cursorCanvasPosition, bool selected, bool locked, bool hidden)
        {
            base.Render(graphics,  cursorCanvasPosition, selected, locked, hidden);
            using (Pen p = new Pen(Brushes.DimGray, 0.5f))
            {
                int j = 0;
                for (float i = _min; i <= _max; i+=(_max-_min)/20)
                {
                    float x = Bounds.X + Offset + GetPos(i );
                    if (j == 10)
                    {
                        graphics.DrawLine(p, x, _sliderBar.Bottom, x, _sliderBar.Bottom - 15);
                    }
                    else if (j == 5 || j ==15)
                    {
                        graphics.DrawLine(p, x, _sliderBar.Bottom, x, _sliderBar.Bottom - 10);
                    }
                    else
                    {
                        graphics.DrawLine(p, x, _sliderBar.Bottom, x, _sliderBar.Bottom - 5);
                    }
                    j++;
                }
            }
        }
 
 
       
        public override void UpdatePos()
        {
            X = GetPos((float)CurrentValue);
        }

        internal override void MouseRightClick(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
            base.MouseRightClick(sender, customComponent, e, ref result);
            if (result.HasFlag(GHMouseEventResult.Handled))
                return;
            
            FloatNumber number = new FloatNumber((float)CurrentValue*100, _min*100, _max*100, (_max - _min) / 100, _decimals);
            //NumericUpDownData<decimal> numeric = new NumericUpDownData<decimal>(d, Convert.ToDecimal( _min), Convert.ToDecimal(_max),FormatString);
            //if (numeric.GetInput(PointToScreen(sender, Pos), out decimal val))
            if (Helpers.GetFloatInput(PointToScreen(sender, Pos),number))
            {
                CurrentValue = number.Value/100;
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
