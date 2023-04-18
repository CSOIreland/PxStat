namespace PxStat.Data
{
    public interface IDimensionVariable
    {
        string Code { get; set; }
        int Sequence { get; set; }
        string Value { get; set; }
        bool Elimination { get; set; }
        short Decimals { get; set; }
        string Unit { get; set; }
        bool AmendFlag { get; set; }
        bool IsEquivalent(IDimensionVariable otherVariable);
    }
}
