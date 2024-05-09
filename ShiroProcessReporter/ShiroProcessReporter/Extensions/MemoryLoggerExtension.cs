using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using ShiroProcessReporter.Providers;


namespace ShiroProcessReporter.Extensions
{
    public static class MemoryLoggerExtension
    {
        public static ILoggingBuilder AddMemory(this ILoggingBuilder builder)
        {
            builder.AddConfiguration();

            builder.Services.TryAddEnumerable(
                ServiceDescriptor.Singleton<ILoggerProvider, MemoryLoggerProvider>());

            return builder;
        }
    }
}
