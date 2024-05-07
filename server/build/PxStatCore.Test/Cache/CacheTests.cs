using API;
using Moq;
using Xunit;
using PxStat.Config;
using PxStat.Report;
using PxStatCore.Test;
using System.Collections.Generic;
using System.Dynamic;
using PxStat;
using DeviceDetectorNET.Cache;

namespace PxStatXUnit.Tests
{
    [Collection("PxStatXUnit")]
    public class CacheTests
    {
        [Fact]
        public void TestBsoStore()
        {
            TestDTO dto = new TestDTO() { Id = 1, Name = "Test" };
            TestData data = new TestData() { Id = 11, Name = "TestData" };

            Helper.SetupTests();
            AppServicesHelper.CacheD.Store_BSO("TestNameSpace", "TestClass", "TestMethod", dto, data, DateTime.Now.AddMinutes(3));

            MemCachedD_Value cache = AppServicesHelper.CacheD.Get_BSO<dynamic>("TestNameSpace", "TestClass", "TestMethod", dto);
            Assert.NotNull(cache);
            Assert.True(cache.hasData);
            Assert.True(cache.data["Id"].ToString() == "11");
        }

        [Fact]
        public void TestStoreBsoNull()
        {
            TestDTO dto = new TestDTO() { Id = 1, Name = "Test" };

            Helper.SetupTests();
            AppServicesHelper.CacheD.Store_BSO("TestNameSpace", "TestClass", "TestMethod", dto, null, DateTime.Now.AddMinutes(3));

            MemCachedD_Value cache = AppServicesHelper.CacheD.Get_BSO<dynamic>("TestNameSpace", "TestClass", "TestMethod", dto);
            Assert.NotNull(cache);
            Assert.True(cache.hasData);
        }

        [Fact]
        public void TestStoreCAS()
        {
            TestDTO dto = new TestDTO() { Id = 2, Name = "Test2" };
            TestData data = new TestData() { Id = 22, Name = "TestData2" };

            Helper.SetupTests();
            AppServicesHelper.CacheD.Store_BSO("TestNameSpace", "TestClass", "TestMethod", dto, data, DateTime.Now.AddMinutes(3), "TestCAS");

            MemCachedD_Value cache = AppServicesHelper.CacheD.Get_BSO<dynamic>("TestNameSpace", "TestClass", "TestMethod", dto);
            Assert.NotNull(cache);
            Assert.True(cache.hasData);
            Assert.True(cache.data["Id"].ToString() == "22");
        }



        [Fact]
        public void TestFlushCAS()
        {
            Helper.SetupTests();

            TestDTO dto = new TestDTO() { Id = 2, Name = "Test2" };
            TestData data = new TestData() { Id = 22, Name = "TestData2" };


            AppServicesHelper.CacheD.Store_BSO("TestNameSpace", "TestClass", "TestMethod", dto, data, DateTime.Now.AddMinutes(3), "TestCAS");

            MemCachedD_Value cache = AppServicesHelper.CacheD.Get_BSO<dynamic>("TestNameSpace", "TestClass", "TestMethod", dto);
            Assert.NotNull(cache);
            Assert.True(cache.hasData);
            Assert.True(cache.data["Id"].ToString() == "22");

            TestDTO dto2 = new TestDTO() { Id = 1, Name = "Test" };
            TestData data2 = new TestData() { Id = 11, Name = "TestData" };

            AppServicesHelper.CacheD.Store_BSO("TestNameSpace", "TestClass", "TestMethod", dto2, data2, DateTime.Now.AddMinutes(3), "TestCAS");

            cache = AppServicesHelper.CacheD.Get_BSO<dynamic>("TestNameSpace", "TestClass", "TestMethod", dto2);
            Assert.NotNull(cache);
            Assert.True(cache.hasData);
            Assert.True(cache.data["Id"].ToString() == "11");

            AppServicesHelper.CacheD.CasRepositoryFlush("TestCAS");

            cache = AppServicesHelper.CacheD.Get_BSO<dynamic>("TestNameSpace", "TestClass", "TestMethod", dto);
            Assert.NotNull(cache);
            Assert.False(cache.hasData);

            cache = AppServicesHelper.CacheD.Get_BSO<dynamic>("TestNameSpace", "TestClass", "TestMethod", dto2);
            Assert.NotNull(cache);
            Assert.False(cache.hasData);

        }

