using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Explotter
{
    public partial class MainForm : Form
    {
        string currentPath = "";
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
                    Image = IconHelper.ExtractAssociatedIcon(drive).ToBitmap(),
                    Text = drive.Substring(0,1),
                    Tag = drive,
                    DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
                };
                toolStrip1.Items.Add(btn);
                btn.Click += (s, a) => EnterDrive((s as ToolStripButton).Tag as string);                
            }
            toolStrip1.Items.Add(new ToolStripSeparator());
            ToolStripButton btnBack = new ToolStripButton
            {
                Text = "UP",
                DisplayStyle = ToolStripItemDisplayStyle.Text
            };
            toolStrip1.Items.Add(btnBack);
            btnBack.Click += (s, a) => Backspace();

            toolStrip1.Items.Add(new ToolStripSeparator());
            ToolStripButton btnPrc = new ToolStripButton
            {
                Text = "Process List",
                DisplayStyle = ToolStripItemDisplayStyle.Text
            };
            toolStrip1.Items.Add(btnPrc);
            btnPrc.Click += (s, a) =>
            {
                {
                    Process[] procList = Process.GetProcesses();
                    string sList = "";
                    foreach (var item in procList)
                    {
                        sList += item.ProcessName + "\t";
                    }
                    MessageBox.Show(sList);

                };
            };
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
                FillList(new DirectoryInfo(currentPath));
            }
            else
            {
                Process.Start(newPath);
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
            FillList(new DirectoryInfo(currentPath));
        }
        /// <summary>
        /// Up one level
        /// </summary>
        public void Backspace()
        {
            DirectoryInfo pDir = new DirectoryInfo(currentPath).Parent;
            if (pDir == null)
            {
                return;
            }
            currentPath = pDir.FullName;
            activeList.Tag = currentPath;
            FillList(pDir);
        }
        /// <summary>
        /// Shows the contents of the current directory.
        /// </summary>
        /// <param name="path">path</param>
        private void FillList(DirectoryInfo dir)
        {
            activeList.Items.Clear();
                     
            imageListSmall.Images.Clear();
            
            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                imageListSmall.Images.Add(di.Name, IconHelper.ExtractAssociatedIcon(Path.GetFullPath(di.FullName)));

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
