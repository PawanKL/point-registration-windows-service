using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Timers;
using PointDataAccess;
using System.Linq;
using System.Data.Entity;

namespace StudentPaymentService
{
    public partial class StudenPaymentService : ServiceBase
    {
        private int eventId = 1;
        public string logSource = ConfigurationManager.AppSettings["LogSource"];
        public int minutes = Convert.ToInt32(ConfigurationManager.AppSettings["Minutes"]);
        public int hours = Convert.ToInt32(ConfigurationManager.AppSettings["Hours"]);
        private bool isDev;
        private string[] args;
        public StudenPaymentService()
        {
            this.isDev = false;
            InitializeComponent();
        }

        public StudenPaymentService(string[] args)
        {
            this.isDev = true;
            this.args = args;
        }

        protected override void OnStart(string[] args)
        {
            LogService("In OnStart.");
            // For Development Mode
            if (this.isDev)
            {
                doThis();

            }
            // For Production Mode
            else
            {
                Timer timer = new Timer();
                if(hours == 0) 
                    timer.Interval = 1000 * 60 * minutes;
                else
                    timer.Interval = 1000 * 60 * hours;
                timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
                timer.Start();
            }
        }
        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            LogService("Service Working: " + (eventId++).ToString());
            using (PointDBEntities entities = new PointDBEntities())
            {
                // today Date
                DateTime today = DateTime.Today;
                // select all point Registrations where days difference is 7
                var studentPoints = (from sp in entities.StudentPoints
                                     where DbFunctions.DiffDays(sp.RegistrationDate, today) == 7
                                     select sp);
                // select all point Registrations where fees is not paid within 7 days
                var toRemove = (from sp in studentPoints
                                where !entities.PointPayments.Any(p => p.StudentID == sp.StudentID)
                                select sp);
                // deleting all point Registrations where fees is not paid within 7 days
                foreach (var s in toRemove)
                {
                   
                    LogService("StudentPointID = " + s.StudentPointId.ToString() + " StudentID = " + s.StudentID.ToString() + " Deleted Successfully!!!");
                    var pointEntity = entities.Points.FirstOrDefault(p => p.PointID == s.PointID);

                    LogService("Increasing Seats of PointID = " + pointEntity.PointID.ToString() + " From NumberOfSeats = " + pointEntity.NumberOfSeats.ToString() + " To NumberOfSeats = " + (pointEntity.NumberOfSeats + 1).ToString());

                    pointEntity.NumberOfSeats = pointEntity.NumberOfSeats + 1;
                    entities.StudentPoints.Remove(s);

                }

                try
                {
                    entities.SaveChanges();
                }
                catch (Exception ex)
                {
                    LogService(ex.ToString());
                }

            }

        }

        protected override void OnStop()
        {
            LogService("Service Stop");
        }
        public void doThis()
        {
            using(PointDBEntities entities = new PointDBEntities())
            {
                // today Date
                DateTime today = DateTime.Today;
                // select all point Registrations where days difference is 7
                var studentPoints = (from sp in entities.StudentPoints
                                   where DbFunctions.DiffDays(sp.RegistrationDate, today) == 7
                                   select sp);
                Console.WriteLine(studentPoints.Count());
                foreach (var s in studentPoints)
                {
                    Console.WriteLine(s.RegistrationDate);
                    Console.WriteLine(s.PointID);
                }
                // select all point Registrations where fees is not paid within 7 days
                var toRemove = (from sp in studentPoints
                                where !entities.PointPayments.Any(p => p.StudentID == sp.StudentID)
                                select sp);
                Console.WriteLine(toRemove.Count());
                foreach (var s in toRemove)
                {
                    Console.WriteLine(s.StudentID);
                    Console.WriteLine(s.PointID);
                }
                // deleting all point Registrations where fees is not paid within 7 days
                foreach (var s in toRemove)
                {
                    Console.WriteLine("StudentPointID = " + s.StudentPointId.ToString() + " StudentID = " + s.StudentID.ToString() + " Deleted Successfully!!!");
                    var pointEntity = entities.Points.FirstOrDefault(p => p.PointID == s.PointID);

                    Console.WriteLine("Increasing Seats of PointID = " + pointEntity.PointID.ToString() + " From NumberOfSeats = " + pointEntity.NumberOfSeats.ToString() + " To NumberOfSeats = " + (pointEntity.NumberOfSeats + 1).ToString());

                    pointEntity.NumberOfSeats = pointEntity.NumberOfSeats + 1;
                    entities.StudentPoints.Remove(s);

                }
                try
                {
                    entities.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }


            }

        }
        internal void TestStartupAndStop(string[] args)
        {
            this.OnStart(args);
            Console.ReadLine();
            this.OnStop();
        }
        private void LogService(string content)
        {
            if (!Directory.Exists(logSource + "Logs"))
            {
                Directory.CreateDirectory(logSource + "Logs");
            }
            string logPath = logSource + "Logs" + "/StudentPaymentService.txt";
            FileStream fs = new FileStream(logPath, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.BaseStream.Seek(0, SeekOrigin.End);
            sw.WriteLine(content);
            sw.Flush();
            sw.Close();
        }
    }
}
