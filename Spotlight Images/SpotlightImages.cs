using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Spotlight_Images
{
    public partial class SpotlightImages : ServiceBase
    {
        public SpotlightImages()
        {
            InitializeComponent();

            eventLog1 = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("MySource"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "Spotlight Images", "New Log");
            }
            eventLog1.Source = "Spotlight Images";
            eventLog1.Log = "SI New Log";
        }

        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("On Start");
            System.Timers.Timer timer = new System.Timers.Timer
            {
                Interval = 60 * 1000 // 60 seconds
            };
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTImer);
            timer.Start();
            eventLog1.WriteEntry("Timer started, exiting OnStart...");

        }

        private void OnTImer(object sender, ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry("On Stop");
        }
    }
}
