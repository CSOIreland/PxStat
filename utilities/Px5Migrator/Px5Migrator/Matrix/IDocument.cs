using System.Collections.Generic;


namespace Px5Migrator
{
    public interface IDocument
    {
        IList<IPxKeywordElement> Keywords { get; }
        IList<dynamic> GetData(string keywordIdentifier);
        IList<IPxSingleElement> GetListOfElements(string keywordIdentifier, string language = null);
        IList<IPxSingleElement> GetListOfElementsIfExist(string keywordIdentifier, string language = null);
        IList<string> GetListOfStringValues(string keywordIdentifier, string language = null);
        IList<string> GetListOfStringValuesIfExist(string keywordIdentifier, string language = null);
        IList<KeyValuePair<string, IPxSingleElement>> GetManySingleValuesWithSubkeysOnlyIfLanguageMatches(string identifier, string language = null);
        IList<KeyValuePair<string, IList<IPxSingleElement>>> GetMultiValuesWithSubkeys(string identifier, string language = null);
        IList<KeyValuePair<string, IList<IPxSingleElement>>> GetMultiValuesWithSubkeysIfExist(string identifier, string language = null);
        IList<KeyValuePair<string, IList<IPxSingleElement>>> GetMultiValuesWithSubkeysOnlyIfLanguageIsEmpty(string identifier);
        IList<KeyValuePair<string, IList<IPxSingleElement>>> GetMultiValuesWithSubkeysOnlyIfLanguageMatches(string identifier, string language = null);
        IList<KeyValuePair<string, IList<IPxSingleElement>>> GetMultiValuesWithSubkeysOnlyIfLanguageMatchesNullable(string identifier, string language = null);
        short GetShortElementValue(string keywordIdentifier, string language = null);
        bool GetShortElementValueIfExist(string keywordIdentifier, string language, ref short aShortNumber);
        IList<KeyValuePair<string, IList<IPxSingleElement>>> GetSingleElementWithSubkeys(string identifier, string language = null);
        IList<KeyValuePair<string, IList<IPxSingleElement>>> GetSingleElementWithSubkeysIfExist(string identifier, string language = null);
        string GetStringElementValue(string keywordIdentifier, string language = null);
        string GetStringValueIfExist(string keywordIdentifier, string language = null);
        string ToPxString();
        string ToString();
    }
}
