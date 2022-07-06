using API;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Px5Migrator
{
    public class Supervisor
    {
        public SupervisoryReport GetReport()
        {
            Migration_ADO mAdo = new Migration_ADO();
            SupervisoryReport report = new SupervisoryReport();

            ADO_readerOutput supyResult;
            using (IADO ado = IAdoFactory.GetAdo())
            {
                supyResult = mAdo.ReadMigrationStatus(ado);
            }
            if (supyResult.hasData)
            {
                report.ItemsRemainingCount = supyResult.data.Where(x => x.MtrMigrationFlag.Equals(DBNull.Value)).Count();
                report.ItemsMigratedCount = supyResult.data.Where(x => x.MtrMigrationFlag.Equals(true)).Count();
                report.ItemsPendingCount = supyResult.data.Where(x => x.MtrMigrationFlag.Equals(false)).Count();
            }

            return report;
        }


    }


    public class SupervisoryReport
    {
        public int ItemsRemainingCount { get; set; }
        public int ItemsMigratedCount { get; set; }
        public int ItemsPendingCount { get; set; }
        public List<Exception> ExceptionList { get; set; }
        public List<string> Messages { get; set; }
    }

    internal class MigrationReport
    {
        internal bool IsComplete { get; set; }
        internal bool HasErrors { get; set; }
        internal List<Exception> Errors { get; set; }
        internal string Message { get; set; }
        internal int RemainingDatasets { get; set; }
        internal int MatrixId { get; set; }
        internal string MtrCode { get; set; }
    }

    internal static class IAdoFactory
    {
        internal static IADO GetAdo()
        {

            return new ADO("defaultConnection");
        }
    }
}
