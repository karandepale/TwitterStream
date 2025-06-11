using Serilog;
using Serilog.Events;

namespace WebApi
{
    public class LoggingFactory
    {

        public static void ConfigureLogging()
        {
            //Log.Logger = new LoggerConfiguration()
            //    .WriteTo.Console()
            //    .WriteTo.File("Logs/Logging.txt", rollingInterval: RollingInterval.Day)
            //    .WriteTo.Seq("http://localhost:5341") // Or your Seq server address
            //    .Enrich.FromLogContext()
            //    .CreateLogger();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("Logs/AllLogs.txt", rollingInterval: RollingInterval.Day)
                .WriteTo.File("Logs/ErrorLogs.txt", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Error)
                .WriteTo.Seq("http://localhost:5341") // All logs go to Seq
                .Enrich.FromLogContext()
                .CreateLogger();
        }

    }
}
