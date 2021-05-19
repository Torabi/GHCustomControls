using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
 
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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

    public class GHCustomAttributes  : GH_ComponentAttributes 
    {
        /// <summary>
        /// the amount of the offset for the bounds of this componenet from the largest sub-control 
        /// </summary>
        float _offset = 2;
        /// <summary>
        /// false when expireLayout() is called , true when Layout() is called 
        /// this is to avoid multiple call to Layout()
        /// </summary>
        bool _layout;

        public GHCustomAttributes(GHCustomComponent owner) : base(owner) 
        { 
        
        }

        /// <summary>
        /// redraw the layout
        /// </summary>
        internal void Redraw()
        {
            if (_layout)
            {
                _layout = false;
                ExpireLayout();
            }

        }

        #region Custom layout logic


        protected override void Layout()
        {
            _layout = true;
            // draw basic layout
            base.Layout();
            PointF p = Pivot;
            RectangleF b = Bounds;
            GHCustomComponent owner = Owner as GHCustomComponent;
            if (owner.CustomControls.Values.Count == 0)
                return;
            // we add the custom constrols to the buttom of each other in the same order they have been added to the dictionary 
            // the value buttom records the buttom postion of the controls as they are being added
            float buttom = Bounds.Bottom;
            
            // to find the overal widht of the component we find the maximum width of the customcontrols
            float maxWidth = Bounds.Width;
            foreach (GHControl item in owner.CustomControls.Values)
            {
                float w = (item.IsVisible)? item.GetWidth():0;
                if (maxWidth < w)
                    maxWidth = w;
            }

            foreach (GHControl item in owner.CustomControls.Values)
            {
                
                float h = (item.IsVisible)? item.GetHeight():0;
                // set the item bounds , we expand the bounds by 2 units from each side 
                item.Bounds = new RectangleF(Bounds.X, buttom, maxWidth, h);
                buttom += h+_offset; // update the buttom value
             
            }
            float currentWidth = Bounds.Width;
            var corner = new PointF(Bounds.X - _offset, Bounds.Y - _offset);
            Bounds = new RectangleF(corner, new SizeF(maxWidth + 2 * _offset, Bounds.Height + buttom - Bounds.Bottom + 2 * _offset));
            
            if (maxWidth > currentWidth)
            {
            //    // update the output parameter layout   
            //    int paramWidth = Owner.Params.Output.Max(
            //        p => GH_FontServer.StringWidth(
            //            (CentralSettings.CanvasFullNames) ? p.Name : p.NickName
            //            , SmallFont
            //            )
            //        );

                //LayoutOutputParams(Owner, new RectangleF(Bounds.X , Bounds.Y, maxWidth, Bounds.Height));

                foreach (var param in Owner.Params.Output)
                {
                    RectangleF rec = param.Attributes.Bounds;
                    rec.Offset(Bounds.Right - param.Attributes.Bounds.Right, 0);
                    param.Attributes.Bounds = rec;
                }
            }

           
            
            
            //Pivot = new PointF((Bounds.X + Bounds.Width) / 2 -12, Pivot.Y);
            
        }

        
        #endregion

        #region Custom Mouse handling
        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            
            GHCustomComponent owner = Owner as GHCustomComponent;
            if (Bounds.Contains(e.CanvasLocation))
            {
                // click is inside the component 
                // check which control is clicked
                GHMouseEventResult result =  GHMouseEventResult.None;
                foreach (GHControl control in owner.CustomControls.Values)
                {
                    if (control.Bounds.Contains(e.CanvasLocation))
                    {
                         
                        // click happened in this control 
                        if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    
                            control.MouseLeftClick( sender, owner, e, ref result);
                        else if (e.Button == System.Windows.Forms.MouseButtons.Right)
                            control.MouseRightClick( sender, owner, e, ref result);
                        if (control is GHParameter && !((GHParameter)control).UpdateSolution)
                        {
                            result &= ~GHMouseEventResult.UpdateSolution;
                        }
                         
                    }
                }
                if (result.HasFlag(GHMouseEventResult.Invalidated))
                    sender.Invalidate();
                if (result.HasFlag(GHMouseEventResult.UpdateSolution))
                    owner.ExpireSolution(true);
                if (result.HasFlag(GHMouseEventResult.Handled))
                    return GH_ObjectResponse.Handled;
            }
            
            return base.RespondToMouseDown(sender, e);
        }
        
        public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            GHCustomComponent owner = Owner as GHCustomComponent;
            GHMouseEventResult result =  GHMouseEventResult.None;
            var backup = sender.Cursor;
            if (Bounds.Contains(e.CanvasLocation))
            {
                foreach (GHControl item in owner.CustomControls.Values)
                {
                    if (!item.Enabled)
                        continue;
                    if (item.Bounds.Contains(e.CanvasLocation))
                    {
                        item.MouseOver( sender, owner, e, ref result);

                    }
                    else
                    {
                        item.MouseLeave( sender, owner, e, ref result);
                    }
                    if (item is GHParameter && !((GHParameter)item).UpdateSolution)
                    {
                        result &= ~GHMouseEventResult.UpdateSolution;
                    }
                }
                
                if (result.HasFlag(GHMouseEventResult.Invalidated))
                    sender.Invalidate();
                if (result.HasFlag(GHMouseEventResult.UpdateSolution))
                    owner.ExpireSolution(true);
               
                
                if (result.HasFlag(GHMouseEventResult.Handled))
                    return GH_ObjectResponse.Ignore;
                


            }
             return base.RespondToMouseMove(sender, e);
            
                

            
        }

        public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            GHCustomComponent owner = Owner as GHCustomComponent;
            GHMouseEventResult result = GHMouseEventResult.None;
            var backup = sender.Cursor;
            if (Bounds.Contains(e.CanvasLocation))
            {
                foreach (GHControl item in owner.CustomControls.Values)
                {
                    if (!item.Enabled)
                        continue;
                    if (item.Bounds.Contains(e.CanvasLocation))
                    {
                        item.MouseKeyUp( sender, owner, e, ref result);
                        if (item is GHParameter && !((GHParameter)item).UpdateSolution)
                        {
                            result &= ~GHMouseEventResult.UpdateSolution;
                        }
                        if (result.HasFlag(GHMouseEventResult.Handled))
                            break;
                    }

                }

                if (result.HasFlag(GHMouseEventResult.Invalidated))
                    sender.Invalidate();
                
                if (result.HasFlag(GHMouseEventResult.UpdateSolution) )
                    owner.ExpireSolution(true);
                

                if (result.HasFlag(GHMouseEventResult.Handled))
                    return GH_ObjectResponse.Handled;



            }
            return base.RespondToMouseUp(sender, e);
        }

        public override void SetupTooltip(PointF canvasPoint, GH_TooltipDisplayEventArgs e)
        {
            GHCustomComponent owner = Owner as GHCustomComponent;
            foreach (GHControl item in owner.CustomControls.Values)
            {
                if (item.Bounds.Contains(canvasPoint) && item.Enabled)
                {
                    item.SetupToolTip(canvasPoint,e);
                    return;
                }
            }
            base.SetupTooltip(canvasPoint, e);
        }

        public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (Bounds.Contains(e.CanvasLocation))
            {
                return GH_ObjectResponse.Ignore;
            }
            return base.RespondToMouseDoubleClick(sender, e);
        }


        #endregion

        #region Custom Render logic
        protected override void Render(GH_Canvas canvas, System.Drawing.Graphics graphics, GH_CanvasChannel channel)
        {
            switch (channel)
            {
                case GH_CanvasChannel.Objects:
                    //We need to draw everything ourselves.
                    base.RenderComponentCapsule(canvas, graphics, true, true, false, true, true,true);
                    //if (Owner.IconDisplayMode == GH_IconDisplayMode.icon)
                    //{
                    //    PointF iconPos = new PointF((Bounds.Left + Bounds.Right) / 2 - 12, Pivot.Y);
                    //    graphics.DrawImage(Owner.Icon_24x24, new RectangleF(iconPos, new Size(24,24)));
                    //}
                    if (canvas.Viewport.Zoom < 0.5)
                    {
                        return;
                    }
                    GHCustomComponent owner = Owner as GHCustomComponent;
                   
                    foreach (GHControl item in owner.CustomControls.Values)
                    {
                        //if (!item.Bounds.Contains(canvas.CursorCanvasPosition))
                        //    item.MouseLeave(owner,new GH_CanvasMouseEvent(),ref invalidate);
                        
                        if (item.IsVisible)
                            item.Render(graphics, canvas.CursorCanvasPosition, this.Selected, Owner.Locked || !item.Enabled, Owner.Hidden);
                    }

                   
                    break;
                default:
                    base.Render(canvas, graphics, channel);
                    break;
            }
        }
        #endregion


        #region helpers
        
        #endregion
    }
}
