using Microsoft.Extensions.Caching.Memory;
using PxStat;
using PxStat.Data;
using PxStat.Data.Px;
using PxStat.DataStore;
using PxStat.DBuild;
using PxStat.JsonStatSchema;
using PxStat.Subscription;
using PxStatCore.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace PxStatXUnit.Tests
{
    [Collection("PxStatXUnit")]
    public class DITest
    {
        [Fact]
        public void MemcacheDTest()
        {
            Helper.SetupTests();

            var cache=AppServicesHelper.CacheD;
            
            TestMatrixFactory tmf = new TestMatrixFactory();
            string testData = tmf.TestPxfileMim04PcsHeadingStubReversed();
            string storeKey = tmf.CubeQueryDtoStringParameters();
            cache.Store_BSO("abc","def","ghi",storeKey,testData,DateTime.Now.AddDays(2),"repo");
            Thread.Sleep(1000);
            var cacheRead = cache.Get_BSO("abc", "def", "ghi", storeKey);
            Assert.True(cacheRead.hasData);
            Assert.True(cacheRead.data.Value.Equals(testData));
            cache.CasRepositoryFlush("repo");
            Thread.Sleep(1000);
            cacheRead = cache.Get_BSO("abc", "def", "ghi", storeKey);
            Assert.False(cacheRead.hasData);
            
        }

        [Fact]
        public void MemcacheDTestStacked()
        {
            Helper.SetupTests();

            TestMatrixFactory tmf = new TestMatrixFactory();
            string testData = tmf.TestPxfileMim04PcsHeadingStubReversed();
            string storeKey = tmf.CubeQueryDtoStringParameters();

            var cache = AppServicesHelper.CacheD;

            for (int i = 0; i < 2; i++)
            {
                
                cache.Store_BSO("abc", "def", "ghi", storeKey, testData, DateTime.Now.AddDays(2), "repo");
                Thread.Sleep(1000);
                var cacheRead = cache.Get_BSO("abc", "def", "ghi", storeKey);
                Assert.True(cacheRead.hasData);
                Assert.True(cacheRead.data.Value.Equals(testData));
                cache.CasRepositoryFlush("repo");
                Thread.Sleep(1000);
                cacheRead = cache.Get_BSO("abc", "def", "ghi", storeKey);
                Assert.False(cacheRead.hasData);
            }
        }

        [Fact]
        public void MemcacheDTestNullWriteAndRead() {
            Helper.SetupTests();
            string? testData = null;
            string storeKey = "test";

            var cache = AppServicesHelper.CacheD;
            cache.Store_BSO("abc", "def", "ghi", storeKey, testData, DateTime.Now.AddDays(2), "repo");
            Thread.Sleep(1000);
            var cacheRead = cache.Get_BSO("abc", "def", "ghi", storeKey);
            Assert.True(cacheRead.hasData);
            Assert.True(cacheRead.data.Value == testData);
            cache.CasRepositoryFlush("repo");
            Thread.Sleep(1000);
            cacheRead = cache.Get_BSO("abc", "def", "ghi", storeKey);
            Assert.False(cacheRead.hasData);
        }

        [Fact]
        public void FirebaseTestBasic()
        {
            Helper.SetupTests();
            IDictionary<string, dynamic> fbUsers = AppServicesHelper.Firebase.GetAllUsers();
            Assert.True(fbUsers.Count > 0);
        }

        [Fact]
        public void FirebaseTestReadSubscribers()
        {
            Helper.SetupTests();
            Subscriber_BSO sBso = new Subscriber_BSO();
            var list = sBso.GetSubscribers(AppServicesHelper.StaticADO,null,false);
            Assert.True(list.Count >= 0);
        }


        [Fact]
        public void ADTest()
        {
            Helper.SetupTests();
            var list = AppServicesHelper.ActiveDirectory.List();
            Assert.True(list.Count > 0);
        }
    }
}
