using Microsoft.Extensions.Logging;
using ShiroProcessReporter.Extensions;
using ShiroProcessReporter.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiroProcessReporter.Helper
{
    public class AppLogHelper
    {
        public static ILoggerFactory Factory { get; set; }

        public static void ConfigureLogging()
        {
            Factory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddDebug()
                    .AddConsole()
                    .AddMemory();
            });
        }
    }
}
