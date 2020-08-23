using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Resources;
using SailawayGribConverter.Properties;

public class Form1 : Form
{
    private NotifyIcon notifyIcon1;
    private ContextMenu contextMenu1;
    private MenuItem menuItemExit;
    private MenuItem menuItemPause;
    private MenuItem menuItemTimeshift;
    private MenuItem menuItemTimeshift1;
    private MenuItem menuItemTimeshift2;
    private MenuItem menuItemTimeshift3;
    private MenuItem menuItemTimeshift4;
    private MenuItem menuItemTimeshift5;
    private MenuItem menuItemTimeshift6;
    private MenuItem menuSeparator;
    private IContainer components;
    private static string gribDirectory = @"C:\Program Files\qtVlm\grib";
    private static string wgrib = @"C:\tools\wgrib\wgrib2.exe";
    private FileSystemWatcher newGribFile;
    private Boolean paused = false;
    private static string TimeshiftSpan = "3";

    [STAThread]
    static void Main()
    {
        try
        {
            Application.Run(new Form1());
        }
        catch
        {
            //
        }
    }

    public Form1()
    {
        string[] args = Environment.GetCommandLineArgs();
        try
        {
            gribDirectory = args[1];
            wgrib = args[2];
        }
        catch
        {
            
        }

        if (!CheckConditions())
        {
            Dispose();
            Application.Exit();
        }
        
        TimeshiftSpan = Settings.Default.TimeshiftSpan;

        components = new Container();
        contextMenu1 = new ContextMenu();
        menuItemExit = new MenuItem();
        menuItemPause = new MenuItem();
        menuItemTimeshift = new MenuItem();
        menuItemTimeshift1 = new MenuItem();
        menuItemTimeshift2 = new MenuItem();
        menuItemTimeshift3 = new MenuItem();
        menuItemTimeshift4 = new MenuItem();
        menuItemTimeshift5 = new MenuItem();
        menuItemTimeshift6 = new MenuItem();
        menuSeparator = new MenuItem("-");

        switch (TimeshiftSpan)
        {
            case "1":
                menuItemTimeshift1.Checked = true;
                break;
            case "2":
                menuItemTimeshift2.Checked = true;
                break;
            case "3":
                menuItemTimeshift3.Checked = true;
                break;
            case "4":
                menuItemTimeshift4.Checked = true;
                break;
            case "5":
                menuItemTimeshift5.Checked = true;
                break;
            case "6":
                menuItemTimeshift6.Checked = true;
                break;
        }


        // Initialize menuItemExit
        menuItemExit.Index = 2;
        menuItemExit.Text = "E&xit";
        menuItemExit.Click += new EventHandler(MenuItemExit_Click);

        // Initialize menuItemPause
        menuItemPause.Index = 1;
        menuItemPause.Text = "&Pause";
        menuItemPause.Click += new EventHandler(MenuItemPause_Click);

        // Initialize menuItemTimeshift
        menuItemTimeshift.Index = 0;
        menuItemTimeshift.Text = "&Timeshift";
        menuItemTimeshift1.Text = "+1 hour";
        menuItemTimeshift2.Text = "+2 hours";
        menuItemTimeshift3.Text = "+3 hours";
        menuItemTimeshift4.Text = "+4 hours";
        menuItemTimeshift5.Text = "+5 hours";
        menuItemTimeshift6.Text = "+6 hours";
        menuItemTimeshift1.Click += new EventHandler(menuItemTimeshift1_Click);
        menuItemTimeshift2.Click += new EventHandler(menuItemTimeshift2_Click);
        menuItemTimeshift3.Click += new EventHandler(menuItemTimeshift3_Click);
        menuItemTimeshift4.Click += new EventHandler(menuItemTimeshift4_Click);
        menuItemTimeshift5.Click += new EventHandler(menuItemTimeshift5_Click);
        menuItemTimeshift6.Click += new EventHandler(menuItemTimeshift6_Click);
        menuItemTimeshift.MenuItems.Add(menuItemTimeshift1);
        menuItemTimeshift.MenuItems.Add(menuItemTimeshift2);
        menuItemTimeshift.MenuItems.Add(menuItemTimeshift3);
        menuItemTimeshift.MenuItems.Add(menuItemTimeshift4);
        menuItemTimeshift.MenuItems.Add(menuItemTimeshift5);
        menuItemTimeshift.MenuItems.Add(menuItemTimeshift6);
        
        // Initialize contextMenu1
        contextMenu1.MenuItems.AddRange(
                    new MenuItem[] { menuItemTimeshift, menuSeparator, menuItemPause, menuItemExit });

        // Set up how the form should be displayed.
        this.ClientSize = new System.Drawing.Size(292, 266);
        this.Text = "Sailaway Grib File Converter";

        // Create the NotifyIcon.
        notifyIcon1 = new NotifyIcon(components);

        // The Icon property sets the icon that will appear
        // in the systray for this application.
        notifyIcon1.Icon = Resources.autorenew_black_18dp;

        // The ContextMenu property sets the menu that will
        // appear when the systray icon is right clicked.
        notifyIcon1.ContextMenu = this.contextMenu1;

        // The Text property sets the text that will be displayed,
        // in a tooltip, when the mouse hovers over the systray icon.
        notifyIcon1.Text = "Sailaway Grib File Converter";
        notifyIcon1.Visible = true;


        // Hide Form
        ShowInTaskbar = false;
        WindowState = FormWindowState.Minimized;
        Hide();

        notifyIcon1.BalloonTipTitle = "Sailaway Grib Converter";
        notifyIcon1.BalloonTipText = "Sucessfully converted grib file, reload it in qtVLM.";
        notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;

        // Initialize Grib Conversion FileSystemWatcher
        newGribFile = new FileSystemWatcher
        {
            Path = gribDirectory,
            Filter = "*.grb2"
        };
        newGribFile.Created += FileSystemWatcher_Created;
        newGribFile.EnableRaisingEvents = true;
    }

