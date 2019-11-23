using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerConsole
{
    class Program
    {
        static ServerThread thread;
        static void Main(string[] args)
        {
            thread = new ServerThread(8989);
            thread.Start();
            Console.ReadKey();
            thread.Stop();
        }
    }
}
