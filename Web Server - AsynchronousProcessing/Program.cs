
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto.Digests;

namespace WebServer
{
    class Program
    {
        static async Task Main(string[] args)
        {

            decimal money = 0;

            object lockObj = new object();//if the data in the threads are common Lock() it must be common

            var thread1 = new Thread(() =>
            {
                for (int i = 0; i < 10000; i++)
                {
                    lock (lockObj)//ensures that the thread will not run together at the same time 
                    {
                        money++;
                    }

                }
            });
            thread1.Start();
            var thread2 = new Thread(() =>
            {
                for (int i = 0; i < 10000; i++)
                {
                    lock (lockObj)
                    {
                        money++;
                    }
                }
            });
            thread2.Start();

            thread1.Join();
            thread2.Join();
            Console.WriteLine(money);

            var numbers = new ConcurrentQueue<int>(Enumerable.Range(0, 10000).ToList());

            for (int i = 0; i < 4; i++)
            {
                new Thread(() =>
                {
                    while (numbers.Count > 0)
                    {
                        numbers.TryDequeue(out _);
                    }
                }).Start();
            }
            //Task parallel Library(TPL)
            //in a good way
            //Promise

            //Task.Run(() =>
            //{
            //    for (int i = 0; i < 1000; i++)
            //    {
            //        Console.WriteLine(i);
            //    }
            //}).ContinueWith((previousTask) =>
            //{
            //    for (int i = 0; i < 1000; i++)
            //    {

            //    }
            //});

            //Async/ await

            try
            {
                var httpClient = new HttpClient();
                var httpResponse = await httpClient.GetAsync("https://softuni.bg");
                var result = await httpResponse.Content.ReadAsStringAsync();
                Console.WriteLine(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

            }

            //Data Parallelism & Concurrency

            var sw = Stopwatch.StartNew();
            var count = 0;
            var lockObject = new object();

            // for (int i = 0; i <= 500000; i++)
            Parallel.For(1, 500001, (i) =>
            {
                bool isPrime = true;
                for (int div = 2; div < Math.Sqrt(i); div++)
                {
                    if (i % div == 0)
                    {
                        isPrime = false;
                    }
                }

                if (isPrime)
                {
                    lock (lockObject)
                    {
                        count++;
                    }
                }
            });
            Console.WriteLine(count);
            Console.WriteLine(sw.Elapsed);

            //ConcurrentDictionary<string,int> kvp =new ConcurrentDictionary<string, int>();
            //ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
            // ConcurrentStack<string> stack = new ConcurrentStack<string>();

        }

        static async Task DownloadFileAsync()//download file from google disk
        {
            string path = "https://drive.google.com/uc?export=download&id=";//enter the id here

            Console.WriteLine("Downloading...");
            await Task.Run(() =>
            {
                using WebClient webClient = new WebClient();
                webClient.DownloadFile("path",
                    @"../../../myfile.pdf");
            });
            Console.WriteLine("Download successful.");
        }
    }
}
