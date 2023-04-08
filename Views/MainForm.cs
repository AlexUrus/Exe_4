using Exercise_4.Controllers;
using GMap.NET;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exercise_4
{
    public partial class MainForm : Form
    {
        private GMapOverlay Overlay;
        private GMapMarker CurrentMarker;
        MarkerController markerController;

        public MainForm()
        {
            InitializeComponent();
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            markerController = new MarkerController();
            List<GMapMarker> markers = markerController.GetListMarkers();

            Overlay = new GMapOverlay("Overlay");

            foreach (GMapMarker mapMarker in markers)
            {
                Overlay.Markers.Add(mapMarker);
            }

            Map.Overlays.Add(Overlay);
        }

        private void Map_Load(object sender, EventArgs e)
        {
            #region Параметры карты
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            Map.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
            Map.MinZoom = 2;
            Map.MaxZoom = 16;
            Map.Zoom = 8;
            Map.Position = new PointLatLng(55.0415, 82.9346);// Новосибирск
            Map.MouseWheelZoomType = MouseWheelZoomType.MousePositionWithoutCenter;
            Map.DragButton = MouseButtons.Left;
            Map.ShowCenter = false;
            Map.ShowTileGridLines = false;
            #endregion
        }

        private void Map_OnMapDoubleClick(PointLatLng pointClick, MouseEventArgs e)
        {
            GMapMarker marker = markerController.CreateMarker(pointClick, e);
            Overlay.Markers.Add(marker);
        }

        private void Map_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            CurrentMarker = item;
            if (e.Button == MouseButtons.Left)
            {
                Map.CanDragMap = false;
                item.ToolTipMode = MarkerTooltipMode.Never;
                item.Offset = new Point(-item.Size.Width / 2, -item.Size.Height / 2);
                item.Position = Map.FromLocalToLatLng(e.X, e.Y);
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
            if (CurrentMarker != null)
            {
                markerController.UpdateMarker(CurrentMarker);
            }
            CurrentMarker = null;
            Map.CanDragMap = true;

            Map.MouseMove -= new MouseEventHandler(Map_MouseMove);
        }

        private void Map_MouseMove(object sender, MouseEventArgs e)
        {
            if (CurrentMarker != null && e.Button == MouseButtons.Left)
            {
                CurrentMarker.Position = Map.FromLocalToLatLng(e.X, e.Y);
            }
        }

        
    }
}
