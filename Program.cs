using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lexa
{
    class Program
    {
        static void Main(string[] args)
        {
            var webclientList = GenerateWebClients(1000);
            var threadlist = GenerateThreads(1000, webclientList);
            
            threadlist[2].Start();



            Console.WriteLine("Done");
            Console.ReadLine();
        }

        static List<WebClient> GenerateWebClients(int amount)
        {
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;

            var webClientList = new List<WebClient>();

            for (int i = 0; i < amount; i++)
            {
                var webclient = new WebClient();
                webclient.Proxy = null;
                webClientList.Add(webclient);
            }
            return webClientList;
        }

        static List<Thread> GenerateThreads(int amount, List<WebClient> webclientList)
        {
            var threadList = new List<Thread>();

            for (int i = 0; i < amount; i++)
            {
                Console.WriteLine(i);
                int temp = i;
                var thread = new Thread(() => ThreadWorker(temp, webclientList[temp]));

                threadList.Add(thread);
            }
            return threadList;
        }



        static void ThreadWorker(object i, WebClient webclient)
        {
            Console.WriteLine(i);   
        }
    }
}
