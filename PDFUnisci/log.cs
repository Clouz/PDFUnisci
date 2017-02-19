using System;
using System.Collections.Generic;

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

    abstract class LogBase
    {
        public abstract void Log(string message, LogType type);
    }

    class ConsoleLogger : LogBase
    {
        public override void Log(string message, LogType type)
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
                LogHelper.ErrorLog.Add(message);
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
        public static List<string> ErrorLog = new List<string>();

        public static void Log(string message, LogTarget target = LogTarget.Console, LogType type = LogType.Normal)
        {
            switch (target)
            {
                case LogTarget.Console:
                logger = new ConsoleLogger();
                logger.Log(message,  type);
                break;

                case LogTarget.EventLog:
                //todo
                break;

                case LogTarget.File:
                //todo
                break;
            }
        }

        public static void Log(string message, LogType type)
        {
            Log(message, LogTarget.Console, type);
        }

    }
}