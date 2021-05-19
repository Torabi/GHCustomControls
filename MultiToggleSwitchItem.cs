using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
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
    public class MultiToggleSwitchItem : GHControl
    {

        
        public readonly int Value;
 

    
        public bool ShowTitle = false;
        readonly Image _image;
        readonly StringFormat _format;

        public bool IsSelected = false;
   
        public override RectangleF Bounds { get ; set ; }

        public override int Offset => 6;

 

        public MultiToggleSwitchItem(string name,string description,int value, Image image ):base(name,description)
        {
 
      
            Value = value;
            _image = image;
            _format = new StringFormat();
            _format.Alignment = StringAlignment.Near;
            _format.LineAlignment = StringAlignment.Near;
            _format.Trimming = StringTrimming.EllipsisCharacter;

        }
        /// <summary>
        /// Width of the item
        /// </summary>
        /// <returns></returns>
        internal override int GetWidth()
        {
            int width =ShowTitle ? GH_FontServer.StringWidth(Name, SmallFont) : 0;
            return (_image == null) ? Offset + width : Offset + _image.Width + width;
            
        }
        /// <summary>
        /// Height of the item
        /// </summary>
        /// <returns></returns>
        internal override int GetHeight()
        {
            
            return (_image == null)? 24 : 4 + _image.Height;
        }

        internal override void Render(Graphics graphics, PointF cursorCanvasPosition, bool selected, bool locked, bool hidden)
        {
            //RectangleF rec = new RectangleF(Bounds.X + Offset / 3f, Bounds.Y + Offset / 3f, Bounds.Width - 2 * Offset / 3f, Bounds.Height - 2 * Offset / 3f);
            
            if (IsSelected)
            {
                GH_Capsule capsule = GH_Capsule.CreateCapsule(Bounds, GH_Palette.Transparent, 2, 0);
                capsule.Render(graphics, 
                    (Enabled && !locked)?
                    new GH_PaletteStyle(Color.AntiqueWhite, Color.SlateGray)
                    :
                    new GH_PaletteStyle(Color.Transparent, Color.SlateGray)
                    );
                capsule.Dispose();
                //graphics.DrawRectangle(new Pen(Brushes.Black,2.5f), Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height);
                
            }else if (Bounds.Contains(cursorCanvasPosition) && Enabled )
            {
                GH_Capsule capsule = GH_Capsule.CreateCapsule(Bounds, GH_Palette.Transparent, 2, 0);
                capsule.Render(graphics, new GH_PaletteStyle(Color.Transparent));
                capsule.Dispose();
            }


            //graphics.DrawRectangle(new Pen(Brushes.Black), Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height);
            
            if (ShowTitle)
            {
                if (_image != null)
                {
                    graphics.DrawImage(_image, Bounds.X + Offset / 3.0f, Bounds.Y + Offset / 3.0f);
                    graphics.DrawString(Name, SmallFont, this.ActiveBrush(), _image.Width + Bounds.X + 2 * Offset / 3.0f, Bounds.Y + Offset / 3.0f, _format);
                }
                else
                {
                    graphics.DrawString(Name, SmallFont, this.ActiveBrush(), Bounds.X + 2 * Offset / 3.0f, Bounds.Y + Offset / 3.0f, _format);
                }
            }
            else
            {

                if (_image != null)
                    graphics.DrawImage(_image, Bounds.X+Bounds.Width/2f - _image.Width/2f, Bounds.Y + Offset / 3.0f);
                else
                    graphics.DrawString(Name, SmallFont,this.ActiveBrush(), Bounds.X + 2 * Offset / 3.0f, Bounds.Y + Offset / 3.0f, _format);
            }
            
        }
        #region mouse events
        //internal override void MouseOver(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        //{
     
        //    //if (Highlighted==0)
        //    //    return;
        //    //result = result | GHMouseEventResult.Invalidated ;
        //    //Highlighted = 0;
            


        //}

        //internal override void MouseLeave(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        //{
             
        //    //if (Highlighted!=0)
        //    //    return;
        //    //Highlighted = -1;
        //    result = result | GHMouseEventResult.Invalidated;

            
        //}

        internal override void MouseLeftClick(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
            //if (Bounds.Contains(e.CanvasLocation))
            //{
            //    // clicked on this toggle 
            //    if (IsSelected)
            //        return; // already selected nothing happens
            //    IsSelected = true;
            //    result = result | GHMouseEventResult.Handled | GHMouseEventResult.UpdateSolution;
            //}
            //else {
            //    // outside of this toggle 
            //    if (!IsSelected)
            //        return; // nothing happens
            //    IsSelected = false;
            //    result = result | GHMouseEventResult.Invalidated;

            //}
            
           

        }
        internal override void MouseRightClick(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
            
        }

        
        #endregion

    }
}
