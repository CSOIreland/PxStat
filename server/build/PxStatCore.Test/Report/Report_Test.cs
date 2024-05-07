using API;
using Moq;
using PxStat.Report;
using PxStat.Security;
using PxStatCore.Test;
using System;

namespace PxStatXUnit.Tests
{
    [Collection("PxStatXUnit")]
    public class Report_Test
    {
        private TableAudit_VLD_Read _tableAuditVldRead;



        [Fact]
        public void TestValidateDateHour()
        {
            Helper.SetupTests();
            _tableAuditVldRead = new TableAudit_VLD_Read();
            DateTime hourBefore = DateTime.Now.AddDays(-0.042);
            DateTime hourAfter = DateTime.Now.AddDays(0.042);
            DateTime now = DateTime.Now;
            bool validation = _tableAuditVldRead.IsBetweenTwoDates(now, hourBefore, hourAfter);
            Assert.True(validation);
        }

        [Fact]
        public void TestValidateDateDay()
        {
            Helper.SetupTests();
            _tableAuditVldRead = new TableAudit_VLD_Read();
            DateTime dayBefore = DateTime.Now.AddDays(-1);
            DateTime dayAfter = DateTime.Now.AddDays(1);
            DateTime now = DateTime.Now;
            bool validation = _tableAuditVldRead.IsBetweenTwoDates(now, dayBefore, dayAfter);
            Assert.True(validation);
        }

        [Fact]
        public void TestValidateDateWeek()
        {
            Helper.SetupTests();
            _tableAuditVldRead = new TableAudit_VLD_Read();
            DateTime weekBefore = DateTime.Now.AddDays(-7);
            DateTime weekAfter = DateTime.Now.AddDays(7);
            DateTime now = DateTime.Now;
            bool validation = _tableAuditVldRead.IsBetweenTwoDates(now, weekBefore, weekAfter);
            Assert.True(validation);
        }

        [Fact]
        public void TestValidateDateMonth()
        {
            Helper.SetupTests();
            _tableAuditVldRead = new TableAudit_VLD_Read();
            DateTime monthBefore = DateTime.Now.AddDays(-30);
            DateTime monthAfter = DateTime.Now.AddDays(30);
            DateTime now = DateTime.Now;
            bool validation = _tableAuditVldRead.IsBetweenTwoDates(now, monthBefore, monthAfter);
            Assert.True(validation);
        }

        [Fact]
        public void TestValidateDateYear()
        {
            Helper.SetupTests();
            _tableAuditVldRead = new TableAudit_VLD_Read();
            DateTime yearBefore = DateTime.Now.AddDays(-365);
            DateTime yearAfter = DateTime.Now.AddDays(365);
            DateTime now = DateTime.Now;
            bool validation = _tableAuditVldRead.IsBetweenTwoDates(now, yearBefore, yearAfter);
            Assert.True(validation);
        }
    }
}
