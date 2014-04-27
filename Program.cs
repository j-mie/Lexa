using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace Lexa
{
    class Program
    {
        public struct Site
        {
            public int Index;
            public string SiteName;
            public Site(int index, string site)
            {
                Index = index;
                SiteName = site;
            }
        }


        static void Main(string[] args)
        {
            var webclientList = GenerateWebClients(1000);
            var threadlist = GenerateThreads(1000, webclientList);

            var sites = new Dictionary<Site, bool>();
            using (var sw = new StreamReader(@"top-1m.csv"))
            {
                foreach (string ln in sw.ReadToEnd().Split('\n'))
                {
                    if (ln != "")
                    {
                        int lnno = Convert.ToInt32(ln.Split(',')[0]);
                        string site = ln.Split(',')[1];
                        Site s = new Site(lnno, site);
                    }
                }
            }

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
