using System.ServiceProcess;

namespace Uranus.WindowsService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            #if (!DEBUG)
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new Service()
                };
                ServiceBase.Run(ServicesToRun);
            #else
                Service serviceCall = new Service();
                serviceCall.ReadWebServiceWEBJUR();
            #endif
        }
    }
}
