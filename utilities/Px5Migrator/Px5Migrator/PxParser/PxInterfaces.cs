using System.Collections.Generic;

namespace Px5Migrator
{
    /// <summary>
    /// Contract for any element to be parsed by the Px Parser
    /// </summary>
    public interface IPx
    {
        string ToPxString();
    }

    /// <summary>
    /// Contract for All the Keyword Elements in a Px File
    /// </summary>
    public interface IPxKeywordElement
    {

        string ToPxString();

        IPxElement Element { get; }

        /// <summary>
        /// The Key that identifies the object within the file
        /// </summary>
        PxKey Key { get; }
    }

    /// <summary>
    /// Contract for Generic Value Metadata Objects
    /// </summary>
    public interface IPxElement
    {
        string ToPxString();

        string ToPxValue();

        double ToDoubleValue();

        IList<string> ToList();

        dynamic Value { get; }

    }

    /// <summary>
    /// Contract for The Single Value Metadata Objects
    /// </summary>
    public interface IPxSingleElement
    {
        dynamic SingleValue { get; set; }

        string ToPxValue();

        double ToDoubleValue();

        string ToPxString();
    }

    /// <summary>
    /// Contract for The Multiple Value Metadata Objects
    /// </summary>
    public interface IPxMultipleElements
    {
        IList<dynamic> Values { get; }

        IList<string> ToList();
    }



}
