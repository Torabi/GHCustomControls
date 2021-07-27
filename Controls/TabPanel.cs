using GH_IO.Types;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
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
    public class TabPanel : GHParameter, IGHPanel
    {

        /// <summary>
        /// value correspond to each tab
        /// </summary>
        List<int> _values;
        public TabPanel(string name, string description, int activeTab, bool showTitle, bool showHeaderText=true, Bitmap toolTipDiagram = null) : base(name,description, activeTab,toolTipDiagram)
        {
           
            _tabs = new Dictionary<string, GHControl[]>();
            _icons = new Dictionary<string, Image>();
            _values = new List<int>();
            _showTitle = showTitle;
            _showHeaderText = showHeaderText;
            _titleHeight = GH_FontServer.MeasureString(Name, SmallFont).Height + Offset;

        }



        public void Add(string name,int value, params GHControl[] controls)
        {
            foreach (var control in controls)
                control.Attributes = Attributes;
            _tabs.Add(name, controls);
            _values.Add(value); 
        }
        public void Add(string name,int value,Image image16x16, params GHControl[] controls)
        {
            foreach (var control in controls)
                control.Attributes = Attributes;

            _tabs.Add(name, controls);
            _values.Add(value);
            _icons.Add(name, image16x16);

        }
        public void Add(string name, params GHControl[] controls)
        {
            foreach (var control in controls)
                control.Attributes = Attributes;
            _tabs.Add(name, controls);
            _values.Add(_values.Count);
        }
        public void Add(string name, Image image16x16, params GHControl[] controls)
        {
            foreach (var control in controls)
                control.Attributes = Attributes;

            _tabs.Add(name, controls);
            _values.Add(_values.Count);
            _icons.Add(name, image16x16);

        }
        /// <summary>
        /// the tab name - imahe dictionary 
        /// </summary>
        Dictionary<string, Image> _icons;

        /// <summary>
        /// the tab name - control array dictionary 
        /// </summary>
        Dictionary<string, GHControl[]> _tabs;
        /// <summary>
        /// the rectangles of the headers
        /// </summary>
        List<RectangleF> _headers = new List<RectangleF>();
        /// <summary>
        /// the area of the tab contents
        /// </summary>
        RectangleF _tabRec;
        /// <summary>
        /// brush used to create the highlight
        /// </summary>
        LinearGradientBrush gradientBrush;

        /// <summary>
        /// if true then it shows the title of the panel group 
        /// </summary>
        bool _showTitle;

        /// <summary>
        /// set to false if the icon is provided . 
        /// </summary>
        bool _showHeaderText;


        public override GH_Types DataType => GH_Types.gh_int32;

        public override GH_ParamAccess Access => GH_ParamAccess.item;
        RectangleF _bounds;
        public override RectangleF Bounds
        {
            get => _bounds;
            set
            {
                _bounds = value;
                _headers = new List<RectangleF>();
                float shift = (_bounds.Width - 4) / _tabs.Count;
                float down = _showTitle ? _titleHeight : 0;
                // updates the headers layout 
                var rec = new RectangleF(_bounds.Left + 2, _bounds.Top + down + Offset, shift, _tabHeight);
                for (int i = 0; i < _tabs.Count; i++)
                {
                    _headers.Add(rec);
                    rec.Offset(shift, 0);
                }
                gradientBrush = new LinearGradientBrush(new PointF(rec.Left, rec.Top), new PointF(rec.Left, rec.Bottom), Color.AntiqueWhite, Color.Transparent);
                _tabRec = new RectangleF(_bounds.Left + 2, _bounds.Top + down +Offset + _tabHeight, _bounds.Width - 4, _bounds.Height - down - 2 * Offset - _tabHeight - 4);

                this.SetChildrenBounds(0,_tabHeight+ (_showTitle?_titleHeight:0)+2);
            }
        }
    

        public override int Offset => 4;
      
        int _titleHeight;

        public GHControl[] Items
        {
            get
            {
                int selectedTab = _values.IndexOf((int)CurrentValue);
                if (selectedTab > -1)
                    return _tabs.Values.ToArray()[selectedTab];
                else
                    return new GHControl[0];
            }
            set
            {

            }
        }
        #region layout
        Orientation _orientation = Orientation.Vecrtical;

        public Orientation Orientation
        {
            get => _orientation;
            set { _orientation = value; }
        }
   



        /// <summary>
        /// the height of the area where tab names are located
        /// </summary>
        int _tabHeight = 18;
        internal override int GetHeight()
        {
            int titleHeight = (_showTitle) ? _titleHeight : 0;
            return _tabHeight + titleHeight +this.GetChildrenHeight(Items) + 2 * Offset;

        }

        internal override int GetWidth()
        {
            // find the total tab width by summing up all the tab names 

            int w1 = _tabs.Keys.Aggregate(
                Offset, (int w, string key) => 
                w  
                + (_showHeaderText?GH_FontServer.StringWidth(key, SmallFont):0) 
                + (_icons.ContainsKey(key)&&_icons[key]!=null? 18:0) + Offset
                );

            // now we find the tab maximum tab size
            int w2 = this.GetChildrenWidth(Items);

            return Math.Max(w1, w2);
        }
        #endregion
        #region Mouse events
        internal override void MouseLeftClick(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
           
            // firts check if one of the headers has been clicked 
            for (int i = 0; i < _headers.Count; i++)
            {
                RectangleF rec = _headers[i];
                if (rec.Contains(e.CanvasLocation))
                {
                    // now check if this is selected header?
                    if ((int)CurrentValue == i)
                        return; // nothing happens
                    // another tab is selected , we switch the tab 
                    CurrentValue = _values[i];
                    // change the items with items inside the tab
                    //Items = _tabs.Values.ToArray()[i];
                    // update the mouse event 
                    result = result | GHMouseEventResult.Handled | GHMouseEventResult.UpdateSolution;
                    Attributes?.Redraw();
                    break;
                }
            }
            // click was out side of the header area, pass the click to the children
            this.MouseClickChildren(sender, customComponent, e, ref result);
        }

        public override void MouseLeave(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {

            // the cursor is already outside this control we must remove the heighlight from the header 
            if (HighlightedHeader > -1)
            {
                HighlightedHeader = -1;
                result = result | GHMouseEventResult.Invalidated;
            }
            // when cursor leaves the panel , it leaves all the childs
            this.MouseOverChildren(sender, customComponent, e, ref result);
        }
        int HighlightedHeader = -1;
        public override void MouseOver(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {

            //when cursor is hovering above the control we check if it highlights any of header

            for (int i = 0; i < _headers.Count; i++)
            {
                if (_headers[i].Contains(e.CanvasLocation))
                {
                    if (i != HighlightedHeader)
                    {
                        // highlight chnages 
                        HighlightedHeader = i;
                        result = result | GHMouseEventResult.Invalidated;

                    }
                    return; // no need to check  the children!
                }
            }
            //the cursor was not above any header so we check the child
            this.MouseOverChildren(sender, customComponent, e, ref result);
        }
        internal override void MouseRightClick(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {

            // click was out side of the header area, pass the click to the children
            this.MouseClickChildren(sender, customComponent, e, ref result);
        }
        public override void MouseKeyUp(GH_Canvas sender, GHCustomComponent customComponent, GH_CanvasMouseEvent e, ref GHMouseEventResult result)
        {
            this.MouseKeyUpChildren(sender, customComponent, e, ref result);
        }
        #endregion

        internal override void SetupToolTip(PointF canvasPoint, GH_TooltipDisplayEventArgs e)
        {
           
            if (this.SetChildrenToolTip(canvasPoint, e))
                return;
            
            int i = _headers.FindIndex(rec => rec.Contains(canvasPoint));
            if (i == -1)
            {
                base.SetupToolTip(canvasPoint,e);
            }
            else
            {
                e.Title = _tabs.ToArray()[i].Key;
                //e.Text = Description;
            }
            
        }
        

        
        private PointF[] headerShape(RectangleF rec)
        {
            return new PointF[]
            {
                new PointF(rec.Left,rec.Bottom),
                new PointF(rec.Left+2,rec.Top),
                new PointF(rec.Right-2,rec.Top),
                new PointF(rec.Right,rec.Bottom),
                new PointF(rec.Left,rec.Bottom)


            };
        }
        internal override void Render(Graphics graphics,PointF cursorCanvasPosition, bool selected, bool locked, bool hidden)
        {
            // render the frame
            Helpers.DrawFrame(Bounds, graphics, (_showTitle)?Name:"", selected, !Enabled, hidden);
            // render the headers,
            int selectedTab = _values.IndexOf((int)CurrentValue);
            for (int i = 0; i < _headers.Count; i++)
            {
                //first draw only the unselected ones

                if (i != selectedTab)
                {
                    //GH_Capsule capsule = GH_Capsule.CreateCapsule(_headers[i], GH_Palette.Transparent, 2, 0);
                    //capsule.Render(graphics, new GH_PaletteStyle((Highlighted == i) ? Color.LightGoldenrodYellow : Color.Transparent, Color.Black));
                    //capsule.Dispose();
                    PointF[] poly = headerShape(_headers[i]);
                    if (_headers[i].Contains(cursorCanvasPosition) && Enabled)
                    {

                        graphics.FillPolygon(gradientBrush, poly);
                    }
                    using (SolidBrush b = new SolidBrush(Color.FromArgb(50, Color.Gray)))
                    {
                        graphics.FillPolygon(b, poly);
                    }
                    using (Pen p = new Pen(Brushes.Black))
                    {
                        graphics.DrawPolygon(p, poly);
                    }

                    string tabName = _tabs.Keys.ToArray()[i];
                    Image icon = _icons.ContainsKey(tabName) ? _icons[tabName] : null;
                    if (icon != null)
                    {
                        graphics.DrawImage(icon, _headers[i].Left + 1, _headers[i].Top + 1);
                    }
                    // draw the text 
                    if (_showHeaderText)
                    {
                        if (icon == null)
                        {
                            using (StringFormat s = new StringFormat() { Alignment = StringAlignment.Center })
                            {

                                graphics.DrawString(
                                tabName, SmallFont, this.ActiveBrush(), _headers[i], s
                                );
                            }
                        }
                        else
                        {
                            using (StringFormat s = new StringFormat() { Alignment = StringAlignment.Near })
                            {

                                graphics.DrawString(
                                tabName, SmallFont, this.ActiveBrush(), _headers[i].Left+1+icon.Width,_headers[i].Top, s
                                );
                            }
                        }
                    }
                }

            }
            if (_headers.Count == 0)
                return;
            // now draw the border 
            //int selectedTab = (int)CurrentValue;
            //RectangleF rect = new RectangleF(Bounds.Left + 2, Bounds.Top + _fontSize + _tabHeight, Bounds.Width - 4, Bounds.Height - _fontSize - _tabHeight - 2);
            RectangleF header = _headers[selectedTab];
            PointF[] tabPoly = new PointF[]
                {
                    new PointF(_tabRec.Left,_tabRec.Top),
                    new PointF(header.Left,header.Bottom),
                    new PointF(header.Left+2,header.Top),
                    new PointF(header.Right-2,header.Top),
                    new PointF(header.Right,header.Bottom),
                    new PointF(_tabRec.Right,_tabRec.Top),
                    new PointF(_tabRec.Right,_tabRec.Bottom+2),
                    new PointF(_tabRec.Left,_tabRec.Bottom+2),
                    new PointF(_tabRec.Left,_tabRec.Top)
                };
            graphics.FillPolygon(new SolidBrush(Color.FromArgb(50, Color.Gray)), tabPoly);
            graphics.DrawPolygon(new Pen(Brushes.Black, (Enabled)?2:1), tabPoly);
            //// draw the active tab 
            //int selectedTab = (int)CurrentValue;
            //GH_Capsule capsule2 = GH_Capsule.CreateCapsule(_headers[selectedTab], GH_Palette.Transparent, 2, 0);
            //capsule2.Render(graphics, new GH_PaletteStyle((Highlighted == selectedTab) ? Color.LightGoldenrodYellow : Color.Transparent, Color.Black));
            //capsule2.Dispose();
            // draw the text 
            string selectedTabName = _tabs.Keys.ToArray()[selectedTab];
            Image slectedicon = _icons.ContainsKey(selectedTabName) ? _icons[selectedTabName] : null;
            if (slectedicon != null)
            {
                graphics.DrawImage(slectedicon, _headers[selectedTab].Left + 1, _headers[selectedTab].Top + 1);
            }
            if (_showHeaderText)
            {
                using (Font f = new Font(SmallFont, FontStyle.Bold))
                {
                    if (slectedicon == null)
                    {

                        using (StringFormat s = new StringFormat() { Alignment = StringAlignment.Center })
                            graphics.DrawString(
                                selectedTabName, f, this.ActiveBrush(), _headers[selectedTab], s);
                    }
                    else
                    {
                        
                        using (StringFormat s = new StringFormat() { Alignment = StringAlignment.Near })
                            graphics.DrawString(
                                selectedTabName, f, this.ActiveBrush(), _headers[selectedTab].Left+1+ slectedicon.Width, _headers[selectedTab].Top, s);
                    }
                }
            }
            // draw the items 
            foreach (GHControl control in Items)
                if (control.IsVisible)
                control.Render(graphics, cursorCanvasPosition, selected, !Enabled, hidden);
        }


    }
}
