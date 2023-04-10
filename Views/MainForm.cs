using Exercise_4.Controllers;
using GMap.NET;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Exercise_4
{
    public partial class MainForm : Form
    {
        private MapController mapController;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            mapController = new MapController(Map);
        }

        private void Map_OnMapDoubleClick(PointLatLng pointClick, MouseEventArgs e)
        {
            mapController.CreateMarker(pointClick);
        }

        private void Map_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            mapController.CurrentMarker = item;
            if (e.Button == MouseButtons.Left)
            {
                mapController.MarkerIsDragging = true;
                Map.CanDragMap = false;
                item.ToolTipMode = MarkerTooltipMode.Never;
                Map.MouseMove += new MouseEventHandler(Map_MouseMove);
            }
        }

        private void Map_OnMarkerEnter(GMapMarker item)
        {
            item.ToolTipMode = MarkerTooltipMode.Always;
        }

        private void Map_OnMarkerLeave(GMapMarker item)
        {
            item.ToolTipMode = MarkerTooltipMode.Never;

            if (mapController.MarkerIsDragging) 
            {
                mapController.UpdateMarkerList(mapController.CurrentMarker);

                mapController.OpenMarkerContextMenu(item);
            }
            else
            {
                mapController.CurrentMarker = null;
            }
           
            mapController.MarkerIsDragging = false;

            Map.CanDragMap = true;

            Map.MouseMove -= new MouseEventHandler(Map_MouseMove);
        }

        private void Map_MouseMove(object sender, MouseEventArgs e)
        {
            if (mapController.CurrentMarker != null && e.Button == MouseButtons.Left)
            {
                mapController.CurrentMarker.Position = Map.FromLocalToLatLng(e.X, e.Y);
            }
        }

        private void ParseButton_Click(object sender, EventArgs e)
        {
            if(!MenuMarkers.Visible)
            {
                mapController.AddMarkersInMenu(MenuMarkers);
                MenuMarkers.BringToFront();
            }
        }
    }
}
