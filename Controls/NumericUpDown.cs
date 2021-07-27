using GH_IO.Types;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
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
    public class NumericUpDown<T> : GHParameter,IGHPanel where T : struct, IFormattable, IComparable<T>
    {

        string FormatString = "";
        //RectangleF _left, _right;
        int _width, _height,_btnWidh,_labelWidth,_titleWidth;
        PushButton _left, _right;
        PushButton _label;
        GHControl[] _controls;
        int incerement = 0;
        object _smallChange;
        System.Timers.Timer timer;
        T _min, _max;

        
        public NumericUpDown(string name ,string description,T value, T min,T max, T smallChange, string formatString="", Bitmap toolTipDiagram = null) : base(name,description,value,toolTipDiagram) 
        {
            FormatString = formatString;
            _smallChange = smallChange;
            _min = min;
            _max = max;
            _btnWidh = GH_FontServer.StringWidth("<", SmallFont);
            _labelWidth = GH_FontServer.StringWidth("888.888", SmallFont);
            _titleWidth = GH_FontServer.StringWidth(Name, SmallFont);
            _width = _labelWidth + 2*_btnWidh + 2 * Offset+_titleWidth;
            _height = GH_FontServer.MeasureString("A", SmallFont).Height + 2 * Offset;

            _left = new PushButton("Up", "Increase", "<");
            _right = new PushButton("Down", "Decrease", ">");
            _label = new PushButton("Value", description, AsString(value), true,null, _labelWidth);
            _left.UpdateSolution = false;_right.UpdateSolution = false; _label.UpdateSolution = false;
            _left.OnValueChanged += _left_OnValueChanged;
            _right.OnValueChanged += _right_OnValueChanged;
            _label.OnValueChanged += _label_OnValueChanged;
            _controls = new GHControl[] { _left, _label, _right };
            timer = new System.Timers.Timer();
            timer.Interval = 100;
            timer.Elapsed += Timer_Elapsed;

        }


        public T Max
        {
            get => _max;
            set {_max = value;}
        }
        public T Min
        {
            get => _min;
            set { _min = value; }
        }

        void changeVlaue()
        {

            if (CurrentValue is double)
            {
                double temp = ((double)CurrentValue) + incerement * ((double)_smallChange);
                if (temp >= Convert.ToDouble(_min) && temp <= Convert.ToDouble(_max))
                    CurrentValue = temp;

            }
            else if (CurrentValue is int)
            {
                int temp = ((int)CurrentValue) + incerement * ((int)_smallChange);
                if (temp >= Convert.ToInt32(_min) && temp <= Convert.ToInt32(_max))
                    CurrentValue = temp;
            }
            else if (CurrentValue is float)
            {
                float temp = ((float)CurrentValue) + incerement * ((float)_smallChange);
                if (temp >= (float)Convert.ToDecimal(_min) && temp <= (float)Convert.ToDecimal(_max))
                    CurrentValue = temp;
            }
            else
                throw new Exception($"invalid type ({CurrentValue})");
            _label.Text = AsString(CurrentValue);
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (incerement!=0)
            {
                changeVlaue();
                Grasshopper.Instances.ActiveCanvas?.ScheduleRegen(100);

            }
            else
            {
                timer.Stop();
            }
        }
        private void _right_OnValueChanged(object sender, ValueChangeEventArgumnet e)
        {
            if (((int)e.Value) == 1)
            {
                incerement = 1;
                //changeVlaue();
                timer.Start();
            }
            else
            {
               
                
                timer.Stop();
                changeVlaue();
                Attributes.Owner.ExpireSolution(true);
                incerement = 0;
            }
        }
        private void _left_OnValueChanged(object sender, ValueChangeEventArgumnet e)
        {
            if (((int)e.Value)==1)
            {
                incerement = -1;
                //changeVlaue();
                timer.Start();
            }
            else
            {
                
                timer.Stop();
                changeVlaue();
                Attributes.Owner.ExpireSolution(true);
                incerement = 0;
            }
        }
        private void _label_OnValueChanged(object sender, ValueChangeEventArgumnet e)
        {

            if (!Attributes.ContentBox.IsEmpty && (int)e.Value == 0) // on muse up
            {

                //T d = (T)CurrentValue;
                //NumericUpDownData<T> numeric = new NumericUpDownData<T>(d, _min, _max, FormatString);

                GenericNumber<T> number = new GenericNumber<T>((T)CurrentValue, Min, Max, (T)Convert.ChangeType( 1,typeof(T)), 2);
                if (Helpers.GetInput<T>(PointToScreen(Grasshopper.Instances.ActiveCanvas, new PointF((_label.Bounds.Left + _label.Bounds.Right) / 2, _label.Bounds.Top+20)),number))
                {
                    CurrentValue = number.Value;
                    _label.Text = AsString(CurrentValue);
                    Attributes.Owner.ExpireSolution(true);
                }
                ////float s = GH_FontServer.MeasureString("A", SmallFont).Height * sender.Viewport.Zoom / 20;
                //if (numeric.GetInput(PointToScreen(Grasshopper.Instances.ActiveCanvas, new PointF((_label.Bounds.Left + _label.Bounds.Right) / 2, _label.Bounds.Top)), out T val))
                //{
                //    CurrentValue = val;
                //    _label.Text = AsString(CurrentValue);
                //    attributes.Owner.ExpireSolution(true);
                //}

            }
        }

        public override int Offset => 2;

        public override GH_Types DataType
        {
            get
            {
                if (typeof(T).Equals(typeof(double)))
                    return GH_Types.gh_double;
                else if (typeof(T).Equals(typeof(int)))
                    return GH_Types.gh_int32;
                else if (typeof(T).Equals(typeof(float)))
                    return GH_Types.gh_decimal;
                else
                    throw new Exception($"{typeof(T).ToString()} Type is not supported ");

            }
        }

        public override GH_ParamAccess Access => GH_ParamAccess.item;

        public GHControl[] Items { get => _controls; set { } }
        public Orientation Orientation { get=> Orientation.Horizontal; set { } }

        RectangleF _bounds;
        public override RectangleF Bounds
        {
            get => _bounds;
            set
            {
                _bounds = value;
                this.SetChildrenBounds(0, 0, ContentAlignment.MiddleRight);
            }
        }

        internal override int GetHeight() => _height;

        internal override int GetWidth() => _width;
        

        string AsString(object val)
        {
            if (val is double)
                return (Convert.ToDouble(val)).ToString(FormatString);
            else if (val is int)
                return (Convert.ToInt32(val)).ToString(FormatString);
            else if (val is float)
                return (Convert.ToDecimal(val)).ToString(FormatString);
            else
                throw new Exception($"{val.ToString()} Type is not supported ");
        }

        internal override void MouseLeftClick(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
            this.MouseClickChildren(sender, customComponent, e, ref result);
           
        }

        internal override void MouseRightClick(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
            this.MouseClickChildren(sender, customComponent, e, ref result);
        }

        internal override void Render(Graphics graphics, PointF cursorCanvasPosition, bool selected, bool locked, bool hidden)
        {
           
            if (!IsVisible)
                return;
            Helpers.DrawFrame(Bounds, graphics, "", selected, locked, hidden);
            using (StringFormat s = new StringFormat()
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Near,
                Trimming = StringTrimming.EllipsisCharacter
            })

            {
                graphics.DrawString(
                   Name,
                   SmallFont,
                   (locked) ? Brushes.Gray : Brushes.Black,
                  Bounds.Left+Offset,
                   Bounds.Top+Offset,
                   s);

            }
            foreach (GHControl control in Items)
                if (control.IsVisible)
                    control.Render(graphics, cursorCanvasPosition, selected, locked, hidden);
        }

       
    }
}
