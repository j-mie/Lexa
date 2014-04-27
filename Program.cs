using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Lexa
{
    class Program
    {
        static void Main(string[] args)
        {
            var list = generateWebClients(1000);
            
            Console.WriteLine("Done");
            Console.ReadLine();
        }

        static List<WebClient> generateWebClients(int amount)
        {
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;

            var webClientList = new List<WebClient>();

            for (int i = 0; i < amount; i++)
            {
                var webclient = new WebClient();
                webclient.Proxy = null;
                webClientList.Add(webcli);
            }
            return webClientList;
        }
    }
}
