using GH_IO.Serialization;
using GH_IO.Types;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
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
    public abstract class GHParameter : GHControl
    {
        protected GHParameter(string name,string description, object defaultValue,Bitmap toolTipDiagram) :base(name,description, toolTipDiagram)
        {
            CurrentValue = defaultValue;
        }
        /// <summary>
        /// type of the data stored in parameter
        /// </summary>
        public abstract GH_Types DataType { get;  }
        

        public abstract GH_ParamAccess Access { get;  }

        

        internal object _currentValue; 
        /// <summary>
        /// the value stored in the parameter
        /// </summary>
        public object CurrentValue {
            get => _currentValue;
            set {
                _currentValue = value;
                OnValueChanged?.Invoke(this,new ValueChangeEventArgumnet(_currentValue,DataType,Access));
                }
        }

        public event EventHandler<ValueChangeEventArgumnet> OnValueChanged;

      

     
    }

    public class ValueChangeEventArgumnet : EventArgs
    {
        
        
        public GH_Types DataType { get; set; }
        public GH_ParamAccess Access { get; set; }
        public object Value { get; set; }
        public ValueChangeEventArgumnet(object value, GH_Types dataType, GH_ParamAccess access)
        {
            Value = value;
            DataType = dataType;
            Access = access;
        }
    }
}
