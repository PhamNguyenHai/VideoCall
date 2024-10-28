using NLog;
using System;

namespace PetProject.Repositories
{
    public class NLogLogger : ILoggerCustom
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public void LogInfo(string message)
        {
            logger.Info(message);
        }

        public void LogError(string message, Exception ex)
        {
            logger.Error(ex, message);
        }
    }
}
