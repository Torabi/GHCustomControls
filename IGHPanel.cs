using Grasshopper.GUI;
using Grasshopper.Kernel;
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
    /// <summary>
    /// implement this interface for the controls with sub-items, like group, tabs ...
    /// </summary>
    public interface IGHPanel : IGHCustomControl
    {
        

        /// <summary>
        /// array of the 
        /// </summary>
        GHControl[] Items { get; set; }

        /// <summary>
        /// The orinetation in which the childrens are placed 
        /// </summary>
        Orientation Orientation { get; set; }


    
    }
}