        [Fact]
        public void TestRemoveBSO()
        {
            Helper.SetupTests();
            TestDTO dto = new TestDTO() { Id = 1, Name = "Test" };
            TestData data = new TestData() { Id = 11, Name = "TestData" };

            Helper.SetupTests();
            AppServicesHelper.CacheD.Store_BSO("TestNameSpace", "TestClass", "TestMethod", dto, data, DateTime.Now.AddMinutes(3));

            MemCachedD_Value cache = AppServicesHelper.CacheD.Get_BSO<dynamic>("TestNameSpace", "TestClass", "TestMethod", dto);
            Assert.NotNull(cache);
            Assert.True(cache.hasData);
            Assert.True(cache.data["Id"].ToString() == "11");

            Assert.True(AppServicesHelper.CacheD.Remove_BSO("TestNameSpace", "TestClass", "TestMethod", dto));

            cache = AppServicesHelper.CacheD.Get_BSO<dynamic>("TestNameSpace", "TestClass", "TestMethod", dto);
            Assert.NotNull(cache);
            Assert.False(cache.hasData);
        }

        [Fact]
        public void TestStoreADO()
        {
            Helper.SetupTests();
            TestData data = new TestData() { Id = 11, Name = "TestData" };
            List<ADO_inputParams> paramData = new List<ADO_inputParams>()
            {
                new ADO_inputParams(){  name="name1",value="value1"}
            };
            AppServicesHelper.CacheD.Store_ADO<dynamic>("nameSpace1", "procName1", paramData, data, DateTime.Now.AddMinutes(3));

            MemCachedD_Value cache = AppServicesHelper.CacheD.Get_ADO("nameSpace1", "procName1", paramData);
            Assert.NotNull(cache);
            Assert.True(cache.hasData);
            Assert.True(cache.data["Id"].ToString() == "11");
        }

        [Fact]
        public void TestRemoveADO()
        {
            Helper.SetupTests();
            TestData data = new TestData() { Id = 11, Name = "TestData" };
            List<ADO_inputParams> paramData = new List<ADO_inputParams>()
            {
                new ADO_inputParams(){  name="name1",value="value1"}
            };
            AppServicesHelper.CacheD.Store_ADO<dynamic>("nameSpace1", "procName1", paramData, data, DateTime.Now.AddMinutes(3));

            MemCachedD_Value cache = AppServicesHelper.CacheD.Get_ADO("nameSpace1", "procName1", paramData);
            Assert.NotNull(cache);
            Assert.True(cache.hasData);
            Assert.True(cache.data["Id"].ToString() == "11");

            Assert.True(AppServicesHelper.CacheD.Remove_ADO("nameSpace1", "procName1", paramData));

            cache = AppServicesHelper.CacheD.Get_ADO("nameSpace1", "procName1", paramData);
            Assert.NotNull(cache);
            Assert.False(cache.hasData);

        }

        [Fact]
        public void TestStoreNullADO()
        {
            Helper.SetupTests();
            List<ADO_inputParams> paramData = new List<ADO_inputParams>()
            {
                new ADO_inputParams(){  name="name1",value="value1"}
            };
            AppServicesHelper.CacheD.Store_ADO<dynamic>("nameSpace1", "procName1", paramData, null, DateTime.Now.AddMinutes(3));

            MemCachedD_Value cache = AppServicesHelper.CacheD.Get_ADO("nameSpace1", "procName1", paramData);
            Assert.NotNull(cache);
            Assert.True(cache.hasData);
        }
    }

    public class TestDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }

    public class TestData
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
