using Autofac;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Server.Standalone.IoC.Modules
{
    public static class LoggerInjection
    {
        public static ContainerBuilder AddLogger(this ContainerBuilder builder, IConfigurationRoot configuration)
        {
            var loggerConfig = new LoggerConfiguration()
              .ReadFrom.Configuration(configuration, sectionName: "Log")
              .WriteTo.Console(theme: AnsiConsoleTheme.Code);

            var logger = loggerConfig.CreateLogger();

            builder.RegisterInstance(logger).SingleInstance();
            builder.RegisterInstance(loggerConfig).SingleInstance();
            return builder;
        }
    }
}
