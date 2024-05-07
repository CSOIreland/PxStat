using PxStatCore.Test;
using PxStat.Data;
using PxStat.DBuild;

namespace PxStatXUnit.Tests
{
    [Collection("PxStatXUnit")]
    public class CubeBsoCreate_Test
    {
        [Fact]
        public void TestMatrixValidatePositive()
        {
            string signature;
            Helper.SetupTests();
            Dictionary<string, object> param = new()
            {
                {"MtrInput","Q0hBUlNFVD0iQU5TSSI7CkFYSVMtVkVSU0lPTj0iMjAwNiI7CkxBTkdVQUdFPSJlbiI7CkNSRUFUSU9OLURBVEU9IjIwMjAwOTA5IDA4OjI5IjsKREVDSU1BTFM9MDsKU0hPV0RFQ0lNQUxTPTA7Ck1BVFJJWD0iTUlNMDQiOwpTVUJKRUNULUFSRUE9IkNlbnN1cyBvZiBQb3B1bGF0aW9uIjsKU1VCSkVDVC1DT0RFPSJBMDEiOwpUSVRMRT0iSW5kdXN0cmlhbCBQcm9kdWN0aW9uIFZvbHVtZSBhbmQgVHVybm92ZXIgSW5kaWNlcyAoQmFzZXggMjAxNT0xMDApIGJ5IgoiSW5kdXN0cnkgU2VjdG9yIE5BQ0UgUmV2IDIsIHN0YXRpc3RpY2FsIGluZGljYXRvciBhbmQgTW9udGgiOwpDT05URU5UUz0iSW5kdXN0cmlhbCBQcm9kdWN0aW9uIFZvbHVtZSBhbmQgVHVybm92ZXIgSW5kaWNlcyAoQmFzZSA8PjIwMTU9MTAwKSI7ClVOSVRTPSJCYXNlIDIwMTU9MTAwIjsKU1RVQj0iSW5kdXN0cnkgU2VjdG9yIE5BQ0UgUmV2IDIiLCJTVEFUSVNUSUMiOwpIRUFESU5HPSJNb250aCI7CkNPTlRWQVJJQUJMRT0iU1RBVElTVElDIjsKVkFMVUVTKCJJbmR1c3RyeSBTZWN0b3IgTkFDRSBSZXYgMiIpPSJGb29kIHByb2R1Y3RzICgxMCkiLCJNZWF0IGFuZCBtZWF0IHByb2R1Y3RzICgxMDEpIiwiRGFpcnkgcHJvZHVjdHMgKDEwNSkiOwpWQUxVRVMoIlNUQVRJU1RJQyIpPSJJbmR1c3RyaWFsIFByb2R1Y3Rpb24gSW5kZXggKEJhc2UgMjAxNT0xMDApIiwiSW5kdXN0cmlhbCBUdXJub3ZlciBJbmRleCAoQmFzZSAyMDE1PTEwMCkiOwpWQUxVRVMoIk1vbnRoIik9IjIwMjBNMDUiLCIyMDIwTTA2IiwiMjAyME05OSI7CkNPREVTKCJJbmR1c3RyeSBTZWN0b3IgTkFDRSBSZXYgMiIpPSIxMCIsIjEwMSIsIjEwNSI7CkNPREVTKCJTVEFUSVNUSUMiKT0iTUlNMDRDMDEiLCJNSU0wNEMwMiI7CkNPREVTKCJNb250aCIpPSIyMDIwTTA1IiwiMjAyME0wNiIsIjIwMjBNOTkiOwpET01BSU4oIkluZHVzdHJ5IFNlY3RvciBOQUNFIFJldiAyIik9Ik5BQ0UyIjsKUFJFQ0lTSU9OKCJTVEFUSVNUSUMiLCJJbmR1c3RyaWFsIFByb2R1Y3Rpb24gSW5kZXggKEJhc2UgMjAxNT0xMDApIik9MTsKUFJFQ0lTSU9OKCJTVEFUSVNUSUMiLCJJbmR1c3RyaWFsIFR1cm5vdmVyIEluZGV4IChCYXNlIDIwMTU9MTAwKSIpPTE7CkxBU1QtVVBEQVRFRCgiSW5kdXN0cmlhbCBQcm9kdWN0aW9uIEluZGV4IChCYXNlIDIwMTU9MTAwKSIpPSIyMDIwMDkwNCAxNjozNyI7CkxBU1QtVVBEQVRFRCgiSW5kdXN0cmlhbCBUdXJub3ZlciBJbmRleCAoQmFzZSAyMDE1PTEwMCkiKT0iMjAyMDA5MDQgMTY6MzciOwpVTklUUygiSW5kdXN0cmlhbCBQcm9kdWN0aW9uIEluZGV4IChCYXNlIDIwMTU9MTAwKSIpPSJCYXNlIDIwMTU9MTAwIjsKVU5JVFMoIkluZHVzdHJpYWwgVHVybm92ZXIgSW5kZXggKEJhc2UgMjAxNT0xMDApIik9IkJhc2UgMjAxNT0xMDAiOwpTT1VSQ0U9IkNlbnRyYWwgU3RhdGlzdGljcyBPZmZpY2UsIElyZWxhbmQiOwpEQVRBQkFTRT0iU3RhdEJhbmsgSXJlbGFuZCI7CkRBVEE9Cjc3LjggOTMuNiAxMTQuNCA5MS43IDEwNC4wIDExNy4zIAoxMDguOCAxMTMuNSAxMTIuMSAxMDYuNyAxMDcuNyAxMDAuOCAKMTQ3LjYgMTY0LjMgMTUxLjEgMTQzLjEgMTU5LjggMTU3Ljc7Cg==" },
                {"FrqCodeTimeval","TLIST(M1)" },
                {"FrqValueTimeval","Month" },
                {"LngIsoCode","en" }
            };
            var request = Helper.GetRequest("PxStat.Data.Matrix_API.Validate", param);
            var result = new DBuild_BSO_Validate(request).Read().Response;
            Assert.NotNull(result.data.Signature); 

            signature= result.data.Signature;

            param = new()
            {
                {"MtrInput","Q0hBUlNFVD0iQU5TSSI7CkFYSVMtVkVSU0lPTj0iMjAwNiI7CkxBTkdVQUdFPSJlbiI7CkNSRUFUSU9OLURBVEU9IjIwMjAwOTA5IDA4OjI5IjsKREVDSU1BTFM9MDsKU0hPV0RFQ0lNQUxTPTA7Ck1BVFJJWD0iTUlNMDQiOwpTVUJKRUNULUFSRUE9IkNlbnN1cyBvZiBQb3B1bGF0aW9uIjsKU1VCSkVDVC1DT0RFPSJBMDEiOwpUSVRMRT0iSW5kdXN0cmlhbCBQcm9kdWN0aW9uIFZvbHVtZSBhbmQgVHVybm92ZXIgSW5kaWNlcyAoQmFzZXggMjAxNT0xMDApIGJ5IgoiSW5kdXN0cnkgU2VjdG9yIE5BQ0UgUmV2IDIsIHN0YXRpc3RpY2FsIGluZGljYXRvciBhbmQgTW9udGgiOwpDT05URU5UUz0iSW5kdXN0cmlhbCBQcm9kdWN0aW9uIFZvbHVtZSBhbmQgVHVybm92ZXIgSW5kaWNlcyAoQmFzZSA8PjIwMTU9MTAwKSI7ClVOSVRTPSJCYXNlIDIwMTU9MTAwIjsKU1RVQj0iSW5kdXN0cnkgU2VjdG9yIE5BQ0UgUmV2IDIiLCJTVEFUSVNUSUMiOwpIRUFESU5HPSJNb250aCI7CkNPTlRWQVJJQUJMRT0iU1RBVElTVElDIjsKVkFMVUVTKCJJbmR1c3RyeSBTZWN0b3IgTkFDRSBSZXYgMiIpPSJGb29kIHByb2R1Y3RzICgxMCkiLCJNZWF0IGFuZCBtZWF0IHByb2R1Y3RzICgxMDEpIiwiRGFpcnkgcHJvZHVjdHMgKDEwNSkiOwpWQUxVRVMoIlNUQVRJU1RJQyIpPSJJbmR1c3RyaWFsIFByb2R1Y3Rpb24gSW5kZXggKEJhc2UgMjAxNT0xMDApIiwiSW5kdXN0cmlhbCBUdXJub3ZlciBJbmRleCAoQmFzZSAyMDE1PTEwMCkiOwpWQUxVRVMoIk1vbnRoIik9IjIwMjBNMDUiLCIyMDIwTTA2IiwiMjAyME05OSI7CkNPREVTKCJJbmR1c3RyeSBTZWN0b3IgTkFDRSBSZXYgMiIpPSIxMCIsIjEwMSIsIjEwNSI7CkNPREVTKCJTVEFUSVNUSUMiKT0iTUlNMDRDMDEiLCJNSU0wNEMwMiI7CkNPREVTKCJNb250aCIpPSIyMDIwTTA1IiwiMjAyME0wNiIsIjIwMjBNOTkiOwpET01BSU4oIkluZHVzdHJ5IFNlY3RvciBOQUNFIFJldiAyIik9Ik5BQ0UyIjsKUFJFQ0lTSU9OKCJTVEFUSVNUSUMiLCJJbmR1c3RyaWFsIFByb2R1Y3Rpb24gSW5kZXggKEJhc2UgMjAxNT0xMDApIik9MTsKUFJFQ0lTSU9OKCJTVEFUSVNUSUMiLCJJbmR1c3RyaWFsIFR1cm5vdmVyIEluZGV4IChCYXNlIDIwMTU9MTAwKSIpPTE7CkxBU1QtVVBEQVRFRCgiSW5kdXN0cmlhbCBQcm9kdWN0aW9uIEluZGV4IChCYXNlIDIwMTU9MTAwKSIpPSIyMDIwMDkwNCAxNjozNyI7CkxBU1QtVVBEQVRFRCgiSW5kdXN0cmlhbCBUdXJub3ZlciBJbmRleCAoQmFzZSAyMDE1PTEwMCkiKT0iMjAyMDA5MDQgMTY6MzciOwpVTklUUygiSW5kdXN0cmlhbCBQcm9kdWN0aW9uIEluZGV4IChCYXNlIDIwMTU9MTAwKSIpPSJCYXNlIDIwMTU9MTAwIjsKVU5JVFMoIkluZHVzdHJpYWwgVHVybm92ZXIgSW5kZXggKEJhc2UgMjAxNT0xMDApIik9IkJhc2UgMjAxNT0xMDAiOwpTT1VSQ0U9IkNlbnRyYWwgU3RhdGlzdGljcyBPZmZpY2UsIElyZWxhbmQiOwpEQVRBQkFTRT0iU3RhdEJhbmsgSXJlbGFuZCI7CkRBVEE9Cjc3LjggOTMuNiAxMTQuNCA5MS43IDEwNC4wIDExNy4zIAoxMDguOCAxMTMuNSAxMTIuMSAxMDYuNyAxMDcuNyAxMDAuOCAKMTQ3LjYgMTY0LjMgMTUxLjEgMTQzLjEgMTU5LjggMTU3Ljc7Cg==" },
                {"Overwrite",true },
                {"GrpCode","LMTEST" },
                {"Signature",signature },
                {"CprCode","CSO" },
                {"FrqCodeTimeval","TLIST(M1)" },
                {"FrqValueTimeval","Month" },
                {"LngIsoCode","en" }
            };

            request = Helper.GetRequest("PxStat.Data.Matrix_API.Create", param);
            result = new Matrix_BSO_Create(request).Create().Response;
            Assert.NotNull(result.data);


        }

