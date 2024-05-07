using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiroProcessReporter.Helper
{
    public class AppLogger
    {
        public static ILoggerFactory Factory { get; set; }

        public static void ConfigureLogging()
        {
            Factory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddDebug()
                    .AddConsole();
            });
        }
    }
}
