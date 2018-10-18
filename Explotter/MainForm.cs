using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Explotter
{
    public partial class MainForm : Form
    {
        string currentPath="";
        const int WM_DEVICECHANGE = 0x219;
        const int DBT_DEVICEARRIVAL = 0x8000;
        const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        ListView activeList;

        public MainForm()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Handles drive connection and disconnect events.
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            if(m.Msg==WM_DEVICECHANGE
                && (int)m.WParam == DBT_DEVICEARRIVAL
                || (int)m.WParam == DBT_DEVICEREMOVECOMPLETE)
            {
                FillDriveButtons();
            }
                base.WndProc(ref m);
        }
        /// <summary>
        /// Form components loading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {   
            FillDriveButtons();
            activeList = driveListView2;
            EnterDrive(Directory.GetLogicalDrives()[0]);
            activeList = driveListView;
            EnterDrive(Directory.GetLogicalDrives()[0]);
        }
        /// <summary>
        /// Shows available drives
        /// </summary>
        private void FillDriveButtons()
        {
            toolStrip1.Items.Clear();
            foreach (string drive in Directory.GetLogicalDrives())
            {
                ToolStripButton btn = new ToolStripButton
                {
                    Text = drive,
                    DisplayStyle = ToolStripItemDisplayStyle.Text
                };
                toolStrip1.Items.Add(btn);
                btn.Click += (s, a) => EnterDrive((s as ToolStripButton).Text);                
            }
        }
        /// <summary>
        /// Enters directory of the active list 
        /// </summary>
        /// <param name="directory">directory</param>
        private void EnterDirectory(string directory)
        {
            string newPath;
            newPath = currentPath + directory + '\\';
            if (Directory.Exists(newPath))
            {
                currentPath = newPath;
                activeList.Tag = currentPath;
                FillList(currentPath);
            }
        }
        /// <summary>
        /// Enters drive of the active list
        /// </summary>
        /// <param name="drive">drive</param>
        private void EnterDrive(string drive)
        {
            currentPath = drive;
            activeList.Tag = currentPath;
            FillList(currentPath);
        }
        /// <summary>
        /// Shows the contents of the current directory.
        /// </summary>
        /// <param name="path">path</param>
        private void FillList(string path)
        {
            activeList.Items.Clear();
            DirectoryInfo dir = new DirectoryInfo(path);
           
            imageListSmall.Images.Clear();
            
            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                //if(di.Name.IndexOf('$')!=0)
                //imageListSmall.Images.Add(di.Name, Icon.ExtractAssociatedIcon(Path.GetFullPath(di.FullName)));

                activeList.Items.Add(new ListViewItem
                {
                    Text=di.Name,
                    ImageKey= di.Name
                });

            }
            foreach (FileInfo fi in dir.GetFiles())
            {
                imageListSmall.Images.Add(fi.Name, Icon.ExtractAssociatedIcon(fi.FullName));

                activeList.Items.Add(new ListViewItem
                {
                    Text = fi.Name,
                    ImageKey = fi.Name
                });
            }
        }
        /// <summary>
        /// Double-clicking opens the folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void driveListView_DoubleClick(object sender, EventArgs e)
        {
            EnterDirectory(activeList.SelectedItems[0].Text);   
        }
        /// <summary>
        /// Assignes current path and active list to the left list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void driveListView_Click(object sender, EventArgs e)
        {
            activeList = driveListView;
            currentPath = activeList.Tag as string;
        }
        /// <summary>
        /// Assignes current path and active list to the right list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void driveListView2_Click(object sender, EventArgs e)
        {
            activeList = driveListView2;
            currentPath = activeList.Tag as string;
        }
    }
}
