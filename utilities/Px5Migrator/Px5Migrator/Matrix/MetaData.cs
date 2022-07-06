namespace Px5Migrator
{
    public class MetaData : IMetaData
    {
        public string GetFormatType()
        {
            return "PX";
        }

        public string GetIsOfficialStatistic()
        {
            return "YES";
        }

        public string GetFrequencyCodes()
        {
            return "TLIST(A1)/annual,TLIST(H1)/half-yearly,TLIST(Q1)/quarterly,TLIST(M1)/monthly,TLIST(W1)/weekly,TLIST(D1)/daily";
        }
    }
}