    private void menuItemTimeshift6_Click(object sender, EventArgs e)
    {
        TimeshiftSpan = "6";
        SaveSettings();
        menuItemTimeshift6.Checked = true;
    }

    private void menuItemTimeshift5_Click(object sender, EventArgs e)
    {
        TimeshiftSpan = "5";
        SaveSettings();
        menuItemTimeshift5.Checked = true;
    }

    private void menuItemTimeshift4_Click(object sender, EventArgs e)
    {
        TimeshiftSpan = "4";
        SaveSettings();
        menuItemTimeshift4.Checked = true;
    }

    private void menuItemTimeshift3_Click(object sender, EventArgs e)
    {
        TimeshiftSpan = "3";
        SaveSettings();
        menuItemTimeshift3.Checked = true;
    }

    private void menuItemTimeshift2_Click(object sender, EventArgs e)
    {
        TimeshiftSpan = "2";
        SaveSettings();
        menuItemTimeshift2.Checked = true;
    }

    private void menuItemTimeshift1_Click(object sender, EventArgs e)
    {
        TimeshiftSpan = "1";
        SaveSettings();
        menuItemTimeshift1.Checked = true;
    }

    private Boolean CheckConditions()
    {
        Boolean terminate = false;
        string messageText = "";
        if (!File.Exists(wgrib))
        {
            messageText = "The file wgrib2.exe was not found.\n";
            terminate = true;
        }
        if (!Directory.Exists(gribDirectory))
        {
            messageText += "The grib directory was not found.\n";
            terminate = true;
        }
        if (!HasWritePermissionOnDir(gribDirectory))
        {
            messageText += "You don´t have write permission in the grib directory " + gribDirectory + "\n";
            terminate = true;
        }
    
        if (terminate) {
            MessageBox.Show(messageText, "Sailaway Grib Converter Error Messages", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
        else
        {
            return true;
        }
    }

    protected override void Dispose(bool disposing)
    {
        // Clean up any components being used.
        if (disposing)
            if (components != null)
                components.Dispose();
        base.Dispose(disposing);
    }


    private void MenuItemExit_Click(object Sender, EventArgs e)
    {
        // Close the form, which closes the application.
        this.Close();
    }


    private void MenuItemPause_Click(object Sender, EventArgs e)
    {
        // Close the form, which closes the application.
        if (paused == false)
        {
            menuItemPause.Text = "&Resume";
            newGribFile.EnableRaisingEvents = false;
            paused = true;
        }
        else
        {
            menuItemPause.Text = "&Pause";
            newGribFile.EnableRaisingEvents = true;
            paused = false;
        }
        
    }

    private void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
    {
        // Deactivate file system watcher
        newGribFile.EnableRaisingEvents = false;
        // Wait a second, just to be sure file is safely written
        Thread.Sleep(1000);
        // set temporary file name for converted file
        string newFilename = string.Concat(Path.GetFileNameWithoutExtension(e.Name), "_temp", Path.GetExtension(e.Name));
        // wgrib2 command line arguments
        string args = String.Concat("\"", e.Name, "\"", " -set_date +", TimeshiftSpan, "hr -grib ", "\"", newFilename, "\"");
        // initialize process
        ProcessStartInfo startInfo = new ProcessStartInfo(wgrib);
        startInfo.Arguments = args;
        startInfo.WorkingDirectory = gribDirectory;
        startInfo.CreateNoWindow = true;
        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
        try
        {
            Process p = Process.Start(startInfo);
            p.WaitForExit();
        }
        catch
        {
            MessageBox.Show("Failed to start " + wgrib, "Sailaway Grib Converter", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        // Delete old file
        try
        {
            File.Delete(e.FullPath);
        }
        catch
        {
            MessageBox.Show("Failed to delete " + e.FullPath, "Sailaway Grib Converter", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        // Move converted file to original file name
        File.Move(Path.Combine(gribDirectory, newFilename), e.FullPath);
        // Notificate the user
        notifyIcon1.Visible = true;
        notifyIcon1.ShowBalloonTip(60000);
        // Wait some seconds before reactivating file system watcher
        Thread.Sleep(5000);
        newGribFile.EnableRaisingEvents = true;
    }

    private static bool HasWritePermissionOnDir(string path)
    {
        var writeAllow = false;
        var writeDeny = false;
        var accessControlList = Directory.GetAccessControl(path);
        if (accessControlList == null)
            return false;
        var accessRules = accessControlList.GetAccessRules(true, true,
                                    typeof(System.Security.Principal.SecurityIdentifier));
        if (accessRules == null)
            return false;

        foreach (FileSystemAccessRule rule in accessRules)
        {
            if ((FileSystemRights.Write & rule.FileSystemRights) != FileSystemRights.Write)
                continue;

            if (rule.AccessControlType == AccessControlType.Allow)
                writeAllow = true;
            else if (rule.AccessControlType == AccessControlType.Deny)
                writeDeny = true;
        }

        return writeAllow && !writeDeny;
    }

    private static void SaveSettings()
    {
        Settings.Default.TimeshiftSpan = TimeshiftSpan;
        Settings.Default.Save();
    }
}