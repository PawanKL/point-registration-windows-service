using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace StudentPaymentService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                StudenPaymentService service1 = new StudenPaymentService(args);
                service1.TestStartupAndStop(args);
            }
            // production mode (windows app)
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new StudenPaymentService()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
