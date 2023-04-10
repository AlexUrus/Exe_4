using GMap.NET;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Exercise_4.Controllers
{
    public class MapController
    {
        private readonly GMapControl Map;
        private GMapPolygon Polygon;
        private readonly GMapOverlay Overlay;
        private readonly DialogController dialogController;
        private readonly MarkerController markerController;
        public GMapMarker CurrentMarker { 
            get
            {
                return markerController.CurrentMarker;
            }
            set
            {
                markerController.CurrentMarker = value;
            }
        } 
        public bool MarkerIsDragging { 
            get
            {
                return markerController.MarkerIsDragging;
            }
            set
            {
                markerController.MarkerIsDragging = value;
            }
        }

        public MapController(GMapControl map)
        {
            Map = map;
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
            markerController = new MarkerController();
            dialogController = new DialogController();
            Overlay = new GMapOverlay("Overlay");
            CreatePolygon();
            AddMarkersInOverlay();
            Map.Overlays.Add(Overlay);
        }

        public List<GMapMarker> GetListMarkers()
        {
            return markerController.GetListMarkers();
        }

        public void UpdateMarkerList(GMapMarker marker)
        {
            markerController.UpdateMarkerInList(marker);
        }

        public void AddMarkersInMenu(MenuStrip menu)
        {
            dialogController.AddListInMenu(GetListMarkers(),menu);

            foreach (ToolStripMenuItem item in menu.Items)
            {
                item.Click += new EventHandler(MenuMarkersItem_Click);
            }
            menu.Visible = true;
        }

        private void MenuMarkersItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            List<GMapMarker> markers = markerController.GetListMarkers();
            CurrentMarker = markers.FirstOrDefault(m => m.ToolTipText == menuItem.Text);

            string nmea = OpenReadNmea();
            MoveMarkerToNmea(nmea);

            Overlay.Markers.Add(CurrentMarker);

            MenuStrip menu = (MenuStrip)menuItem.GetCurrentParent();
            menu.Visible = false;
        }
        public string OpenReadNmea()
        {
           return dialogController.ReadNmea();
        }

        public void MoveMarkerToNmea(string nmea)
        {
            markerController.ChangeLatLngMarker(nmea);

            OpenMarkerContextMenu(CurrentMarker);
        }

        public void OpenMarkerContextMenu(GMapMarker marker)
        {
            if (Polygon.IsInside(CurrentMarker.Position))
            {
                ContextMenuStrip menuStrip = dialogController.CreateMarkerContextMenu();

                menuStrip.Items[0].Click += new EventHandler(DialogInfoMarker_Click);
                menuStrip.Items[1].Click += new EventHandler(ChangeColorMarker_Click);
                menuStrip.Items[2].Click += new EventHandler(CreateRandomMarker_Click);

                GPoint gpoint = Map.FromLatLngToLocal(marker.Position);
                Point point = new Point((int)gpoint.X, (int)gpoint.Y);

                menuStrip.Show(Map, point);
            } 
        }
        public void CreateMarker(PointLatLng pointClick)
        {
            GMapMarker marker = markerController.CreateMarker(pointClick);

            Overlay.Markers.Add(marker);

            Map.UpdateMarkerLocalPosition(marker);

            OpenMarkerContextMenu(CurrentMarker);
        }

        public void ChangeColorMarker_Click(object sender, EventArgs e)
        {
            Overlay.Markers.Remove(markerController.CurrentMarker);

            GMapMarker marker = markerController.ChangeColorMarker();

            Overlay.Markers.Add(marker);

            markerController.CurrentMarker = null;

        }

        public void CreateRandomMarker_Click(object sender, EventArgs e)
        {
            GMapMarker marker = markerController.CreateRandomMarker();

            Overlay.Markers.Add(marker);

            MessageBox.Show("Маркер создан: " + marker.Position);
        }

        public void DialogInfoMarker_Click(object sender, EventArgs e)
        {
            markerController.ShowDialogMarker();
            markerController.CurrentMarker = null;
        }

        private void CreatePolygon()
        {
            Polygon = new GMapPolygon(new List<PointLatLng>
            {
                new PointLatLng(54.93109, 82.81769),
                new PointLatLng(54.93109, 83.03123),
                new PointLatLng(54.86752, 82.98592),
                new PointLatLng(54.87503, 82.79228),
            }, "MyPolygon")
            {
                Fill = new SolidBrush(Color.FromArgb(50, Color.Blue)),
                Stroke = new Pen(Color.Blue, 1)
            };
            Overlay.Polygons.Add(Polygon);
        }
        private void AddMarkersInOverlay()
        {
            List<GMapMarker> markers = markerController.GetListMarkers();
            foreach (GMapMarker mapMarker in markers)
            {
                Overlay.Markers.Add(mapMarker);
            }
        }
    }
}