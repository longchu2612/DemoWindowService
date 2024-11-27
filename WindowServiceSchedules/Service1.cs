using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace WindowServiceSchedules
{
    public partial class Service1 : ServiceBase
    {
        private System.Timers.Timer timer = null;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            timer = new System.Timers.Timer();

            timer.Interval = 60000;

            timer.Elapsed += timer_Tick;

            timer.Enabled = true;

            Utilities.WriteLogError("Test for 1st run WindowsService");
        }

        private async void timer_Tick(object sender, ElapsedEventArgs e)
        {
            Utilities.WriteLogError("Timer has ticked for doing something!!!");
            Console.WriteLine(sender);
            Console.WriteLine(e);

            List<ScheduleVM> schedules = await getAllSchedulesNotify(e.SignalTime);

            if(schedules == null)
            {
                return;
            }

            foreach (ScheduleVM schedule in schedules) {
            
                String notify = getNotifycation(schedule);
                
            }
        }

        public String getNotifycation(ScheduleVM scheduleVM)
        {
            return $"Sắp đến lịch hẹn: {scheduleVM.Reason}\nGiờ bắt đầu: {scheduleVM.FromX:D2}: {scheduleVM.FromY:D2}\n đến {scheduleVM.ToX:D2} : {scheduleVM.ToY:D2}";
        }

        public async Task<List<ScheduleVM>> getAllSchedulesNotify(DateTime dateTime)
        {
            HttpClient httpClient = new HttpClient();
            DateTime currentTime = new DateTime(dateTime.Year,dateTime.Month,dateTime.Day,0,0,0,0);
            string currentTimeStr = currentTime.ToString("yyyy-MM-dd");
            String link = $"http://localhost:5112/api/Schedules/getAllNotify?currentTime={currentTimeStr}";
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(link);
                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();

                    Console.WriteLine(responseData);
                    List<ScheduleVM> scheduleVMs = JsonConvert.DeserializeObject<List<ScheduleVM>>(responseData);
                    return scheduleVMs;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
        protected override void OnStop()
        {
            timer.Enabled = true;
            Utilities.WriteLogError("1st WindowsService has been stop");
        }
    }
}
