using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    internal class NumericUpDownData<T> : INotifyPropertyChanged where T:struct , IFormattable, IComparable<T>
    {

        internal T _min, _max;
        private T _oldVal ;
        public string FormatString;
        
        public NumericUpDownData(T val, T min, T max, string formatString)
        {
            _oldVal= _value = val;
            _min = min;
            _max = max;
            FormatString = formatString;
        }

        T _value;

        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }
        }

        

        
        public event PropertyChangedEventHandler PropertyChanged;


        public bool GetInput (System.Drawing.PointF point,  out T updatedValue)
        {
            NumericUpDownWindow window = new NumericUpDownWindow(new System.Windows.Point(point.X,point.Y));
            window.SetData(this);
            window.ShowDialog();
            updatedValue = _value;
            if (window.DialogResult.HasValue && window.DialogResult.Value)
            {

                
                if (_oldVal.Equals(Value))

                    return false;
                else
                    return true;
            }
            return false;
        }
    }
}
