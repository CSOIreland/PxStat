using API;
using Microsoft.AspNetCore.Components.Forms;
using PxStat;
using PxStatCore.Test;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PxStatXUnit.Tests
{
    [Collection("PxStatXUnit")]
    public class SemphoreCacheTests
    {
        [Fact]
        public void CheckTest()
        {
            Assert.True(true);
        }

        [Fact]
        public void BasicSemaphore()
        {
           
            TestDTO dto = new TestDTO() { Id = 111, Name = "Test1" };

            Helper.SetupTests();
            AppServicesHelper.CacheD.ResetTestReport();

            Dictionary<string, dynamic> semaConfig = new();
            semaConfig.Add("sema_lock_max", 5);
            semaConfig.Add("sema_sleep_add", 10);
            semaConfig.Add("sema_prefix", "_meta_");
            semaConfig.Add("sema_poll_interval", 200);

            MemCachedD_Value cache = AppServicesHelper.CacheD.Get_BSO<dynamic>(semaConfig,"TestNameSpace", "TestClass", "TestMethod", dto);
            Assert.True(AppServicesHelper.CacheD.GetTestReport().missCount.Equals(1));
            Assert.True(AppServicesHelper.CacheD.GetTestReport().queueCount.Equals(0));
            Assert.True(true);
        }
        [Fact]
        public void OneMissOneHit()
        {
            

            TestDTO dto = new TestDTO() { Id = 2, Name = "Test" };
            TestData data = new TestData() { Id = 11, Name = "TestData" };

            Helper.SetupTests();
            AppServicesHelper.CacheD.ResetTestReport();
            Dictionary<string, dynamic> semaConfig = new();
            semaConfig.Add("sema_lock_max", 5);
            semaConfig.Add("sema_sleep_add", 1);
            semaConfig.Add("sema_prefix", "_meta_");
            semaConfig.Add("sema_poll_interval", 200);

            MemCachedD_Value cache = AppServicesHelper.CacheD.Get_BSO<dynamic>(semaConfig, "TestNameSpace", "TestClass", "TestMethod", dto);
            Assert.True(AppServicesHelper.CacheD.GetTestReport().missCount.Equals(1));
            Assert.True(AppServicesHelper.CacheD.GetTestReport().queueCount.Equals(0));
            var a = AppServicesHelper.CacheD.GetTestReport();
            Thread.Sleep(1000);
            AppServicesHelper.CacheD.Store_BSO(semaConfig, "TestNameSpace", "TestClass", "TestMethod", dto, data, DateTime.Now.AddSeconds(60), null);
            MemCachedD_Value cache2 = AppServicesHelper.CacheD.Get_BSO<dynamic>(semaConfig, "TestNameSpace", "TestClass", "TestMethod", dto);
            var b = AppServicesHelper.CacheD.GetTestReport();
            Assert.True(AppServicesHelper.CacheD.GetTestReport().missCount.Equals(1));
            Assert.True(AppServicesHelper.CacheD.GetTestReport().hitCount.Equals(1));
            Assert.True(AppServicesHelper.CacheD.GetTestReport().queueCount.Equals(0));
            Assert.True(true);
        }

        [Fact]
        public void TwoMissOneWait()
        {
            

            TestDTO dto = new TestDTO() { Id = 3, Name = "Test" };
            TestData data = new TestData() { Id = 11, Name = "TestData" };

            Helper.SetupTests();
            AppServicesHelper.CacheD.ResetTestReport();
            Dictionary<string, dynamic> semaConfig = new();
            semaConfig.Add("sema_lock_max", 5);
            semaConfig.Add("sema_sleep_add", 1);
            semaConfig.Add("sema_prefix", "_meta_");
            semaConfig.Add("sema_poll_interval", 200); //milliseconds

            MemCachedD_Value cache = AppServicesHelper.CacheD.Get_BSO<dynamic>(semaConfig, "TestNameSpace", "TestClass", "TestMethod", dto);
            Assert.True(AppServicesHelper.CacheD.GetTestReport().missCount.Equals(1));
            Assert.True(AppServicesHelper.CacheD.GetTestReport().queueCount.Equals(0));
            var a = AppServicesHelper.CacheD.GetTestReport();

            MemCachedD_Value cache2 = AppServicesHelper.CacheD.Get_BSO<dynamic>(semaConfig, "TestNameSpace", "TestClass", "TestMethod", dto);
            var b = AppServicesHelper.CacheD.GetTestReport();
            Assert.True(AppServicesHelper.CacheD.GetTestReport().missCount.Equals(2));
            Assert.True(AppServicesHelper.CacheD.GetTestReport().queueCount.Equals(1));
            Assert.True(true);
        }

        [Fact]
        public void OneMissOneHitInterrupted()
        {
            
            Stopwatch sw = new();
            sw.Start();
            TestDTO dto = new TestDTO() { Id = 4, Name = "Test" };
            TestData data = new TestData() { Id = 11, Name = "TestData" };

            Helper.SetupTests();
            AppServicesHelper.CacheD.ResetTestReport();
            Dictionary<string, dynamic> semaConfig = new();
            semaConfig.Add("sema_lock_max", 120);
            semaConfig.Add("sema_sleep_add", 1);
            semaConfig.Add("sema_prefix", "_meta_");
            semaConfig.Add("sema_poll_interval", 1000);

            MemCachedD_Value cache = AppServicesHelper.CacheD.Get_BSO<dynamic>(semaConfig, "TestNameSpace", "TestClass", "TestMethod", dto);
            
            var a = AppServicesHelper.CacheD.GetTestReport();
            
            

            var otherThread = new Thread(() => 
            { 
                Thread.Sleep(8000);
                AppServicesHelper.CacheD.Store_BSO(semaConfig, "TestNameSpace", "TestClass", "TestMethod", dto, data, DateTime.Now.AddSeconds(60), null);
            
            });
            otherThread.Start();

            MemCachedD_Value cache2 = AppServicesHelper.CacheD.Get_BSO<dynamic>(semaConfig, "TestNameSpace", "TestClass", "TestMethod", dto);
            var val = cache2.data;
            var b = AppServicesHelper.CacheD.GetTestReport();
            sw.Stop();
            Assert.True(AppServicesHelper.CacheD.GetTestReport().missCount.Equals(1));
            Assert.True(AppServicesHelper.CacheD.GetTestReport().hitCount.Equals(1));
            Assert.True(AppServicesHelper.CacheD.GetTestReport().queueCount.Equals(1));
            Assert.True(AppServicesHelper.CacheD.GetTestReport().pollCount  <50);
            
            Assert.True(true);
        }

        [Fact]
        public void Stampede()
        {
            
            Stopwatch sw = new();
            sw.Start();
            TestDTO dto = new TestDTO() { Id = 5, Name = "Test" };
            TestData data = new TestData() { Id = 11, Name = "TestData" };

            Helper.SetupTests();
            AppServicesHelper.CacheD.ResetTestReport();
            Dictionary<string, dynamic> semaConfig = new();
            semaConfig.Add("sema_lock_max", 50);
            semaConfig.Add("sema_sleep_add", 1);
            semaConfig.Add("sema_prefix", "_meta_");
            semaConfig.Add("sema_poll_interval", 1000);

            List<dynamic> finalValues = new();

            

            var a = AppServicesHelper.CacheD.GetTestReport();

            var otherThread = new Thread(() =>
            {
                Thread.Sleep(8000);
                AppServicesHelper.CacheD.Store_BSO(semaConfig, "TestNameSpace", "TestClass", "TestMethod", dto, data, DateTime.Now.AddSeconds(60), null);

            });
            otherThread.Start();

            List<Thread> threads = new();
            for (int i=0;i<10;i++)
            {
                
                Thread.Sleep(50);
                MemCachedD_Value cache = AppServicesHelper.CacheD.Get_BSO<dynamic>(semaConfig, "TestNameSpace", "TestClass", "TestMethod", dto);
                if(cache.hasData)
                    finalValues.Add(cache.data);
                //The first read won't have any data because it is assumed it must go to the database
            }


            var b = AppServicesHelper.CacheD.GetTestReport();
            
            Assert.True((AppServicesHelper.CacheD.GetTestReport().hitCount + AppServicesHelper.CacheD.GetTestReport().missCount).Equals(10));
            Assert.True(finalValues.Count.Equals(9)); //The first read won't have any data because it is assumed it must go to the database
            foreach (var item in finalValues)
            {
                
                Assert.True(item.Id !=null);
            }

            Assert.True(true);
        }
    }


}
