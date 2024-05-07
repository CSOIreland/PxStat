using PxStatCore.Test;
using PxStat.System.Navigation;
using Moq;
using Moq.Protected;
using PxStat.Config;
using PxStat.Workflow;
using API;
using ClosedXML.Excel;
using System.Dynamic;
using DocumentFormat.OpenXml.Math;
using PxStat.Security;
using FluentValidation;

namespace PxStatXUnit.Tests
{
    [Collection("PxStatXUnit")]
    public class Workflow_Test
    {
        [Fact]
        public void GetRequestUser_Test()
        {
            WorkflowRequest_ADO workflow = new WorkflowRequest_ADO();
            Mock<IADO> ado = new Mock<IADO>();
            ado.Setup(a => a.ExecuteReaderProcedure("Security_Account_Read", It.IsAny<List<ADO_inputParams>>())).Returns(GetReaderOutput());
            ActiveDirectory_DTO adDto = new ActiveDirectory_DTO() { CcnUsername = "expiredUser" };
            dynamic dynamicObj = new ExpandoObject();
            dynamicObj.CcnUsername = "chapmand";
            var result = workflow.GetRequestUser(ado.Object, dynamicObj, adDto);
            Assert.Null(result);
        }

        private ADO_readerOutput GetReaderOutput()
        {
            ADO_readerOutput output = new ADO_readerOutput();
            dynamic o = new ExpandoObject();
            output.data.Add(o);
            output.hasData = true;
            return output;
        }
    }
}
