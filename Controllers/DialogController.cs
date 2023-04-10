using GMap.NET;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Exercise_4.Controllers
{
    public class DialogController
    {

        private readonly OpenFileDialog openFileDialog;

        public DialogController()
        {
            openFileDialog = new OpenFileDialog();
        }

        public void AddListInMenu(List<GMapMarker> markers, MenuStrip menuMarkers)
        {
            menuMarkers.Items.Clear();
            foreach (var marker in markers)
            {
                ToolStripMenuItem menuItem = new ToolStripMenuItem(marker.ToolTipText);
                menuMarkers.Items.Add(menuItem);
            }
            menuMarkers.Visible = true;
        }

        public ContextMenuStrip CreateMarkerContextMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem dialog = new ToolStripMenuItem("Диалоговое окно с информацией");
            ToolStripMenuItem color = new ToolStripMenuItem("Изменить цвет маркера");
            ToolStripMenuItem newMarker = new ToolStripMenuItem("Создать новый маркер в случайной точке");

            menu.Items.Add(dialog);
            menu.Items.Add(color);
            menu.Items.Add(newMarker);
            
            return menu;
        }

        public string ReadNmea()
        {
            openFileDialog.ShowDialog();

            string fileName = openFileDialog.FileName;
            string nmea = System.IO.File.ReadAllText(fileName);
            return nmea;
        }
    }
}