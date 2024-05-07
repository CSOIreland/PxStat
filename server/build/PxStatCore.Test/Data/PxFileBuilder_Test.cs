using PxStat.Data;
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
    public class PxFileBuilder_Test
    {


        [Theory]
        [InlineData(10.51, 1, "10.5")]
        [InlineData(10.56, 1, "10.6")]
        [InlineData(10.5, 2, "10.50")]
        [InlineData(10.51, 2, "10.51")]
        [InlineData(10.512, 1, "10.5")]
        [InlineData(10.549, 1, "10.5")]
        [InlineData(10.512, 2, "10.51")]
        [InlineData(0.0000009, 6, "0.000001")]
        [InlineData(0.009, 2, "0.01")]
        [InlineData(0.0095, 3, "0.010")]

        public void TestFormatDecimalPlaces(double value, int numberOfDecimalPlaces, string expected)
        {
            PxFileBuilder pxFileBuilder = new PxFileBuilder();
            var result = pxFileBuilder.FormatDecimalPlaces(value, numberOfDecimalPlaces);
            Assert.Equal(expected, result);

        }
    }
}


