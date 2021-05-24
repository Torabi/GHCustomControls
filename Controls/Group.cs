using GH_IO.Types;
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
    public class Group :  GHParameter ,IGHPanel
    {
        RectangleF _bounds;
        /// <summary>
        /// the rectangle for the collapse and open toggle 
        /// </summary>
        RectangleF _switch;
        /// <summary>
        /// the height of the group title
        /// </summary>
        int _titleHeight;
        /// <summary>
        /// icon , if not null
        /// </summary>
        Image _icon16x16;
        bool Collapsed
        {
            get => (bool)CurrentValue;
            set
            {
                CurrentValue = value;
            }
        }
       
        //internal IGHCustomControl[] Items { get; set; }

        public override  RectangleF Bounds { 
            get =>  _bounds;
            set 
            {
                _bounds = value;
                // updatet the children bounds
                this.SetChildrenBounds(0,_titleHeight);
                //float bottom = Offset+ GH_FontServer.MeasureString(Name, GH_FontServer.StandardItalic).Height;
                //foreach(IGHCustomControl control in Members)
                //{
                //    int h = control.GetHeight();
                //    control.Bounds = new RectangleF(Offset+_bounds.Left,_bounds.Top+ bottom, _bounds.Width - 2 * Offset,h );
                //    bottom +=h + Offset;
                //}
            } 
        }
        

        public override int Offset => 2;
 


        #region IGHPanel interface 
        public GHControl[] Items { get ; set ; }

        public Orientation Orientation { get; set; } = Orientation.Vecrtical;



        #endregion
        public override GH_Types DataType => GH_Types.gh_bool;

        public override GH_ParamAccess Access => GH_ParamAccess.item;

        

        public Group (string name, string description,Image icon16x16 , Bitmap toolTipDiagram , params GHControl[] members):base(name,description, true,  toolTipDiagram )
        {
            Size size = GH_FontServer.MeasureString(name, GH_FontServer.StandardItalic);
            _titleHeight = Math.Max(size.Height, 16) + 2 * Offset+5;
            _icon16x16 = icon16x16;
            this.Items = members;
         
        }
        public Group(string name, string description) : base(name, description, true, null)
        {
            Size size = GH_FontServer.MeasureString(name, GH_FontServer.StandardItalic);
            _titleHeight = Math.Max(size.Height, 16) + 2 * Offset + 5;
            _icon16x16 = null;
            this.Items = new GHControl[0];

        }


        public void Add ( params GHControl[] items)
        {
            foreach (GHControl item in items)
                item.attributes = attributes;

            Items = Items.Concat(items).ToArray();
        }

        

        internal override int GetHeight()
        {

            if (Items.Length == 0)
                return 0;
            if (Collapsed)
                return _titleHeight;
            else
                return _titleHeight + this.GetChildrenHeight(Items);
            //foreach(IGHCustomControl control in Items)
            //{
            //    h += Offset + control.GetHeight();
            //}
            //return h;
        }

        internal override int GetWidth()
        {
            int w1 = 16+GH_FontServer.StringWidth(Name, GH_FontServer.StandardItalic)+ 2 * Offset + (_icon16x16==null?0:16) ;
            int w2 = 16+this.GetChildrenWidth(Items);
            return Math.Max(w1, w2);
        }

        #region Mouse events

        internal override void MouseLeftClick(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
 
            if (_switch.Contains(e.CanvasLocation))
            {
                Collapsed = !Collapsed;
                
                result = result | GHMouseEventResult.Invalidated | GHMouseEventResult.Handled;

                attributes?.Redraw();
                return;
            }
            // pass the clicl events to the children
            this.MouseClickChildren(sender, customComponent, e, ref result);



        }
        internal override void MouseRightClick(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
 
            this.MouseClickChildren( sender, customComponent, e, ref result);
        }
        //internal override void MouseLeave(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        //{
        //    base.MouseLeave(sender, customComponent, e, ref result);
        //    //foreach (GHCustomControl control in Items)
        //    //{
        //    //    //if (control.Bounds.Contains(e.CanvasLocation))
        //    //    //{
        //    //        control.MouseOver(sender, customComponent, e, ref result);
                    
        //    //    //}
        //    //}  
        //    this.MouseOverChildren( sender, customComponent, e, ref result);
        //}

        //internal override void MouseOver(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        //{

        //    base.MouseOver(sender, customComponent, e, ref result);
        //    //if (_switch.Contains(e.CanvasLocation))
        //    //{
        //    //    // highlight the collapse button
        //    //    if (Highlighted!=0)
        //    //    {
        //    //        Highlighted = 0;
        //    //        result = result | GHMouseEventResult.Invalidated;
                  
        //    //    }
        //    //    return;
        //    //}
        //    //else
        //    //{
        //    //    if (Highlighted==0)
        //    //    {
        //    //        Highlighted = -1;
        //    //        result = result | GHMouseEventResult.Invalidated;
                    
        //    //    }
        //    //}
        //    this.MouseOverChildren( sender, customComponent, e, ref result);
        //    //foreach (IGHCustomControl control in Items)
        //    //{
        //    //    if (control.Bounds.Contains(e.CanvasLocation))
        //    //    {
        //    //        control.MouseOver(customComponent, e, ref result);
        //    //    }
        //    //}
        //}
        internal override RectangleF ActiveZone => _switch;
        internal override void SetupToolTip(PointF canvasPoint, GH_TooltipDisplayEventArgs e)
        {
             
            if (this.SetChildrenToolTip(canvasPoint, e))
                return;
            base.SetupToolTip(canvasPoint, e);
        }
        #endregion
        internal override void Render(Graphics graphics, PointF cursorCanvasPosition, bool selected, bool locked, bool hidden)
        {
            if (Items.Length == 0)
                return;
            bool Highlighted = _switch.Contains(cursorCanvasPosition);
            GH_Capsule capsule = GH_Capsule.CreateCapsule(Bounds, GH_Palette.Transparent, 5, 2);
            capsule.Render(graphics, selected, !Enabled, hidden);
            _switch = new RectangleF(capsule.Box.Left + 5, capsule.Box.Top + 5+2, 16, 16);
            graphics.FillRectangle( 
                (locked)?Brushes.Gray:(Highlighted)?Brushes.White:Brushes.Black, _switch
                );
            using (StringFormat s = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.None
            })
            {
                graphics.DrawString(
                    (Collapsed) ? "+" : "-",
                    GH_FontServer.StandardBold,
                    (Highlighted) ? Brushes.Black : Brushes.White,
                    _switch,
                    s
                    );
            }
            using (StringFormat s = new StringFormat()
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Near,
                Trimming = StringTrimming.EllipsisCharacter
            })
            {
                graphics.DrawString(
                    Name,
                    GH_FontServer.StandardItalic,
                    Brushes.Black,
                    capsule.Box.Left + capsule.MaxRadius + _switch.Width + 2,
                    capsule.Box.Top + capsule.MaxRadius,
                    s);
            }
            if (_icon16x16 != null)
            {
                graphics.DrawImage(_icon16x16, Bounds.Right - Offset-_icon16x16.Width, Bounds.Top + Offset+5);
            }
            capsule.Dispose();
            if (!Collapsed)
            {
                foreach (GHControl control in Items)
                    if (control.IsVisible)
                    control.Render(graphics,  cursorCanvasPosition, selected, locked, hidden);
            }
        }

      

        /// <summary>
        /// find a member in the group or sub-groups by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public void FindMember(string name, ref GHControl control)
        {
            foreach(GHControl m in Items)
            {
                if (m.Name == name)
                {
                    control = m;
                    return;
                }
                if (m is Group)
                {
                    ((Group)m).FindMember(name, ref control);
                }
            }
        }

        
    }
}
