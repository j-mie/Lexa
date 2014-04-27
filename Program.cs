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
        public class Site
        {
            public int Index;
            public string SiteName;
            public int Progress { get; set; }
            public WebHeaderCollection Headers{ get; set; }
            public Site(int index, string site)
            {
                Index = index;
                SiteName = site;
            }
        }


        static void Main(string[] args)
        {
            var sites = new List<Site>();

            using (var sw = new StreamReader(@"top-1m.csv"))
            {
                foreach (string ln in sw.ReadToEnd().Split('\n'))
                {
                    if (ln != "")
                    {
                        int lnno = Convert.ToInt32(ln.Split(',')[0]);
                        string site = ln.Split(',')[1];
                        Site s = new Site(lnno, site);
                        sites.Add(s);
                    }
                }
            }

            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            
            runLoop(sites);
            
            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private static async void runLoop(List<Site> sites)
        {
            var tasks = sites.Select(site => ProcessSite(site)).ToList();
            await Task.WhenAll(tasks);
            Console.WriteLine("site complete");
        }

        private static async Task<Site> ProcessSite(Site site)
        {
            Console.WriteLine(site.SiteName);
            WebClient wc = new WebClient();
            wc.Proxy = GlobalProxySelection.GetEmptyWebProxy();
            
            var data = await wc.DownloadStringTaskAsync(new Uri(site.SiteName));

            site.Headers = wc.ResponseHeaders;
            return site;
        }
    }
}
