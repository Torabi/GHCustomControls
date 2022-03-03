using System;
using System.Collections.Generic;
using System.Linq;
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
    /// a class represent a constrained integer number 
    /// </summary>
    public class IntegerNumber : GenericNumber<int>
    {
        /// <summary>
        /// construct an contrsianed integer
        /// </summary>
        /// <param name="value"><inheritdoc/></param>
        /// <param name="min"><inheritdoc/></param>
        /// <param name="max"><inheritdoc/></param>
        /// <param name="inc"><inheritdoc/></param>
        public IntegerNumber(int value,int min,int max, int inc = 1) : base(value, min, max, inc,0) { }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        internal override int increment(int value)
        {
            return value + Inc;

        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        internal override int decrement(int value)
        {
            return value - Inc;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool ConvertToValue(string text, out int result)
        {
            return int.TryParse(text, out result);
                
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        internal override Regex _regex => (Min < 0) ? signedInteger : unSignedInteger;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        internal override string FormatString => "";
    }
}
