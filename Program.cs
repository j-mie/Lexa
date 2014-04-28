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
        //Thanks to -- http://stackoverflow.com/a/22425211/1365365
        public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
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

        public static List<TimeSpan> TimeSpans = new List<TimeSpan>(); 
        public static List<Site> ProcessedSites = new List<Site>();
        static void Main(string[] args)
        {
            var sites = new List<Site>();
            
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;

            using (var sw = new StreamReader(@"top-1m.csv"))
            {
                sites.AddRange(from ln in sw.ReadToEnd().Split('\n') where ln != "" let lnno = Convert.ToInt32(ln.Split(',')[0]) let site = ln.Split(',')[1] select new Site(lnno, site));
            }


            const int sitesPerATask = 100;
            const int sleepTime = 200;
            int taskCount = sites.Count / sitesPerATask;
            Console.WriteLine("Using {0} tasks each crawling a total of {1} sites and sleeping inbetween for {2}MS", taskCount, sitesPerATask, sleepTime);

            for (var i = 0; i <= taskCount; i++)
            {
                var taskList = sites.Skip(i * sitesPerATask).Take(sitesPerATask);
                Console.WriteLine(taskList.Count());
                runLoop(taskList.ToList(), i, taskCount);
                Thread.Sleep(sleepTime);
            }            
            
            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private static async void runLoop(List<Site> sites, int taskID, int totalTasks)
        {
            var start = DateTime.Now;

            Console.WriteLine(start.ToString());
            Console.WriteLine("Running Task {0} of {1}", taskID, totalTasks);
            var tasks = sites.Select(site => ProcessSite(site, taskID)).ToList();
            
            await Task.WhenAll(tasks);
            
            var end = DateTime.Now;
            var ts = end - start;
            TimeSpans.Add(ts);

            foreach (var task in tasks)
            {
                ProcessedSites.Add(task.Result);
            }

            Console.WriteLine("Tasks complete");
        }

        private static async Task<Site> ProcessSite(Site site, int taskID)
        {
            var wc = new WebClient();
            wc.Proxy = null;
            try
            {
                site.Data = await wc.DownloadStringTaskAsync(new Uri("http://" + site.SiteName));
            }
            catch(Exception ex)
            {
                Console.WriteLine("Site: {0} has errored!", site.SiteName);
                site.Error = ex.ToString();
            }
            site.Headers = wc.ResponseHeaders;

            Console.WriteLine("(T:{0}) Completed {1}", taskID, site.SiteName);
            return site;
        }
    }
}
