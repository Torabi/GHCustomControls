using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
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
    public class PictureFrame : GHControl
    {
        Image _image;
        Size _size;
        /// <summary>
        /// render the image in actual size
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="image"></param>
        public PictureFrame(string name,string description,Image image):base(name,description)
        {
    
            _image = image;
            _size = image.Size;
        }
        /// <summary>
        /// fit the image to the given size 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="image"></param>
        /// <param name="size"></param>
        public PictureFrame(string name, string description, Image image,Size size):base(name,description)
        {
            
            _image = image;
            _size = size;
        }
        /// <summary>
        /// fit the image to the given width
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="image"></param>
        /// <param name="width"></param>
        public PictureFrame(string name, string description, Image image, int width):base(name,description)
        {
           
            _image = image;
            _size = new Size(width, _image.Height*width/ _image.Width);
        }

        public override RectangleF Bounds {get; set; }
       

        public override int Offset => 5;

       
        internal override int GetHeight()
        {
            return _size.Height+2*Offset;
        }

        internal override int GetWidth()
        {
            return _size.Width+2*Offset;
        }
        #region mouse events
        public override void MouseLeave(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
             
        }

        internal override void MouseLeftClick(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
            
        }

        public override void MouseOver(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
            
        }

        internal override void MouseRightClick(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
             
        }
        #endregion
        internal override void Render(Graphics graphics, PointF cursorCanvasPosition, bool selected, bool locked, bool hidden)
        {
            Helpers.DrawFrame(Bounds, graphics, "", selected, locked, hidden);
            graphics.DrawImage(_image, Bounds.Offset(Offset).Center(_size));
        }

        
    }
}
