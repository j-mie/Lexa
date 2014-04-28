using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        // Thanks to -- http://stackoverflow.com/a/6822458/1365365
        public class CMCStopWatch : Stopwatch
        {
            /// <summary>
            /// Gets ETA based off of stopwatch the specified counter.
            /// </summary>
            /// <param name="counter">The this is what line you are on</param>
            /// <param name="counterGoal">this is the total lines</param>
            /// <returns></returns>
            public TimeSpan eta(int counter, int counterGoal)
            {
                float elapsedMin = ((float)this.ElapsedMilliseconds / 1000) / 60;
                float minLeft = (elapsedMin / counter) * (counterGoal - counter);
                TimeSpan ret = TimeSpan.FromMinutes(minLeft);
                return ret;
            }
        }

        public class Site
        {
            public int Index;
            public string Data;
            public string SiteName;
            public string Error;
            public WebHeaderCollection Headers{ get; set; }
            public Site(int index, string site)
            {
                Index = index;
                SiteName = site;
            }
        }

        public static int poo = 0;

        private static void Main(string[] args)
        {
            var sites = new List<Site>();

            ServicePointManager.DefaultConnectionLimit = int.MaxValue;

            //using (var sw = new StreamReader(@"top-1m.csv"))
            //{
            //    sites.AddRange(from ln in sw.ReadToEnd().Split('\n') where ln != "" let lnno = Convert.ToInt32(ln.Split(',')[0]) let site = ln.Split(',')[1] select new Site(lnno, site));
            //}

            sites.Add(new Site(1, "google.com"));
            sites.Add(new Site(2, "google.co.uk"));
            sites.Add(new Site(3, "jamiehankins.co.uk"));
            sites.Add(new Site(4, "bing.com"));
            sites.Add(new Site(5, "reddit.com"));

            var done = scrapeTask(sites);

            foreach (var site in done)
            {
                Console.WriteLine("{0} : {1} : {2}", site.Index, site.SiteName, site.Headers[1]);
            }

            Console.WriteLine("Done!!!!!");
            Console.ReadLine();
        }

        private static List<Site> scrapeTask(List<Site> sites)
        {
            const int sitesPerATask = 1;
            const int sleepTime = 1;
            int taskCount = sites.Count / sitesPerATask;
            Console.WriteLine("Using {0} tasks each crawling a total of {1} sites and sleeping inbetween for {2}MS", taskCount, sitesPerATask, sleepTime);

            var taskResults = new List<List<Site>>();

            CMCStopWatch sw = new CMCStopWatch();
            sw.Start();

            for (var i = 0; i < taskCount; i++)
            {
                var taskList = sites.Skip(i * sitesPerATask).Take(sitesPerATask);
                taskResults.Add(ListProcessor(taskList.ToList(), i, taskCount).Result);
                Thread.Sleep(sleepTime);
                Console.WriteLine("{0} minutes remaining", sw.eta(1, taskCount).Minutes);
            }

            return taskResults.SelectMany(siteList => siteList).ToList();
        }

        private static async Task<List<Site>>  ListProcessor(List<Site> sites, int taskID, int totalTasks)
        {
            Console.WriteLine("Running Task {0} of {1}", taskID, totalTasks);
            var tasks = sites.Select(site => ProcessSite(site, taskID)).ToList();
            await Task.WhenAll(tasks);
            return tasks.Select(t => t.Result).ToList();
        }

        private static async Task<Site> ProcessSite(Site site, int taskID)
        {
            var wc = new WebClient();
            wc.Proxy = null;

            int timeout = 5000;
            try
            {
                Task<string> ts = wc.DownloadStringTaskAsync(new Uri("http://" + site.SiteName));
                if (await Task.WhenAny(ts, Task.Delay(timeout)) == ts)
                {
                    site.Data = ts.Result;
                }
                else
                {
                    wc.CancelAsync();
                    site.Error = "Timed out";
                    return site;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Site: {0} has errored!", site.SiteName);
                site.Error = ex.ToString();
            }

            site.Headers = wc.ResponseHeaders;
            Console.WriteLine("(T:{0} - Site: {1}) Completed {2}", taskID + 1,site.Index, site.SiteName);
            return site;
        }
    }
}
