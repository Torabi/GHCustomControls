using GH_IO.Serialization;
using GH_IO.Types;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
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
                    switch (param.Access)
                    {
                        case GH_ParamAccess.item:
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
                                    writer.SetDecimal(name, Convert.ToDecimal(param.CurrentValue));
                                    break;

                                case GH_Types.gh_point2d:
                                    writer.SetPoint2D(name, (GH_Point2D)param.CurrentValue);
                                    break;
                            }
                            break;

                        case GH_ParamAccess.list:
                            switch (param.DataType)
                            {
                                case GH_Types.gh_bool:
                                    writer.SetByteArray(name, ((bool[])param.CurrentValue).Select(i=> (byte)(i? 1:0)).ToArray());
                                    break;
                                case GH_Types.gh_double:
                                    writer.SetDoubleArray(name, (double[])param.CurrentValue);
                                    break;
                                case GH_Types.gh_int32:
                                    writer.SetDoubleArray(name, ((int[])param.CurrentValue).Cast<double>().ToArray());
                                    break;
                                case GH_Types.gh_decimal:
                                    writer.SetDoubleArray(name, ((float[])param.CurrentValue).Cast<double>().ToArray());
                                    break;
                                case GH_Types.gh_point2d:
                                    writer.SetDoubleArray(name, ((GH_Point2D[])param.CurrentValue).SelectMany(p => new double[] { p.x, p.y }).ToArray());
                                    //writer.SetDoubleArray(name + ".y", ((GH_Point2D[])param.CurrentValue).Select(p => p.y).ToArray());
                                    break ;
                            }
                            break;
                    }
                    
                } 
                if (control is IGHPanel)
                {
                     
                    if (control is TabPanel tabPanel)
                    {
                        foreach(var kpv in tabPanel._tabs)
                        {
                            
                                writeCustomControls(writer, kpv.Value,  name+"."+kpv.Key);
                             
                        }
                    }
                    else
                    {
                        writeCustomControls(writer, ((IGHPanel)control).Items,name);
                    }
                    
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
                    switch(param.Access)
                    {
                        case GH_ParamAccess.item:
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
                                case GH_Types.gh_point2d:
                                    param.CurrentValue = reader.GetPoint2D(name);
                                    break;

                            }
                            break;
                        case GH_ParamAccess.list:
                            switch (param.DataType)
                            {
                                case GH_Types.gh_bool:

                                    param.CurrentValue = reader.GetByteArray(name).Select(b=>b==1).ToArray();
                                    break;
                                case GH_Types.gh_double:
                                    param.CurrentValue = reader.GetDoubleArray(name);
                                    break;
                                case GH_Types.gh_int32:
                                    param.CurrentValue = reader.GetDoubleArray(name).Select(d=>(int)d).ToArray();
                                    break;
                                case GH_Types.gh_decimal:
                                    param.CurrentValue =  reader.GetDoubleArray(name).Select(d => (float)d).ToArray();
                                    break;
                                case GH_Types.gh_point2d:

                                    var values = reader.GetDoubleArray(name);
                                    var points = new GH_Point2D[values.Length / 2];
                                    for (int i = 0; i < values.Length; i += 2)
                                        points[i/2] = new GH_Point2D(values[i], values[i + 1]);
                                    param.CurrentValue = points;
                                    break;

                            }
                            break;
                    }
                    
                    

                }

                if (control is IGHPanel)
                {

                    if (control is TabPanel tabPanel)
                    {
                        foreach (var kpv in tabPanel._tabs)
                        {

                            readCustomControls(reader, kpv.Value, name + "." + kpv.Key);

                        }
                    }
                    else
                    {
                        readCustomControls(reader, ((IGHPanel)control).Items, name);
                    }

                    //readCustomControls(reader, ((IGHPanel)control).Items, name);
                }
                
            }
        }

        public override bool Read(GH_IReader reader)
        {
            readCustomControls(reader , CustomControls.Values,"");
 
            return base.Read(reader);
        }


        #endregion


        #region Helper methods 
        /// <summary>
        /// Rerun an image by its path.
        /// </summary>
        /// <param name="path">Path to your image : FLODER.IMAGENAME</param>
        /// <returns></returns>
        public Image GetImage(string path)
        {

            Assembly myAssembly = this.GetType().Assembly;
            string title = myAssembly.GetCustomAttribute<AssemblyTitleAttribute>().Title;
            Stream myStream = myAssembly.GetManifestResourceStream($"{title}.{path}");
            if (myStream == null)
                return null;
            return Image.FromStream(myStream);
        }
        /// <summary>
        /// Return an icon by its path. 
        /// </summary>
        /// <param name="path">Path to your icon : FLODER.ICONNAME</param>
        /// <returns></returns>
        public Bitmap GetBitmap(string path)
        {

            Assembly myAssembly = this.GetType().Assembly;
            string title = myAssembly.GetCustomAttribute<AssemblyTitleAttribute>().Title;
            Stream myStream = myAssembly.GetManifestResourceStream($"{title}.{path}" );
            if (myStream == null)
                return null;
            return new Bitmap(myStream);
        }

        /// <summary>
        /// converts the given value from millimeter to internal units
        /// </summary>
        /// <param name="mm"></param>
        /// <returns></returns>
        public static double ToInternalUnits(double mm)
        {

            return RhinoMath.UnitScale(UnitSystem.Millimeters, Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem) * mm;
        }
        /// <summary>
        /// return the number of decimals which at least can represent millimiter in any unit system
        /// </summary>
        /// <returns></returns>
        public static int NumDecimals()
        {
            return Math.Max(Convert.ToInt32(-Math.Log10(ToInternalUnits(1))), 0);
        }
        #endregion

    }
}
