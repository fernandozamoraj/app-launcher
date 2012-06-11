using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;


namespace FzLauncher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
 
        }

        private void LoadApps()
        {
            panel1.Controls.Clear();

            DirectoryInfo directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

            List<DirectoryInfo> directories = new List<DirectoryInfo>();
            directories.Add(directory);

            foreach (var childDirectory in directory.GetDirectories())
            {
                directories.Add(childDirectory);
            }

            TabControl tabControl = new TabControl();

            tabControl.Size = this.panel1.Size;
            this.panel1.Controls.Add(tabControl);
            tabControl.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            tabControl.Location = new Point(0, 0);
            tabControl.Font = new Font("Arial", 11);

            int count = 1;


            CreateTabPage(directory, tabControl, count);

            count = 2;
            foreach (var tempDirectory in directory.GetDirectories())
            {
                CreateTabPage(tempDirectory, tabControl, count);
                count++;
            }

            CreateHelpTabPage(tabControl);
        }

        private void CreateTabPage(DirectoryInfo directory, TabControl tabControl, int count)
        {
            TabPage tabPage = new TabPage(directory.Name);
            tabPage.Size = tabControl.Size;
            tabPage.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            tabPage.Location = new Point(0, 0);
            tabControl.TabPages.Add(tabPage);
            tabPage.Font = new Font("Arial", 8);

            Panel panel = new Panel();
            panel.Size = tabPage.Size;
            panel.Anchor =
                tabPage.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            panel.Location = new Point(0, 0);
            tabPage.Controls.Add(panel);
            panel.AutoScroll = true;
            panel.AllowDrop = true;
            panel.DragEnter += new DragEventHandler(panel_DragEnter);
            panel.DragDrop += new DragEventHandler(panel_DragDrop);
           

            CreateTab(panel, directory);
        }

        void panel_DragDrop(object sender, DragEventArgs e)
        {
            throw new NotImplementedException();
        }

        void panel_DragEnter(object sender, DragEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void CreateHelpTabPage(TabControl tabControl)
        {
            TabPage tabPage = new TabPage("Help");
            tabPage.Size = tabControl.Size;
            tabPage.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            tabPage.Location = new Point(0, 0);
            tabControl.TabPages.Add(tabPage);

            Panel panel = new Panel();
            panel.Size = tabPage.Size;
            panel.Anchor =
                tabPage.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            panel.Location = new Point(0, 0);
            tabPage.Controls.Add(panel);

            RichTextBox richTextBox = new RichTextBox();
            richTextBox.Size = new Size(tabPage.Size.Width, tabPage.Size.Height);
            richTextBox.Location = new Point(0,0);
            richTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            panel.Controls.Add(richTextBox);

            richTextBox.Text = GetHelpText();
            richTextBox.Font = new Font("Arial", 14);
        }

        private string GetHelpText()
        {
            return "To use this application follow the steps below" + Environment.NewLine + Environment.NewLine +
                   "1. Create a folder anywhere you desire" + Environment.NewLine +
                   "2. Create shortcuts to your favorite apps and drop them in that folder" + Environment.NewLine +
                   "3. Drop this application in that folder as well" + Environment.NewLine +
                   "4. Create subfolders with additional shorcuts for grouping your apps" + Environment.NewLine + Environment.NewLine +
                   "FZ Launcher will create a tab corressponding to each folder. " +
                   "   This will help you group all your applications to your liking." + Environment.NewLine +
                   " " + Environment.NewLine+
                   "Currently FZ Launcher has trouble loading some apps such as Java Apps and Apps that require Run as Administrator " +
                   ".  These fixes will be coming soon." + Environment.NewLine + Environment.NewLine +
                   "As an addition tip you can set this application to your toolbar and have just this one app instead of all individual app shortcuts on your toolbar.";

        }


        private void CreateTab(Panel panel, DirectoryInfo directory)
        {
            
            int currentPositionX = 10;
            int currentPositionY = 10;
            int fileCount = -1;

            int lastKnownWIdth = 0;
            string log = string.Empty;

            foreach (FileInfo file in directory.GetFiles())
            {

                log = file.FullName;

                try
                {
                    string fullFilePath = GetFullExePath(file);

                    if (string.IsNullOrEmpty(fullFilePath))
                        continue;

                    Icon icon;

                    try
                    {
                        icon = ExecutableUtility.IconFromFilePath(fullFilePath);

                    }
                    catch (Exception exception)
                    {
                        icon = this.Icon;
                    }

                    if (fileCount < 1)
                        this.Icon = icon;

                    fileCount++;

                    int rightSide = this.panel1.Right < 800 ? 800 : this.panel1.Right;

                    if (currentPositionX + (lastKnownWIdth * 3) >= rightSide)
                    {
                        currentPositionX = 10;
                        currentPositionY += lastKnownWIdth * 2;
                    }
                    else
                    {
                        currentPositionX += lastKnownWIdth * 2;
                    }

                    Button button = CreateButton(icon, file.Name, currentPositionX, currentPositionY);
                    panel.Controls.Add(button);

                    lastKnownWIdth = button.Width;

                    string executablePath = file.FullName;

                    SetupButtonClickEvent(fullFilePath, button, executablePath);

                }
                catch (Exception exception)
                {
                    continue;
                }
            }
        }

        private Size _lastKnownSize = new Size(50, 50);

        private Button CreateButton(Icon icon, string text, int currentPositionX, int currentPositionY)
        {
            Button button = new Button();
            button.Location = new Point(currentPositionX, currentPositionY);
            button.Size = new Size(50, 50);
            
            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(button, "Launch " + text.Replace(".lnk", "").Replace(".exe", ""));
            
            if (icon != null)
            {
                button.Image = icon.ToBitmap();
                button.Size = new Size((int)(button.Image.Size.Width * 1.5), (int)(button.Image.Size.Height * 1.5));
                _lastKnownSize = button.Size;
            }
            else
            {
                button.Size = new Size(_lastKnownSize.Width, _lastKnownSize.Height);
                button.Text = text;
            }

            return button;
        }

        private void SetupButtonClickEvent(string fullFilePath, Button button, string executablePath)
        {
            button.Click += (btnSender, args) =>
                                {
                                    try
                                    {
                                        ProcessStartInfo info = new ProcessStartInfo(executablePath);
                                        Process.Start(info);

                                    }
                                    catch (Exception exception)
                                    {
                                        try
                                        {
                                            ProcessStartInfo info = new ProcessStartInfo("java.exe");
                                            info.Arguments = "-jar " + executablePath;
                                        }
                                        catch (Exception newException)
                                        {
                                            MessageBox.Show("Unable to run excecutable at: " + executablePath +
                                                            ". Additona info: " + exception.Message);

                                            MessageBox.Show(newException.Message);

                                        }
                                    }

                                };
        }

        private static string GetFullExePath(FileInfo file)
        {
            string fullFilePath = string.Empty;

            if (file.Extension.ToUpper() == ".EXE")
            {
                fullFilePath = file.FullName;
            }
            else if (file.Extension.ToUpper() == ".LNK")
            {
                try
                {
                    fullFilePath = ExecutableUtility.GetShortcutTargetFile(file.FullName);
                }
                catch
                {
                    
                }
            }

            if(!string.IsNullOrEmpty(fullFilePath) && new FileInfo(fullFilePath).Extension.ToUpper() != ".EXE")
            {
                fullFilePath = string.Empty;
            }

            return fullFilePath;
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            LoadApps();
        }
    }
}
