using System;

namespace PxStat
{
    /// <summary>
    /// If an object contains contains a parameter with the NoHtmlStrip attribute then no HTML tags will be stripped when passed to the Sanitizer
    /// </summary>
    internal class NoHtmlStrip : Attribute { }

    /// <summary>
    /// If an object contains contains a parameter with the NoTrim attribute then no trimming will apply when passed to the Sanitizer
    /// </summary>
    internal class NoTrim : Attribute { }

    /// <summary>
    /// If an object contains a parameter with the LowerCase attribute then the value will be converted to lower case when passed to the Sanitizer
    /// </summary>
    internal class LowerCase : Attribute { }

    /// <summary>
    /// If an object contains a parameter with the UpperCase attribute then the value will be converted to upper case when passed to the Sanitizer
    /// </summary>
    internal class UpperCase : Attribute { }

    /// <summary>
    /// If this attribute is asserted on an API method then calls to that method will not be traced
    /// </summary>
    internal class NoTrace : Attribute { }

    /// <summary>
    /// If this attribute is asserted on an API method then calls to that method will be logged in the Analytic table
    /// </summary>
    internal class Analytic : Attribute { }

    /// <summary>
    /// If this attribute is asserted then (a) the operation will search the cache before attempting a Read, and (b) any Read operation will be cached
    /// The CAS_REPOSITORY may need to be qualified by a DOMAIN. This is typically a DTO proprty. Include the proprty name if this needs to form part 
    /// of the name of the cas repository as well.
    /// </summary>
    internal class CacheRead : Attribute
    {
        public virtual string CAS_REPOSITORY { get; set; }
        public virtual string DOMAIN { get; set; }

        //The returned object property who's contents will define the expiry time of the cache
        public virtual string EXPIRY_DATE_TIME_PROPERTY { get; set; }
    }

    /// <summary>
    /// If this attribute is asserted then the relevant cache will be flushed. Separate the Cas values with a comma if multiple caches need to be flushed.
    /// If the cache name needs to be qualified with a property name as well, then they should also be part of a comma separated list. Make sure to keep 
    /// corresponding pairs in order. If no domain is required then use an empty string "" in order to keep the list coherent.
    /// </summary>
    internal class CacheFlush : Attribute
    {
        public virtual string CAS_REPOSITORY_DOMAIN_LIST { get; set; }
        public virtual string DOMAIN { get; set; }
    }
}