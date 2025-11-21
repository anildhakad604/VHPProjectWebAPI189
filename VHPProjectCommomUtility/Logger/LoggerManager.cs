using NLog;

namespace VHPProjectCommonUtility.Logger
{
    public class LoggerManager : ILoggerManager
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();

        public void LogDebug(string message)
        {
            message = $"Thread Id: {Thread.CurrentThread.ManagedThreadId}:: {message}";
            logger.Debug(message);
        }

        public void LogError(string message,Exception ex)
        {
            message = $"Thread Id: {Thread.CurrentThread.ManagedThreadId}:: {message}";
            logger.Error(message);
        }

        public void LogException(Exception exception, string message)
        {
            message = $"Thread Id: {Thread.CurrentThread.ManagedThreadId}:: {message}";
            logger.Fatal(exception, message);
            //if (exception != null)
            //{
            //    LogError($"Error Message:{exception.Message}, \n stack Trace: {exception.StackTrace}");
            //}
        }

        public void LogInfo(string message)
        {
            message = $"Thread Id: {Thread.CurrentThread.ManagedThreadId}:: {message}";
            logger.Info(message);
        }

        public void LogWarn(string message)
        {
            message = $"Thread Id: {Thread.CurrentThread.ManagedThreadId}:: {message}";
            logger.Warn(message);
        }
    }

}