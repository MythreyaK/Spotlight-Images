using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.ServiceProcess;

namespace Spotlight_Images
{
    public partial class SpotlightImages : ServiceBase
    {
        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public int dwServiceType;
            public ServiceState dwCurrentState;
            public int dwControlsAccepted;
            public int dwWin32ExitCode;
            public int dwServiceSpecificExitCode;
            public int dwCheckPoint;
            public int dwWaitHint;
        };

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(System.IntPtr handle, ref ServiceStatus serviceStatus);

        public SpotlightImages()
        {
            InitializeComponent();
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        protected override void OnStart(string[] args)
        {
            // Initial set-up to update the service status 
            ServiceStatus serviceStatus = new ServiceStatus
            {
                dwWaitHint = 2 * 1000,
                dwCurrentState = ServiceState.SERVICE_START_PENDING
            };
            SetServiceStatus(ServiceHandle, ref serviceStatus);

            // Set up the file watcher on this directory, where 
            // spotlight images are saved
            string spotlightPath = @"C:\Users\{0}\AppData\Local\Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets\";

            // Get the username 
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT UserName FROM Win32_ComputerSystem");
            ManagementObjectCollection collection = searcher.Get();
            string userHost = (string)collection.Cast<ManagementBaseObject>().First()["UserName"];
            // username is after the / in userHost; index into it
            string username = userHost.Substring(userHost.IndexOf("\\") + 1);
            spotlightPath = String.Format(spotlightPath, username);

            // Add a filesystem watcher to that location
            FileSystemWatcher spotlightImgWatcher = new FileSystemWatcher(spotlightPath);
            spotlightImgWatcher.NotifyFilter = NotifyFilters.LastWrite;
            spotlightImgWatcher.Created += OnFSChanged;
            spotlightImgWatcher.Changed += OnFSChanged;

            // Start the watcher
            spotlightImgWatcher.EnableRaisingEvents = true;

            // Update the service state to Running.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(ServiceHandle, ref serviceStatus);
        }

        private void OnFSChanged(object sender, FileSystemEventArgs e)
        {
            // Read the file in question, get the first few bytes, 
            // if jpg | jpeg, save the file to the users directory

        }


        protected override void OnStop()
        {
            // Do cleanup
        }

    }
}
