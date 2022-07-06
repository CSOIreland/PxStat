namespace Px5Migrator
{
    /// <summary>
    /// 
    /// The Keyword is composed of an identifier with optional language and subkey and a value or a list of values
    /// Only the identifier and the value are mandatory, below are different valid representations of keywords
    /// 
    /// IDENTIFIER[LANGUAGE](SUB-KEY)="VALUE_1","VALUE_2", …; 
    /// IDENTIFIER[LANGUAGE](SUB-KEY)=VALUE_1; 
    /// IDENTIFIER[LANGUAGE]="VALUE_1","VALUE_2", …; 
    /// IDENTIFIER(SUB-KEY)="VALUE_1","VALUE_2", …; 
    /// IDENTIFIER[LANGUAGE]=VALUE_1; 
    /// IDENTIFIER(SUB-KEY)=VALUE_1;
    /// IDENTIFIER=VALUE_1
    /// IDENTIFIER="VALUE_1"
    /// "VALUE_2";
    /// 
    /// We can represent the Keyword structure with the following BNF expression:
    /// 
    /// <PxKeywordElement> ::= <PxName> ["["<PxLanguage>"]"]["("PxSubKey")"] = <PxValue> [ , <PxValue> ]
    /// 
    /// </summary>
    public class PxKeywordElement : IPxKeywordElement
    {
        public IPxElement Element { get; }

        /// <summary>
        /// The Key that identifies the object within the file
        /// </summary>
        public PxKey Key { get; }

        /// <summary>
        /// True if this object is a Data Keyword, false if is a Medatadata Keyword
        /// </summary>
        public bool IsData { get { return Key.IsData; } }

        /// <summary>
        /// Constructs a PxKeywordElement
        /// </summary>
        /// <param name="theKey">The Key that identifies the object</param>
        public PxKeywordElement(PxKey theKey, IPxElement element)
        {
            Key = theKey;
            Element = element;
        }

        /// <summary>
        /// The string representation, used to get the textual representation to save the PxDocument
        /// </summary>
        /// <returns>A string with key and value representation of the object separated with =</returns>
        public string ToPxString()
            => $"{Key.ToPxString()}={Element.ToPxString()}";

        public string ToPxValue()
        {
            if (Element != null)
            {
                return Element.ToPxValue();
            }
            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double ToDoubleValue()
        {
            if (Element != null)
            {
                return Element.ToDoubleValue();
            }
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToPxString();
        }
    }



}
