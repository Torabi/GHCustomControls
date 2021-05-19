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
    public static class IGHPanelExtensions
    {
        public static void SetChildrenBounds(this IGHPanel gHContainer,int offsetX,int offsetY,ContentAlignment contentAlignment = ContentAlignment.MiddleCenter ) 
        {
            int _offset = gHContainer.Offset;
            RectangleF _bounds = gHContainer.Bounds;
            float bottom = _offset ;
            float left = _offset;
            
            if (gHContainer.Orientation == Orientation.Vecrtical)
            {
                foreach (GHControl control in gHContainer.Items)
                {
                    int h =(control.IsVisible)? control.GetHeight():0;
                    int groupShift = (control is Group) ? 18 : 0;
                    control.Bounds = new RectangleF(left + _bounds.Left+ offsetX+groupShift, _bounds.Top + bottom + offsetY, _bounds.Width - 2 * _offset-offsetX- groupShift, h);
                    bottom += h + _offset;
                }
            }
            else
            {
                int[] widths = new int[gHContainer.Items.Length];
                int netWidth = 0;
                for (int i = 0;i< widths.Length;i++)
                {
                    netWidth += widths[i] = (gHContainer.Items[i].IsVisible)? gHContainer.Items[i].GetWidth():0;
                    
                }

                var _gap = 0f;
                if (contentAlignment == ContentAlignment.MiddleCenter || contentAlignment == ContentAlignment.BottomCenter || contentAlignment == ContentAlignment.TopCenter  )
                {
                    _gap = (_bounds.Width - offsetX - netWidth) / (widths.Length + 1);
                    left = _gap;
                }
                else if (contentAlignment == ContentAlignment.MiddleLeft || contentAlignment == ContentAlignment.BottomLeft || contentAlignment == ContentAlignment.TopLeft)
                {
                    _gap = 0;
                    left = 0;
                }
                else
                {
                    _gap = 0;
                    left = _bounds.Width - offsetX - netWidth;
                }
                
                for (int i = 0; i < widths.Length; i++)
                {
                    int w = widths[i];
                    gHContainer.Items[i].Bounds = new RectangleF(left + _bounds.Left+ offsetX, _bounds.Top + bottom + offsetY, w, _bounds.Height - 2 * _offset);
                    left += w + _gap;
                }
            }
        }
        /// <summary>
        /// for vertical panel calculates the sum of the height of all children plus the offset amount.
        /// for horizontal panel calculates the maximum height among the children
        /// </summary>
        /// <param name="gHContainer"></param>
        /// <returns></returns>
        public static int GetChildrenHeight (this IGHPanel gHContainer, IEnumerable<GHControl> items)  
        {
            int offset = gHContainer.Offset;
            if (gHContainer.Orientation == Orientation.Vecrtical)
            {
               
                int h = offset;
            
                foreach (GHControl control in items)
                {
                    h += offset + (control.IsVisible?control.GetHeight():0);
                }
                return h;
            }
            else
            {
                return items.Any()?items.Max(c => c.IsVisible?c.GetHeight():0) + 2 * offset:0;
            }
            
        }
        /// <summary>
        /// for vertical panel calculates the maximum widh of all chilkdren.
        /// for horizontal panel calculates sum of the width plus the offset between them
        /// </summary>
        /// <param name="gHContainer"></param>
        /// <returns></returns>
        public static int GetChildrenWidth(this IGHPanel gHContainer, IEnumerable<GHControl> items) 
        {
            int offset = gHContainer.Offset;
            if (gHContainer.Orientation == Orientation.Horizontal)
            {

                int w = offset;

                foreach (GHControl control in items)
                {
                    w += offset + (control.IsVisible?control.GetWidth():0);
                }
                return w;
            }
            else
            {
                return items.Any()?items.Max(c => c.IsVisible?c.GetWidth():0) + 2 * offset:0;
            }

        }
        /// <summary>
        /// finds the children in which the cursor is inside and then call the MouseClick method for that child
        /// </summary>
        /// <param name="gHContainer"></param>
        /// <param name="customComponent"></param>
        /// <param name="e"></param>
        /// <param name="result"></param>
        public static void MouseClickChildren (this IGHPanel gHContainer,GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)  
        {
             
            foreach (GHControl control in gHContainer.Items)
            {
                if (control.Enabled && control.Bounds.Contains(e.CanvasLocation))
                {
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                        control.MouseLeftClick( sender, customComponent, e, ref result);
                    else
                        control.MouseRightClick(sender,customComponent, e, ref result);
                    if (control is GHParameter && !((GHParameter)control).UpdateSolution)
                    {
                        result &= ~GHMouseEventResult.UpdateSolution;
                    }
                    if (result.HasFlag(GHMouseEventResult.Handled))
                    {
                        return;
                    }
                }
            }
        }
        /// <summary>
        /// finds the children in which the cursor is inside and then call the MouseOver method for that child
        /// </summary>
        /// <param name="gHContainer"></param>
        /// <param name="customComponent"></param>
        /// <param name="e"></param>
        /// <param name="result"></param>
        public static void MouseOverChildren(this IGHPanel gHContainer, GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result) 
        {
             
            foreach (GHControl control in gHContainer.Items)
            {
                if (control.Bounds.Contains(e.CanvasLocation) && control.Enabled )
                {
                    control.MouseOver( sender, customComponent, e, ref result);
                    if (result.HasFlag(GHMouseEventResult.Invalidated)) // no need to continue as this child already set the flag 
                        return;
                }
                else
                {
                    control.MouseLeave( sender, customComponent, e, ref result);
                    //if (result.HasFlag(GHMouseEventResult.Invalidated)) // no need to continue as this child already set the flag 
                    //    return;
                }
            }
        }
        /// <summary>
        /// finds the children in which the cursor is inside and then set the too tip accordingly and return true.
        /// If no child is active then return false
        /// </summary>
        /// <param name="canvasPoint"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool SetChildrenToolTip(this IGHPanel gHContainer, PointF canvasPoint, GH_TooltipDisplayEventArgs e) 
        {
            
            foreach (GHControl control in gHContainer.Items)
            {
                if (control.Enabled && control.Bounds.Contains(canvasPoint))
                {
                    control.SetupToolTip(canvasPoint, e);
                    return true;
                }
            }
            return false;
                
        }
        /// <summary>
        /// get access to the child control in the panel and its children by set of the names (path).
        /// The path must start with a child name.
        /// throw exception if the path is not valid
        /// </summary>
        /// <param name="gHContainer"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static GHControl GetControl (this IGHPanel gHContainer,  IEnumerable<string> path) 
        {
            string name = path.First();
            GHControl ct = gHContainer.Items.FirstOrDefault(item => item.Name == name);
            if (ct == null)
                throw new Exception($"Invalid path, unable to find child control in control {gHContainer.Name}");
            if (path.Count() == 1)
                return ct;
            else if (ct is IGHPanel)
            {
                return GetControl((IGHPanel)ct, path.Skip(1));
            }
            else
            {
                throw new Exception($"Invalid path, Expected an IGHPanel in {gHContainer.Name} found {ct.GetType()}");
            }

        }
        /// <summary>
        /// return the defualt brush based on the control state (enable or disable)
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static Brush ActiveBrush(this GHControl control)
        {
            return (control.Enabled) ? Brushes.Black : Brushes.Gray;
        }


       
    }
}
