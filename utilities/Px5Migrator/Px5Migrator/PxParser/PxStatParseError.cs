using Pidgin;
using System;

namespace Px5Migrator
{
    public class PxStatParseError : IEquatable<ParseError<string>>
    {

        public bool Equals(ParseError<string> other)
        {
            return true;
        }

    }
}