using System;

namespace LogManager
{
    public enum LogTarget
    {
        File, EventLog, Console
    }

    public enum LogType 
    {
        Error, Normal, Successful
    }

    public abstract class LogBase
    {
        public abstract void Log(LogType type, string message);
    }

    public class ConsoleLogger : LogBase
    {
        public override void Log(LogType type, string message)
        {
            switch (type)
            {
                case LogType.Normal:
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(message);
                Console.ResetColor();
                break;

                case LogType.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(message);
                Console.ResetColor();
                break;

                case LogType.Successful:
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(message);
                Console.ResetColor();
                break;
            }
        }
    }


    public static class LogHelper
    {
        private static LogBase logger = null;

        public static void Log(LogTarget target, LogType type, string message)
        {
            switch (target)
            {
                case LogTarget.Console:
                logger = new ConsoleLogger();
                logger.Log( type, message);
                break;

                case LogTarget.EventLog:
                //todo
                break;

                case LogTarget.File:
                //todo
                break;
            }
        }
    }
}