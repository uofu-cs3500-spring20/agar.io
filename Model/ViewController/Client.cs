/// <summary> 
/// Author:    Gabriel Job && CS 3500 staff 
/// Partner:   N/A
/// Date:      4/13/20
/// Course:    CS 3500, University of Utah, School of Computing 
/// Copyright: CS 3500 and Gabe - This work may not be copied for use in Academic Coursework. 
/// 
/// I, Gabe, certify that I wrote this code from scratch and did not copy it in part or whole from  
/// another source.  All references used in the completion of the assignment are cited in my README file. 
/// </summary>

using FileLogger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ViewController
{
    /// <summary>
    /// Entry class for application.
    /// </summary>
    static class Client
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Logger setup
            ServiceCollection services = new ServiceCollection();
            using CustomFileLogProvider provider = new CustomFileLogProvider();
            services.AddLogging(configure =>
            {                
                configure.AddProvider(provider);
                configure.SetMinimumLevel(LogLevel.Debug);

            });
            using ServiceProvider serviceProvider = services.BuildServiceProvider();
            ILogger logger = serviceProvider?.GetRequiredService<ILogger<view>>();

            //Run the app
            Application.Run(new view(logger));
        }
    }
}