        [Fact]
        public void TestGetReleaseOfUploadedMatrix()
        {
            Helper.SetupTests();
 
            Dictionary<string, object> format = new()
            {
                { "type" ,"JSON-stat"} ,
                {"version", "2.0" }
            };
            Dictionary<string, object> param = new()
            {
                {"matrix","MIM04" },
                {"LngIsoCode","en" },
                {"format",format }
            };
            var request = Helper.GetRequest("PxStat.Data.Cube_API.ReadMetaData", param);
            var result = new Cube_BSO_ReadMetadata(request).Read().Response;

            Assert.True(result.data != null);
        }

        [Fact]
        public void TestAreVariablesSorted()
        {
            StatDimension sd = new() { Sequence = 1, Code = "1001", Value = "TestAreVariablesSortedTrue",Variables=new List<IDimensionVariable>() };
            sd.Variables.Add(new DimensionVariable() { Sequence=1, Code="A001A", Value="First" });
            sd.Variables.Add(new DimensionVariable() { Sequence = 2, Code = "A001C", Value = "Second" });
            sd.Variables.Add(new DimensionVariable() { Sequence = 1, Code = "A001E", Value = "Third" });

            DBuild_BSO bid = new();
            Assert.True(bid.AreVariablesSequential(sd));

            sd = new() { Sequence = 1, Code = "1002", Value = "TestAreVariablesSortedFalse", Variables = new List<IDimensionVariable>() };
            sd.Variables.Add(new DimensionVariable() { Sequence = 1, Code = "A001A", Value = "First" });
            sd.Variables.Add(new DimensionVariable() { Sequence = 2, Code = "A001E", Value = "Second" });
            sd.Variables.Add(new DimensionVariable() { Sequence = 1, Code = "A001B", Value = "Third" });

            Assert.False(bid.AreVariablesSequential(sd));

        }
    }
}
