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
using PxStat.Resources;
using System.Reflection;
namespace PxStatCore.Test.ApiLibrary
{
    [Collection("PxStatXUnit")]
    public class MethodReaderTests
    {
        [Fact]
        public void TestMethodHasAttribute()
        {
            Helper.SetupTests();
            bool read=API.MethodReader.MethodHasAttribute("PxStat.Data.Cube_API.ReadDataset", "AllowHEADrequest");
            
            Assert.True(read);
        }

        [Fact]
        public void TestMethodHasNotAttribute()
        {
            Helper.SetupTests();
            bool read = API.MethodReader.MethodHasAttribute("PxStat.Data.Cube_API.ReadDataset", "NonesuchAttribute");
            Assert.False(read);
        }

        [Fact]
        public void TestMethodAttributeValue()
        {
            Helper.SetupTests();
            CustomAttributeData read = API.MethodReader.MethodAttributeValue("PxStat.Data.Cube_API.ReadDataset", "AllowHEADrequest");
            Assert.True(read!=null);
            Assert.True(read.AttributeType.FullName.Equals("PxStat.AllowHEADrequest"));
        }

        [Fact]
        public void TestMethodAttributeValueNegative()
        {
            Helper.SetupTests(); 
            CustomAttributeData read = API.MethodReader.MethodAttributeValue("PxStat.Data.Cube_API.ReadDataset", "NonesuchAttribute");
            Assert.True(read == null);
        }

        [Fact]
        public void TestAllowedAPIDictionary()
        {
            Helper.SetupTests();
           
            Assert.True(true);
        }
    }
}
