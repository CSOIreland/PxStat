using Pidgin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PxStat.Resources.PxParser
{
    public class PxStatParseError : IEquatable<ParseError<string>>
    {

        public bool Equals(ParseError<string> other)
        {
            return true;
        }

    }
}