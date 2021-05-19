using GH_IO.Serialization;
using GH_IO.Types;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Inheriate from this calss to use custom controls in GH component
    /// </summary>
    public abstract class GHCustomComponent  : GH_Component
    {
        
        protected GHCustomComponent(string name,string nickname,string description,string category,string subCategory) : base(name,nickname,description,category,subCategory) { }
        public Dictionary<string, GHControl> CustomControls = new Dictionary<string, GHControl>();

        public void AddCustomControl(GHControl customControl)
        {
            if (customControl != null)
            {
                // check for prexisting data ;
                
                CustomControls.Add(customControl.Name, customControl);
                customControl.SetAttribute((GHCustomAttributes)m_attributes);

                
            }
        }
 

        public override void CreateAttributes()
        {
            m_attributes = new GHCustomAttributes(this);
        }

        public GHControl GetControl(params string[] path)
        {
             
            GHControl ct = CustomControls[path[0]];
            if (path.Length == 1)
                return ct;
            if (ct is IGHPanel)
            {
                return ((IGHPanel)ct).GetControl(path.Skip(1));
            }

            throw new Exception("Invalid path.");
             
        }


        /// <summary>
        /// retrive the value of the custom control by name.
        /// if the path is invalid or the control is not a parameter then thorws an exception.
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">the current value of the control</param>
        /// <param name="path">path to the control</param>
        /// <returns></returns>
        public void GetControlData<T>(ref T value,params string[] path)
        {
            GHControl control = GetControl(path);
            
            
            if (control is GHParameter)
            {
                GHParameter param = (GHParameter)control;
                value = (T)param.CurrentValue;
                return ;

            }
            
            throw new Exception("Invalid path to control");
            
        }

        #region Serialization and Deserialization
        private void writeCustomControls(GH_IWriter writer, IEnumerable<GHControl> customControls, string path)
        {
            foreach (GHControl control in customControls)
            {
                string name = (path.Length > 0) ? (path + "." + control.Name): control.Name;
                if (control is GHParameter)
                {
                    GHParameter param = (GHParameter)control;
                    switch (param.DataType)
                    {
                        case GH_Types.gh_bool:
                            writer.SetBoolean(name, (bool)param.CurrentValue);
                            break;
                        case GH_Types.gh_double:
                            writer.SetDouble(name, (double)param.CurrentValue);
                            break;
                        case GH_Types.gh_int32:
                            writer.SetInt32(name, (int)param.CurrentValue);
                            break;
                        case GH_Types.gh_decimal:
                            writer.SetDecimal(name, Convert.ToDecimal( param.CurrentValue));
                            break;
                    }
                } 
                if (control is IGHPanel)
                {
                     
                       
                    writeCustomControls(writer, ((IGHPanel)control).Items,name);
                }
            }
        }
        public override bool Write(GH_IWriter writer)
        {
            writeCustomControls(writer, CustomControls.Values,"");
            return base.Write(writer);
        }

        private void readCustomControls(GH_IReader reader, IEnumerable<GHControl> customControls, string path)
        {
            foreach (GHControl control in customControls)
            {
                string name = (path.Length > 0) ? (path + "." + control.Name) : control.Name;
               
                if (control is GHParameter)
                {
                    if (reader.FindItem(name) == null)
                        continue;
                    GHParameter param = (GHParameter)control;
                    switch (param.DataType)
                    {
                        case GH_Types.gh_bool:

                            param.CurrentValue = reader.GetBoolean(name);
                            break;
                        case GH_Types.gh_double:
                            param.CurrentValue = reader.GetDouble(name);
                            break;
                        case GH_Types.gh_int32:
                            param.CurrentValue = reader.GetInt32(name);
                            break;
                        case GH_Types.gh_decimal:
                            param.CurrentValue = (float)reader.GetDecimal(name);
                            break;
                    }
                    

                }

                if (control is IGHPanel)
                {
                    readCustomControls(reader, ((IGHPanel)control).Items, name);
                }
                
            }
        }

        public override bool Read(GH_IReader reader)
        {
            readCustomControls(reader , CustomControls.Values,"");
 
            return base.Read(reader);
        }

        #endregion


    }
}
