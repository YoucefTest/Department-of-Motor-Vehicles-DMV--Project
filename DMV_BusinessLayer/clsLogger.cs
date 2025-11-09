using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_Buisness
{
    //I grouped clsLogger and clsMethods inside one class file but in real world App separate classes better for maintainbility
    public class clsLogger
    {
        private readonly Action<string> _logger;
        public clsLogger(Action <string>logger)
        {
            _logger = logger;
        }
        public  void log(string message)
        {
            _logger(message);
        }
    }
    public static class clsLogMethods
    {
        public static void LogToFile(string message)
        {
            File.AppendAllText("log.txt", $"{DateTime.Now}: {message}{Environment.NewLine}");
        }

        public static void LogToConsole(string message)
        {
      //      Console.WriteLine($"{DateTime.Now}: {message}");
        }

        public static void LogToDatabase(string message)
        {
            // Simulated DB logging
    //        Console.WriteLine($"[DB LOG] {DateTime.Now}: {message}");
            // Real-world: use ADO.NET/EF to insert into a Logs table
        }
    }

}
