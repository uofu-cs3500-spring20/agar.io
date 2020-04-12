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

            ServiceCollection services = new ServiceCollection();
            using CustomFileLogProvider provider = new CustomFileLogProvider();
            services.AddLogging(configure =>
            {
                
                configure.AddProvider(provider);
                configure.SetMinimumLevel(LogLevel.Debug);

            });
            using ServiceProvider serviceProvider = services.BuildServiceProvider();

            ILogger logger = serviceProvider?.GetRequiredService<ILogger<view>>();
            Application.Run(new view(logger));
           
        }
    }
}
