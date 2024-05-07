
using Microsoft.AspNetCore.Http.Metadata;
using PxStat.Security;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PxStat.Resources
{
    internal class BBCodeTagReader
    {
        internal string Input { get; set; }
        internal string Output { get; set; } 
        internal List<positionReader> Positions { get; set; }   
        internal bool IsValid { get; set; }
        internal string Message { get; set; }
        internal string LngIsoCode { get; set; }

        internal BBCodeTagReader(string input, string lngIsoCode=null)
        {
            
            Input = input;
            Positions = new List<positionReader>();
            IsValid = true;
            LngIsoCode = lngIsoCode?? Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
        }

        internal void parseTag(string startTag, string endTag)
        {
            if (!IsValid) return;
            if(string.IsNullOrEmpty(Input)) { return; }

            int startPos = 0;
            int endPos = 0;
            
            while(startPos>=0)
            {
                positionReader pr = new();

                endPos = Input.IndexOf(endTag, startPos +1);
                startPos =Input.IndexOf(startTag, startPos);
                //Nothing found
                if (startPos < 0 && endPos<0) break;
                //end tag before start tag
                if (endPos < startPos ||(endPos>0 && startPos<0))
                {
                    IsValid = false;
                    Message = String.Format(Label.Get("px.build.end-tag-before-start", LngIsoCode), endTag, startTag);
                    break;
                }

                //Get the next starting position, just for testing
                int checkNextStartPos =startPos +1 <Input.Length ? Input.IndexOf(startTag, startPos+1):0;

                //No end tag found or the next start tag appears before the next scheduled end tag
                if (endPos < 0 || (checkNextStartPos>=0 && checkNextStartPos<endPos))
                {
                    if (endPos == -1)
                        Message = String.Format(Label.Get("px.build.end-tag-not-found", LngIsoCode), startTag);
                    else
                        Message = String.Format(Label.Get("px.build.misplaced-start-tag", LngIsoCode), startTag); 
                    IsValid = false;
                    break;
                }
                else
                {
                    //All good
                    pr.StartTag = startTag;
                    pr.EndTag = endTag;
                    pr.Start = startPos;
                    pr.End = endPos;
                    pr.isValid = true;
                    Positions.Add(pr);
                    startPos = endPos;
                }
                    
            }
        }

        internal void RemoveTags()
        {
            //When we remove tags, we must adjust the former positions of the tags to correspond to the new positions of the text that must be formatted

            if (!IsValid) return;
            if (string.IsNullOrEmpty(Input)) { return; }
            
            Output = Input;
            foreach (var item in Positions)
            {
                //Remove the tags
                Output = Output.Replace(item.StartTag, "");
                Output = Output.Replace(item.EndTag, "");

                //Any marked position greater than the position of item must be decremented by the length of the item tag
                //Calculate for item.Start as well as item.End
                string tag = item.StartTag;
                int pos = item.Start;
                foreach(var otherItem in Positions)
                {
                    if (otherItem.Start > pos) otherItem.Start = otherItem.Start - tag.Length;
                    if(otherItem.End > pos) otherItem.End = otherItem.End - tag.Length;
                }
                tag = item.EndTag;
                pos = item.End;
                foreach (var otherItem in Positions)
                {
                    if (otherItem.Start > pos) otherItem.Start = otherItem.Start - tag.Length;
                    if (otherItem.End > pos) otherItem.End = otherItem.End - tag.Length;
                }


            }

        }

        internal void ValidateResult()
        {//If a start tag is between the start and end tags of another pair, then the end tag must be between the start and end pairs as well
            if (!IsValid) return;
            if (string.IsNullOrEmpty(Input)) { return; }
            
            foreach (var pair in Positions)
            {
                foreach (var item in Positions)
                {
                    if (pair.Start == item.Start && pair.End == item.End)
                        continue;
                    if(pair.Start>item.Start && pair.Start <item.End)
                    {
                        if (pair.End >= item.End)
                        {
                            IsValid = false;
                            Message = Label.Get("px.build.overlapping-tags", LngIsoCode);
                            return;
                        }
                    }
                }
            }
        }
    }

    internal class positionReader
    {
        internal int Start { get; set; }    
        internal int End { get; set; }
        internal string StartTag { get; set; }
        internal string EndTag { get; set; }
        internal bool isValid { get; set; }
    }


}
