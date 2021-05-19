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
    public interface IGHCustomControl
    {
        string Name { get; set; }
        string Description { get; set; }

        RectangleF Bounds { get; set; }

        int Offset { get; }

        bool Enabled { get; set; }

        Bitmap ToolTipDiagram { get; set; }

        /// <summary>
        /// Update the graphics of the control
        /// </summary>
        void Refresh();
    }
}
