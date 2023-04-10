using Exercise_4.Controllers;
using GMap.NET;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exercise_4
{
    public partial class MainForm : Form
    {
        private GMapOverlay Overlay;
        private GMapPolygon Polygon;
        private MarkerController markerController;
        private OpenFileDialog dlg; // можно убрать в маркер контроллер

        public MainForm()
        {
            InitializeComponent();
        }
        private void MainForm_Load(object sender, EventArgs e) // разбить на методы
        {
            MenuMarkers.BringToFront();
            markerController = new MarkerController();
            List<GMapMarker> markers = markerController.GetListMarkers();

            Overlay = new GMapOverlay("Overlay");

            foreach (GMapMarker mapMarker in markers)
            {
                Overlay.Markers.Add(mapMarker);
            }
            AddPolygon();
            Map.Overlays.Add(Overlay);

            dlg = new OpenFileDialog();
            dlg.Filter = "NMEA Files (*.nmea) |*.nmea";
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
            CreateMarker(pointClick);
        }
        private void CreateMarker(PointLatLng pointClick)
        {
            GMapMarker marker = markerController.CreateMarker(pointClick);

            Overlay.Markers.Add(marker);

            Map.UpdateMarkerLocalPosition(marker);

            if (Polygon.IsInside(marker.Position))
            {
                CreateMarkerContextMenu(marker);
            }
        }

        private void Map_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            markerController.CurrentMarker = item;
            if (e.Button == MouseButtons.Left)
            {
                markerController.MarkerIsDragging = true;
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

        private void Map_OnMarkerLeave(GMapMarker item)// бкз перемещения на маркер наводишь и выдается меню нужно исправить
        {
            bool isCalledActionsMarker = false;
            item.ToolTipMode = MarkerTooltipMode.Never;

            if (markerController.MarkerIsDragging) // разобраться с условием как лучше сделать
            {
                markerController.UpdateMarkerList(markerController.CurrentMarker);
                if (Polygon.IsInside(markerController.CurrentMarker.Position))
                {
                    CreateMarkerContextMenu(markerController.CurrentMarker);
                    isCalledActionsMarker = true;
                }
            }
            if(!isCalledActionsMarker) 
            {
                markerController.CurrentMarker = null;
            }
            markerController.MarkerIsDragging = false;

            Map.CanDragMap = true;

            Map.MouseMove -= new MouseEventHandler(Map_MouseMove);
        }

        private void Map_MouseMove(object sender, MouseEventArgs e)
        {
            if (markerController.CurrentMarker != null && e.Button == MouseButtons.Left)
            {
                markerController.CurrentMarker.Position = Map.FromLocalToLatLng(e.X, e.Y);
            }
        }

        private void ParseButton_Click(object sender, EventArgs e)
        {
            OpenMenuVehicles();
            ParseButton.Enabled = false;
        }

        private void OpenMenuVehicles()
        {
            List<GMapMarker> markers = markerController.GetListMarkers();
            foreach (var marker in markers)
            {
                ToolStripMenuItem menuItem = new ToolStripMenuItem(marker.ToolTipText);
                menuItem.Click += new EventHandler(MenuItem_Click);
                MenuMarkers.Items.Add(menuItem);
            }
            MenuMarkers.Visible = true;
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;

            string nmea = ReadNmea();

            markerController.CurrentMarker = Overlay.Markers.FirstOrDefault(m => m.ToolTipText == menuItem.Text);

            MoveMarkerToNmea(nmea);

            Overlay.Markers.Add(markerController.CurrentMarker);
            //markerController.CurrentMarker = null;
            MenuMarkers.Items.Clear();
            MenuMarkers.Visible = false;
            ParseButton.Enabled = true;
        }
        private void MoveMarkerToNmea(string nmea)
        {
            markerController.ChangeLatLngMarker(nmea);

            if (Polygon.IsInside(markerController.CurrentMarker.Position))
            {
                CreateMarkerContextMenu(markerController.CurrentMarker);
            }
        }
        private string ReadNmea()
        {
            dlg.ShowDialog();

            string fileName = dlg.FileName;
            string nmea = System.IO.File.ReadAllText(fileName);
            return nmea;
        }

        private void AddPolygon()
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
       
        private void CreateMarkerContextMenu(GMapMarker marker)
        {
            markerController.CurrentMarker = marker;

            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem menuItem1 = new ToolStripMenuItem("Диалоговое окно с информацией");
            ToolStripMenuItem menuItem2 = new ToolStripMenuItem("Изменить цвет маркера");
            ToolStripMenuItem menuItem3 = new ToolStripMenuItem("Создать новый маркер в случайной точке");

            menuItem1.Click += DialogInfoMarker_Click;
            menuItem2.Click += ChangeColorMarker_Click;
            menuItem3.Click += CreateRandomMarker_Click;

            menu.Items.Add(menuItem1);
            menu.Items.Add(menuItem2);
            menu.Items.Add(menuItem3);

            GPoint gpoint = Map.FromLatLngToLocal(marker.Position);
            Point point = new Point((int)gpoint.X, (int)gpoint.Y);

            menu.Show(Map, point);
        }
        private void ChangeColorMarker_Click(object sender, EventArgs e)
        {
            Overlay.Markers.Remove(markerController.CurrentMarker);

            GMapMarker marker = markerController.ChangeColorMarker();
            Overlay.Markers.Add(marker);
            markerController.CurrentMarker = null;
        }
        private void CreateRandomMarker_Click(object sender, EventArgs e)
        {
            GMapMarker marker = markerController.CreateRandomMarker();
            Overlay.Markers.Add(marker);
        }
        private void DialogInfoMarker_Click(object sender, EventArgs e)
        {
            markerController.ShowDialogMarker();
            markerController.CurrentMarker = null;
        }


    }
}
