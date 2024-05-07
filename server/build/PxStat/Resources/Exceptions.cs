using System;


namespace PxStat.Resources
{
    internal class UnmatchedParametersException : Exception
    {
        internal UnmatchedParametersException(Exception innerException) : base(Label.Get("unmatched-parameters"), innerException)
        {

        }


    }
    internal class IncompleteCasFlush : Exception
    {

        internal IncompleteCasFlush(Exception innerException): base("One or more CAS items was not flushed",innerException) { }

        internal IncompleteCasFlush():base("One or more CAS items was not flushed") { }

    }

}
