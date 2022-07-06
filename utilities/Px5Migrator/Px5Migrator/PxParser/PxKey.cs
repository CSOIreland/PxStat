using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Px5Migrator
{
    /// <summary>
    /// Represents the key in a keyword object in a Px File
    /// All that is at the left of equal = sign
    /// </summary>
    public class PxKey : IPx
    {


        /// <summary>
        /// True if the Identifier represents a Data Keyword Object Identifier, false otherwise.
        /// </summary>
        public bool IsData { get { return Identifier == "DATA"; } }

        /// <summary>
        /// 
        /// The keyword Identifier - mandatory
        /// 
        /// IDENTIFIER[LANGUAGE](SUB-KEY)="VALUE_1","VALUE_2", …; 
        /// 
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// The keyword LANGUAGE - optional
        /// 
        /// IDENTIFIER[LANGUAGE](SUB-KEY)=VALUE_1; 
        /// IDENTIFIER[LANGUAGE]="VALUE_1","VALUE_2", …; 
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// The keyword Subkey value - optional
        /// 
        /// IDENTIFIER[LANGUAGE](SUB-KEY)="VALUE_1","VALUE_2", …; 
        /// IDENTIFIER[LANGUAGE](SUB-KEY)=VALUE_1; 
        /// IDENTIFIER(SUB-KEY)=VALUE_1;
        /// </summary>
        public PxSubKey SubKey { get; }

        /// <summary>
        /// Constructs a key from an identifier
        /// </summary>
        /// <param name="theIdentifier">The Keyword Identifier</param>
        public PxKey(string theIdentifier)
        {
            Identifier = theIdentifier;
        }

        /// <summary>
        /// Constructs a key with an identifier and a language
        /// </summary>
        /// <param name="theIdentifier">The Keyword Identifier</param>
        /// <param name="aLanguage">The language</param>
        public PxKey(string theIdentifier, string aLanguage) : this(theIdentifier)
        {
            Language = aLanguage;
        }

        /// <summary>
        /// Constructs a key with an identifier and a subkey object
        /// </summary>
        /// <param name="theIdentifier">The Keyword Identifier</param>
        /// <param name="aSubKey">The Subkey object</param>
        public PxKey(string theIdentifier, PxSubKey aSubKey) : this(theIdentifier)
        {
            SubKey = aSubKey;
        }

        /// <summary>
        /// Constructs a key with an identifier, a language and a subkey object
        /// </summary>
        /// <param name="theIdentifier">The Keyword Identifier</param>
        /// <param name="aLanguage">The language</param>
        /// <param name="aSubKey">The subkey object</param>
        public PxKey(string theIdentifier, string aLanguage, PxSubKey aSubKey) : this(theIdentifier, aLanguage)
        {
            SubKey = aSubKey;
        }

        /// <summary>
        /// A representation of the key composed by the identifier, the language and the subkey
        /// </summary>
        /// <returns>A string representing the Key part of the keyword object, used to save the px file</returns>
        public string ToPxString()
        {
            var sb = new StringBuilder(Identifier.ToString());
            if (Language != null)
            {
                sb.AppendFormat("[{0}]", Language);
            }
            if (SubKey != null)
            {
                sb.AppendFormat("({0})", SubKey.ToPxString());
            }
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToPxValue()
        {
            return "";
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

    /// <summary>
    /// Represents the subkey par of the Key in a Keyword
    /// A subkey is composed by a sub key and optionally a value
    /// IDENTIFIER(SUB-KEY)
    /// </summary>
    public class PxSubKey : IPx
    {
        /// <summary>
        /// 
        /// The subkey Name identifier
        /// 
        /// IDENTIFIER[LANGUAGE](SUB-KEY)
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// The Subkey value
        /// IDENTIFIER[LANGUAGE](SUB-KEYNAME, SUB-KEYVALUE)
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Constructs the Subkey with the subKeyName provided.
        /// </summary>
        /// <param name="subKeyName">The subKeyName for the subkey</param>
        public PxSubKey(string subKeyName)
        {
            // Strip HTLML tags
            Name = Regex.Replace(subKeyName, @"<.*?>", "");
        }

        /// <summary>
        /// Constructs the Subkey with the subKeyName and subKeyValue provided.
        /// </summary>
        /// <param name="subKeyName">The subKeyName for the subkey</param>
        /// <param name="subKeyValue">The subKeyValue for the subkey</param>
        public PxSubKey(string subKeyName, string subKeyValue) : this(subKeyName)
        {
            // Strip HTLML tags
            Value = Regex.Replace(subKeyValue, @"<.*?>", "");
        }

        /// <summary>
        /// A representation of the subkey composed by the subkey name and value (when applicable).
        /// </summary>
        /// <returns>A string representing the SubKey part of the keyword object, used to save the px file</returns>
        public string ToPxString()
        {
            StringBuilder sb = new StringBuilder();
            if (Name.Substring(0, 1) != "\"")
                sb.AppendFormat("\"{0}\"", Name);
            else
                sb.Append(Name);

            if (!string.IsNullOrEmpty(Value))
            {
                sb.AppendFormat(",\"{0}\"", Value);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToPxStringUnquoted()
        {
            StringBuilder sb = new StringBuilder();
            //sb.Append('"' + Name + '"');
            sb.Append(Name);
            if (!string.IsNullOrEmpty(Value))
            {
                // sb.Append('"' + Value + '"');
                sb.Append(Value);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToPxValue()
        {
            if (!string.IsNullOrEmpty(Value))
            {
                return Value;
            }
            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double ToDoubleValue()
        {
            throw new NotImplementedException();
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
