using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;
namespace GHCustomControls
{
    /// <summary>
    /// Interaction logic for TextBoxWindow.xaml
    /// </summary>
    public partial class NumericUpDownWindow : Window
    {
        Point _location;
        float _scale;
        public NumericUpDownWindow(Point location)
        {
            InitializeComponent();
            _location = location;
            //_scale = size;
        }


        private void MoveBottomRightEdgeOfWindowToMousePosition()
        {
            var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
            //var mouse = transform.Transform(GetMousePosition());
            var pos = transform.Transform(_location);
            //var pos = _location;
            
            //this.Width *= _scale;
            //this.Height *= _scale;
            Left = pos.X - ActualWidth / 2.0;
            Top = pos.Y - ActualHeight;
        }

        public System.Windows.Point GetMousePosition()
        {
            System.Drawing.Point point = System.Windows.Forms.Control.MousePosition;
            return new System.Windows.Point(point.X, point.Y);
        }




        internal void SetData<T>(NumericUpDownData<T> numericUpDownControl) where T:struct, IFormattable, IComparable<T>
        {
            Control control;
            Binding binding = new Binding("Value");
            binding.Source = numericUpDownControl;
            if (typeof(T) == typeof(int))
            {
                var numeric = new IntegerUpDown();
                numeric.Minimum = Convert.ToInt32(numericUpDownControl._min);
                numeric.Maximum = Convert.ToInt32(numericUpDownControl._max);
                numeric.UpdateValueOnEnterKey = true;
                numeric.FormatString = numericUpDownControl.FormatString;
                numeric.SetBinding(IntegerUpDown.ValueProperty, binding);
                 
                control = numeric;
            }
            else if (typeof(T) == typeof(decimal))
            {
                var numeric = new DecimalUpDown();
                numeric.Minimum = Convert.ToDecimal(numericUpDownControl._min);
                numeric.Maximum = Convert.ToDecimal(numericUpDownControl._max);
                numeric.Increment = (numeric.Maximum - numeric.Minimum) / 100;
                numeric.UpdateValueOnEnterKey = true;
                numeric.FormatString = numericUpDownControl.FormatString;
                numeric.SetBinding(DecimalUpDown.ValueProperty, binding);
                control = numeric;
            }
            else if (typeof(T) == typeof(double))
            {
                var numeric = new DoubleUpDown();
                numeric.Minimum = Convert.ToDouble(numericUpDownControl._min);
                numeric.Maximum = Convert.ToDouble(numericUpDownControl._max);
                numeric.UpdateValueOnEnterKey = true;
                numeric.SetBinding(DoubleUpDown.ValueProperty, binding);
                numeric.FormatString = numericUpDownControl.FormatString;
                control = numeric;
            }
            else
            {
                throw new Exception($"Inavlid type, Expected int,double or decimal received {typeof(T)}");
            }
            //control.Width = 200;
            //control.Height = 24;
            control.Name = "Numeric";
            control.Background = Brushes.LightGoldenrodYellow;
            control.KeyDown += Control_KeyDown;
            
            this.PlaceHolder.Children.Add(control);

            
             

        }

        private void Control_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                DialogResult = true;
                this.Close();
            }
        }


        //object _value;
        //public object Value
        //{
        //    get => _value;
        //    set
        //    {
        //        _value = value;
        //        if (_value is int)
        //        {
        //            (control as IntegerUpDown).Value = (int)_value;
        //        }else if (_value is decimal)
        //        {
        //            (control as DecimalUpDown).Value = (decimal)_value;
        //        }
        //        else if (_value is double)
        //        {
        //            (control as DoubleUpDown).Value = (double)_value;
        //        }
        //    }
        //}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //_value = (int)this.control.Value;
            DialogResult = true;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MoveBottomRightEdgeOfWindowToMousePosition();
        }

        

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender != null)
            {
                Window w = sender as Window;
                
                if (w.IsVisible)
                    w.Close();
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape )
            {
                Window w = sender as Window;
                if (w.IsVisible)
                    w.Close();
            }
        }
    }
}
