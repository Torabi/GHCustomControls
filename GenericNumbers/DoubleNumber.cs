using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
    /// this class represent a formatted double number with constrains
    /// </summary>
    public class DoubleNumber : GenericNumber<double>
    {
        /// <summary>
        /// construct a constrained double number
        /// </summary>
        /// <param name="value"><inheritdoc/></param>
        /// <param name="min"><inheritdoc/> </param>
        /// <param name="max"><inheritdoc/></param>
        /// <param name="inc"><inheritdoc/></param>
        /// <param name="decimals"><inheritdoc/></param>
        public DoubleNumber(double value,double min,double max,double inc,int decimals) : base(value, min, max, inc,decimals) { }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        internal override Regex _regex => (Min < 0) ? signedDecimal : unSignedDecimal;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool ConvertToValue(string text, out double result)
        {
            return double.TryParse(text, out result);
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        internal override double decrement(double value)
        {
            return value - Inc;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        internal override double increment(double value)
        {
            return value + Inc;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        internal override string FormatString => $"N{Decimals}";
    }
}
