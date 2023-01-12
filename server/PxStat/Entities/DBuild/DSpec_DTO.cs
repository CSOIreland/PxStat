using System.Collections.Generic;

namespace PxStat.DBuild
{


    public class DSpec_DTO
    {
        public ICollection<StatDimension_DTO> StatDimensions { get; set; } = new List<StatDimension_DTO>();
        //The Contents of the dataset
        public string Contents { get; set; }
        //The name of the statistic dimension if required
        public string ContentVariable { get; set; }
        //The url for copyright
        public string CopyrightUrl { get; set; }
        //Number of decimal places for values
        public short Decimals { get; set; }
        //The iso language code for this spec
        public string Language { get; set; }
        //The relevant Matrix Code
        public string MatrixCode { get; set; }

        //The publishing source
        public string Source { get; set; }
        //The title of the dataset
        public string MtrTitle { get; set; }
        //To flag whether or not time values have been defined
        public bool TimeValsDefined { get; set; }
        //Time Values
        public ICollection<KeyValuePair<string, ICollection<string>>> TimeVals { get; set; }
        //General values
        public ICollection<KeyValuePair<string, ICollection<string>>> Values { get; set; }
        //Main values
        public ICollection<KeyValuePair<string, ICollection<string>>> MainValues { get; internal set; }
        //Notes as string
        public string MtrNote { get; set; }
        public bool IsEquivalent(DSpec_DTO otherSpec)
        {
            if (this.StatDimensions.Count != otherSpec.StatDimensions.Count) return false;
            List<StatDimension_DTO> thisList = (List<StatDimension_DTO>)this.StatDimensions;
            List<StatDimension_DTO> otherList = (List<StatDimension_DTO>)otherSpec.StatDimensions;
            for (int i = 0; i < this.StatDimensions.Count; i++)
            {
                if (!thisList[i].IsEquivalent(otherList[i])) return false;
            }

            if (!this.ContentVariable.Equals(otherSpec.ContentVariable)) return false;
            if (this.CopyrightUrl == null && otherSpec.CopyrightUrl != null) return false;
            if (this.CopyrightUrl != null && otherSpec.CopyrightUrl == null) return false;
            if (this.CopyrightUrl != null && otherSpec.CopyrightUrl != null)
                if (!this.CopyrightUrl.Equals(otherSpec.CopyrightUrl)) return false;

            if (!this.Decimals.Equals(otherSpec.Decimals)) return false;

            if (this.MatrixCode == null && otherSpec.MatrixCode != null) return false;
            if (this.MatrixCode != null && otherSpec.MatrixCode == null) return false;
            if (this.MatrixCode != null && otherSpec.MatrixCode != null)
                if (!this.MatrixCode.Equals(otherSpec.MatrixCode)) return false;

            if (this.Source == null && otherSpec.Source != null) return false;
            if (this.Source != null && otherSpec.Source == null) return false;
            if (this.Source != null && otherSpec.Source != null)
                if (!this.Source.Equals(otherSpec.Source)) return false;

            return true;
        }

    }
}
