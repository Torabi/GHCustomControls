using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
/*
     MIT License
    Copyright (c) 2021 ALI TORABI (ali@parametriczoo.com)
    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
    */
namespace GHCustomControls
{
    /// <summary>
    /// class provide basic functionality for different number classes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericNumber<T> : INotifyPropertyChanged where T : struct, IFormattable, IComparable<T>
    {
        /// <summary>
        /// construct a GenericNumber class based on the Type (T)
        /// </summary>
        /// <param name="value">current value</param>
        /// <param name="min">minimum value</param>
        /// <param name="max">maximum value</param>
        /// <param name="inc">increment value used to increment or decrement the current value</param>
        /// <param name="decimals">number of deciams shown on the text box</param>
        public GenericNumber (T value, T min, T max,T inc, int decimals)
        {
            
            Min = min;
            Max = max;
            Inc = inc;
            Decimals = decimals;
            Value = value;

        }
        /// <summary>
        /// number of decimals shown in the textbox
        /// </summary>
        public int Decimals = 0;
        /// <summary>
        /// the current value 
        /// </summary>
        internal T _value;
        /// <summary>
        /// minimum value
        /// </summary>
        public T Min;
        /// <summary>
        /// maximum value
        /// </summary>
        public T Max;
        /// <summary>
        /// value used to increment or decrement the current value
        /// </summary>
        public T Inc;
        /// <summary>
        /// the string value corressponding to the current value 
        /// </summary>
        //internal string _stringValue;

 


        #region Helper methods 

        int toInt(T val)
        {
            return (int)Convert.ChangeType(val, typeof(int));
        }
        decimal toDecimal(T val)
        {
            return (decimal)Convert.ChangeType(val, typeof(decimal));
        }
        double toDouble(T val)
        {
            return (double)Convert.ChangeType(val, typeof(double));
        }
        T toT(int val)
        {
            return (T)Convert.ChangeType(val, typeof(int));
        }
        T toT(decimal val)
        {
            return (T)Convert.ChangeType(val, typeof(decimal));
        }
        T toT(double val)
        {
            return (T)Convert.ChangeType(val, typeof(double));
        }
        #endregion


        /// <summary>
        /// returns <c>_value + _increment</c>
        /// </summary>
        /// <returns></returns>
        internal virtual T increment(T value)
        {
            if (typeof(T).Equals(typeof(int)))
            {
                return toT(toInt(value) + toInt(Inc));
            }
            else if (typeof(T).Equals(typeof(decimal)))
            {
                return toT(toDecimal(value) + toDecimal(Inc));
            }
            else if (typeof(T).Equals(typeof(double)))
            {
                return toT(toDouble(value) + toDouble(Inc));
            }
            else
            {
                throw new Exception("T expected int,decimal and double");
            }
        }

        

        /// <summary>
        /// returns <c>_value - _increment</c>
        /// </summary>
        /// <returns></returns>
        internal virtual T decrement(T value)
        {
            if (typeof(T).Equals(typeof(int)))
            {
                return toT(toInt(value) - toInt(Inc));                
            }
            else if (typeof(T).Equals(typeof(decimal)))
            {
                return toT(toDecimal(value) - toDecimal(Inc));
            }
            else if (typeof(T).Equals(typeof(double)))
            {
                return toT(toDouble(value) - toDouble(Inc));
            }
            else
            {
                throw new Exception("T expected int,decimal and double");
            }
        }
        /// <summary>
        /// provides the regex pattern for filtering numerical value based on the type (T) and lower bound (_min)
        /// </summary>
        internal virtual Regex _regex
        {
            get {
                if (typeof(T).Equals(typeof(int)))
                {
                    return (toInt(Min) < 0) ? signedInteger : unSignedInteger;
                }
                else if (typeof(T).Equals(typeof(decimal)))
                {
                    return (toDecimal(Min) < 0) ? signedDecimal : unSignedDecimal;
                }
                else if (typeof(T).Equals(typeof(double)))
                {
                    return (toDouble(Min) < 0) ? signedDecimal : unSignedDecimal;
                }

                else
                {
                    return signedDecimal;
                }

            }
        }


        /// <summary>
        /// get and sets the current value
        /// </summary>
        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value.CompareTo(value) == 0)
                    return;// nothing to do 
                if (value.CompareTo(Min) < 0)
                {
                    _value = Min;

                }
                else if (value.CompareTo(Max) > 0)
                {
                    _value = Max;

                }
                else
                {
                    _value = value;

                }
                //StringValue = ConvertToString(_value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));

            }
        }
        /// <summary>
        /// The string value shown in the text box .
        /// this property fires the <c>NotifyPropertyChanged</c>
        /// </summary>
        //public string StringValue
        //{
        //    get
        //    {
        //        return _stringValue;
        //    }
        //    set
        //    {


        //        _stringValue = value;
        //        if (_stringValue == string.Empty)
        //            return;
        //        if (!ConvertToValue(value, out T temp))
        //            return;
        //        Value = temp;
        //        //if (temp.CompareTo(_min) < 0 )
        //        //{
        //        //    _value = _min;
        //        //    _stringValue = ConvertToString();
        //        //}else if ( temp.CompareTo(_max) > 0)
        //        //{
        //        //    _value = _max;
        //        //    _stringValue = ConvertToString();
        //        //}
        //        //else
        //        //{
        //        //    _value = temp;
        //        //    _stringValue = ConvertToString();
        //        //}
        //        NotifyPropertyChanged();
        //    }
        //}
        /// <summary>
        /// convert the current value to the string
        /// </summary>
        /// <returns></returns>
        public string ConvertToString(T value )
        {
            return value.ToString(FormatString, CultureInfo.CurrentCulture);
        }
        /// <summary>
        /// override this method to parse the current string to type (T)
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public virtual bool ConvertToValue(string text, out T val)
        {
            bool result = false;
            if (typeof(T).Equals(typeof(int)))
            {
                result= int.TryParse(text, out int x);
                val = toT(x);
            }
            else if (typeof(T).Equals(typeof(decimal)))
            {
                result = decimal.TryParse(text, out decimal x);
                val = toT(x);
            }
            else if (typeof(T).Equals(typeof(double)))
            {
                result = double.TryParse(text, out double x);
                val = toT(x);
            }
            else
            {
                throw new Exception("T expected int,decimal and double");
            }
            return result;
        }
        /// <summary>
        /// override to provide the format string used to format the string in the text box
        /// </summary>
        internal virtual string FormatString
        {
            get
            {
                if (typeof(T).Equals(typeof(int)))
                {
                    return "";
                }
                else if (typeof(T).Equals(typeof(decimal)))
                {
                    return $"F{Decimals}";
                }
                else if (typeof(T).Equals(typeof(double)))
                {
                    return $"N{Decimals}";
                }
                else
                {
                    throw new Exception("T expected int,decimal and double");
                }
            }

        }
        /// <summary>
        /// Property changed event
        /// </summary>

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.  
        // The CallerMemberName attribute that is applied to the optional propertyName  
        // parameter causes the property name of the caller to be substituted as an argument.  
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        /// <summary>
        /// create an instance of the <c>IncrementCommand</c> for the increment button in UI 
        /// </summary>
        //internal IncrementCommand<T> IncrementCommand => new IncrementCommand<T>(this);
        /// <summary>
        /// create an instance of the <c>DecrementCommand</c> for the increment button in UI 
        /// </summary>
        //internal DecrementCommmand<T> DecrementCommand => new DecrementCommmand<T>(this);

        /// <summary>
        /// filters the signed decimals
        /// </summary>
        internal Regex signedDecimal = new Regex(@"^(-?)([0-9]*)(\.?)([0-9]*)$"); 
        /// <summary>
        /// filters the unsigned decimals
        /// </summary>
        internal Regex unSignedDecimal = new Regex(@"^[0-9]*(\.?)[0-9]*$");
        /// <summary>
        /// filters the signed integers
        /// </summary>
        internal Regex signedInteger = new Regex(@"^-?[0-9]+$");
        /// <summary>
        /// filters the unsigned integers
        /// </summary>
        internal Regex unSignedInteger = new Regex(@"^[0-9]+$");


    }
}
