using System;
using TQ.TQ_Engine;
using Rendering;
using System.IO;

namespace TQ
{

    public class Debugger
    {
        public static void Log(string message)
        {
            Program.currentEngine.Window.Title = message;
        }
    }

	public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        ///

        public static TQ_EngineRuntime currentEngine { get; private set; }

		public static void Main(string[] args)
        {
            currentEngine = new TQ_EngineRuntime();
            try
            {
                GameCache.Init();
                currentEngine.Run();
            }
            catch (Exception ex)
            {
                StreamWriter sw = new StreamWriter(@"C:\Debug.txt");
                sw.WriteLine(ex.StackTrace);
            }
        }
    }
}

