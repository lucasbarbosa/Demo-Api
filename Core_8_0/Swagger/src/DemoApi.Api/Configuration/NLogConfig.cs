using NLog;
using NLog.Web;

namespace DemoApi.Api.Configuration
{
    public static class NLogConfig
    {
        #region Public Methods

        public static Logger AddNLogConfig(this WebApplicationBuilder builder)
        {
            var config = new NLog.Config.LoggingConfiguration();
            var nlogConfigPath = Path.Combine(AppContext.BaseDirectory, "nlog.config");

            try
            {
                if (File.Exists(nlogConfigPath))
                {
                    LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration(nlogConfigPath);
                }
                else
                {
                    throw new FileNotFoundException($"nlog.config was not found at: {nlogConfigPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while loading nlog.config: {ex.Message}");
                throw;
            }

            var logger = LogManager.GetCurrentClassLogger();

            builder.Logging.ClearProviders();
            builder.Host.UseNLog();

            return logger;
        }


        public static void Shutdown()
        {
            LogManager.Shutdown();
        }

        #endregion
    }
}