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
            FileSystemWatcher spotlightImgWatcher = new FileSystemWatcher(spotlightPath)
            {
                NotifyFilter = NotifyFilters.FileName
            };
            spotlightImgWatcher.Created += OnFSChanged;

            // Start the watcher
            spotlightImgWatcher.EnableRaisingEvents = true;

            // Update the service state to Running.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(ServiceHandle, ref serviceStatus);
        }

        private void OnFSChanged(object sender, FileSystemEventArgs e)
        {
            // File that's been created might still be under access
            // by the writer, so wait for, say 5 seconds before reading it
            System.Threading.Thread.Sleep(5 * 1000);

            // Now read the file in question, get the first 2 bytes, 
            // if jpg | jpeg, save the file to the user's directory
            FileStream flHandle = File.OpenRead(e.FullPath);
            Byte[] arr = new Byte[2];
            flHandle.Read(arr, 0, 2);

            // Close the file 
            flHandle.Close();

            // JPG | JPEG has FF-FE as the first 2 'Magic' bytes
            if (arr[0] == 255 && arr[1] == 216)
            {
                // JPG | JPEG file, so save it 
                // Get the file name 
                string fileName = e.FullPath.Substring(e.FullPath.LastIndexOf("\\") + 1);
                string output = @"G:\Output\";
                output = string.Concat(output, fileName, ".jpg");
                System.IO.File.Copy(e.FullPath, output);
            }
        }

        protected override void OnStop()
        {
            // Do cleanup
        }

    }
}
