using System;
using System.Xml;
using System.Windows;
using System.Diagnostics;

namespace BigBlue
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        App()
        {
            InitializeComponent();
        }
        
        [STAThread]
        static void Main(string[] args)
        {
            string procName = string.Empty;
            using (Process currentProcess = Process.GetCurrentProcess())
            {
                procName = currentProcess.ProcessName;
            }

            int processCount = 0;
            Process[] processes = Process.GetProcessesByName(procName);
            try
            {
                processCount = processes.Length;
            }
            finally
            {
                foreach (Process proc in processes)
                {
                    proc.Dispose();
                }

                processes = null;
            }

            if (processCount > 1)
            {
                MessageBox.Show(procName + " is already running!", "Big Blue");
                return;
            }
            else
            {
                App app = new App();

                string applicationDirectory = AppDomain.CurrentDomain.BaseDirectory;

                XmlNode config = FileHandling.GetNodeFromFile(applicationDirectory + @"\config.xml", "/config");

                bool hideErrors = false;

                // disable windows error reporting if hideerrors is set in the config file
                if (config["hideerrors"] != null)
                {                    
                    if (bool.TryParse(config["hideerrors"].InnerText, out hideErrors))
                    {
                        if (hideErrors && NativeMethods.GetWindowsErrorReportingDontShowUIValue() == 0)
                        {
                            Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\Windows Error Reporting", "DontShowUI", 1);
                        }
                    }
                }
                
                bool uiSelected = false;
                
                if (config["interface"] != null)
                {
                    string interFace = config["interface"].InnerText;
                    switch (interFace)
                    {
                        case "Desktop":                            
                            DesktopWindow bigBlueDesktopWindow = new DesktopWindow(applicationDirectory, config);
                            bigBlueDesktopWindow.hideWindowsErrorReportUI = hideErrors;

                            if (config["desktopicon"] != null)
                            {
                                string desktopIconPath = config["desktopicon"].InnerText;

                                if (!string.IsNullOrWhiteSpace(desktopIconPath))
                                {
                                    if (System.IO.File.Exists(desktopIconPath))
                                    {
                                        Uri customIconUri;
                                        if (Uri.TryCreate(desktopIconPath, UriKind.RelativeOrAbsolute, out customIconUri))
                                        {
                                            bigBlueDesktopWindow.Icon = ImageLoading.loadImageFromUri(customIconUri, null, null);
                                        }
                                    }
                                }
                            }
                            
                            OpenFrontendWindow(bigBlueDesktopWindow);
                            uiSelected = true;
                            break;

                        case "Custom":
                            // handle theme XML parsing errors
                            try
                            {
                                string themeDirectoryName = config["theme"]?.InnerText;

                                if (!string.IsNullOrWhiteSpace(themeDirectoryName))
                                {
                                    string templateDir = applicationDirectory + @"themes\" + themeDirectoryName;

                                    string templatePath = templateDir + @"\theme.xml"; 

                                    if (System.IO.File.Exists(templatePath))
                                    {
                                        XmlNode themeConfig = FileHandling.GetNodeFromFile(templatePath, "/theme");
                                        
                                        ThemeableWindow customBigBlueWindow = new ThemeableWindow(applicationDirectory, templateDir, config, themeConfig);
                                        customBigBlueWindow.hideWindowsErrorReportUI = hideErrors;

                                        OpenFrontendWindow(customBigBlueWindow);
                                        uiSelected = true;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            break;
                    }
                }

                if (!uiSelected)
                {
                    MainWindow classicBigBlueWindow = new MainWindow(applicationDirectory, config);
                    classicBigBlueWindow.hideWindowsErrorReportUI = hideErrors;
                    OpenFrontendWindow(classicBigBlueWindow);
                }
                
                app.Run();
            }
        }

        private static void OpenFrontendWindow(BigBlueWindow bbWindow)
        {
            // you always do the input states even if you can't get the controls 
            bbWindow.ProvisionInputStates();

            // get the configuration
            bbWindow.ReadConfigFile();            

            bbWindow.StartFrontend();
            
            // showing the window gets you its handle
            // once you have a handle, you can set the windows hooks
            bbWindow.Show();
            
            // set the hooks
            bbWindow.ProvisionWindowsHooks();

            if (bbWindow.hideWindowControls)
            {
                // wipe out the system menu entirely
                NativeMethods.SetWindowLong(bbWindow.windowHandle, NativeMethods.GWL_STYLE, NativeMethods.GetWindowLong(bbWindow.windowHandle, NativeMethods.GWL_STYLE) & ~NativeMethods.WS_SYSMENU);
            }

            NativeMethods.SetForegroundWindow(bbWindow.windowHandle);

            // initialize raw input
            NativeMethods.RegisterInputDevices(bbWindow);
            
            bbWindow.Activate();
            bbWindow.Topmost = true;
            bbWindow.Topmost = false;
            bbWindow.Focus();
            bbWindow.FocusBigBlue();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // hook on error before app really starts
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            //base.OnStartup(e);

            // render in software mode
            //System.Windows.Media.RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // put your tracing or logging code here (I put a message box as an example)
            MessageBox.Show(e.ExceptionObject.ToString());
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            
        }
    }
}
