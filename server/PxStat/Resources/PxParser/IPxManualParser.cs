//using API;
using PxParser.Resources.Parser;

namespace PxParser.Resources.Parser
{
    public interface IPxManualParser
    {
        PxDocument Parse();
        IPxKeywordElement ProcessKeywordElement(string keyword, string value);
    }
}
