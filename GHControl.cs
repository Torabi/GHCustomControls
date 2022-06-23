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
    public enum GHMouseEventResult
    {
        None=0,
        Invalidated=1,        
        UpdateSolution=2,
        Handled=4,
        Highlighted=8


    }
    public abstract class  GHControl : IGHCustomControl
    {
        protected GHControl(string name ,string description,Bitmap toolTipDiagram = null)
        {
            Name = name;
            Description = description;
            ToolTipDiagram = toolTipDiagram;
        }

        public static Font SmallFont =GH_FontServer.StandardAdjusted;
        public static Font StandardFont = GH_FontServer.LargeAdjusted;

        /// <summary>
        /// if true then updates the solution when change
        /// </summary>
        public bool UpdateSolution { get; set; } = true;

        public GHCustomAttributes Attributes;

        internal abstract int GetHeight();
        internal abstract int GetWidth();

        public virtual RectangleF Bounds { get; set; }

        internal abstract void Render(Graphics graphics,PointF cursorCanvasPosition, bool selected, bool locked, bool hidden);
        //internal abstract void MouseOver(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e , ref GHMouseEventResult result);
        /// <summary>
        /// true when the cursor is inside the <c>ActiveZone</c>
        /// </summary>
        public bool IsMouseIn = false;
        public virtual void MouseOver(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
            if (this is IGHPanel)
            {
                ((IGHPanel)this).MouseOverChildren(sender, customComponent, e, ref result);
                if (result.HasFlag(GHMouseEventResult.Invalidated | GHMouseEventResult.Handled))
                    return;

            }

            if (ActiveZone.Contains(e.CanvasLocation))
            {
                if (!IsMouseIn)
                {
                    IsMouseIn = true;
                    result = result | GHMouseEventResult.Invalidated | GHMouseEventResult.Handled;
                    return;
                }
            }
            else
            {
                if (IsMouseIn )
                {
                    IsMouseIn = false;
                    result = result | GHMouseEventResult.Invalidated | GHMouseEventResult.Handled;

                }
            }
            
            
        }

        public virtual RectangleF ActiveZone => Bounds;
        public virtual void MouseLeave(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e,ref GHMouseEventResult result)
        {
            
            
            if (IsMouseIn)
            {
                IsMouseIn = false;
                result = result | GHMouseEventResult.Invalidated | GHMouseEventResult.Handled;
                return;

            }
            if (this is IGHPanel)
            {
                ((IGHPanel)this).MouseOverChildren(sender, customComponent, e, ref result);
                if (result.HasFlag(GHMouseEventResult.Invalidated | GHMouseEventResult.Handled))
                    return;

            }

        }

        internal abstract void MouseLeftClick(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result);

        internal abstract void MouseRightClick(GH_Canvas sender,GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result);
        internal virtual void SetupToolTip(PointF canvasPoint,GH_TooltipDisplayEventArgs e)
        {
            if (!Enabled)
                return;
            if (ToolTipDiagram == null)
            {
                e.Title = Name;
                e.Description = Description;
            }
            else
            {
                e.Text = Name;
                e.Description = Description;
                e.Diagram = ToolTipDiagram;
                
            }
        }

         


        public virtual void MouseKeyUp(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result) 
        { 
            if (this is IGHPanel)
            {
                IGHPanel panel = (IGHPanel)this;
                foreach (GHControl control in panel.Items)
                {
                    if (control.Bounds.Contains(e.CanvasLocation))
                    {
                        control.MouseKeyUp(sender, customComponent, e, ref result);
                        if (!control.UpdateSolution)
                        {
                            result &= ~GHMouseEventResult.UpdateSolution;
                        }
                        if (result.HasFlag(GHMouseEventResult.Handled))
                            return;
                    }
                }
            }
        }
        /// <summary>
        /// name of the control also it is used as the title in UI
        /// </summary>
        public string Name { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// a bitmap shown in the detail area of the tooltip
        /// </summary>
        public Bitmap ToolTipDiagram { get ; set; }
        /// <summary>
        /// the amount of the offset for the sub-controls inside this control
        /// </summary>
        public abstract int Offset { get; }
        /// <summary>
        /// used to react to the mouse  
        /// </summary>
        //public int Highlighted { get; set; } = -1;

        bool _visible=true;
        
        /// <summary>
        /// return true if the control is visible 
        /// </summary>
        public bool IsVisible { get=>_visible;
            set 
            { 
                if (_visible != value)
                {
                    Attributes?.Redraw();
                    _visible = value;
                         
                }
            } 
        } 


        private bool _enabled = true;
        /// <summary>
        /// when control is not enabled do not responds to  mouse event and it renderes as locked 
        /// </summary>
        public bool Enabled { 
            get=>_enabled;
            set 
            {
                if (_enabled != value)
                    Refresh();
                _enabled = value;
                if(this is IGHPanel)
                {
                    IGHPanel panel = (IGHPanel)this;
                    foreach (IGHCustomControl control in panel.Items)
                        control.Enabled = _enabled;
                }
            }
        }

       

        /// <summary>
        /// set the attribute of this control and all its children 
        /// </summary>
        /// <param name="att"></param>
        internal virtual void SetAttribute(GHCustomAttributes att)
        {
            Attributes = att;
            
            if (this is IGHPanel)
            {
                IGHPanel panel = (IGHPanel)this;
                foreach (GHControl child in panel.Items)
                {
                    child.SetAttribute(Attributes);
                }
            }
        }
        /// <summary>
        /// return the defualt brush based on the control state (enable or disable)
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public Brush ActiveBrush()
        {
            return (Enabled) ? Brushes.Black : Brushes.Gray;
        }

        internal Point PointToScreen(GH_Canvas canvas,PointF point)
        {
            var p = canvas.Viewport.ProjectPoint(point); // in the grasshopper window
            return canvas.PointToScreen(new System.Drawing.Point((int)p.X, (int)p.Y));
        }

        /// <summary>
        /// updates the control graphics at the next paint operation
        /// </summary>
        public void Refresh()
        {

           if (!Bounds.IsEmpty)
                Grasshopper.Instances.ActiveCanvas?.Invalidate(Rectangle.Round( Bounds));
            //if (this is IGHPanel)
            //{
            //    IGHPanel panel = (IGHPanel)this;
            //    foreach (IGHCustomControl control in panel.Items)
            //        control.Refresh();
            //}
        }
        /// <summary>
        /// schedule to redraw the canvas in 100 ms
        /// </summary>
        public void ForceRedraw()
        {
            Grasshopper.Instances.ActiveCanvas?.ScheduleRegen(100);
        }

        


    }
}
