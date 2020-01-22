using PxStat.Security;
using System.Collections.Generic;

namespace PxStat.System.Notification
{
    /// <summary>
    /// DTO for sending a message to one or more groups
    /// </summary>
    internal class Email_DTO_GroupMessage
    {
        public string Subject { get; internal set; }
        public string Body { get; internal set; }
        public List<Group_DTO_Read> GroupCodes { get; internal set; }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Email_DTO_GroupMessage(dynamic parameters)
        {
            if (parameters.Subject != null)
                Subject = parameters.Subject;
            if (parameters.Body != null)
                Body = parameters.Body;
            GroupCodes = new List<Group_DTO_Read>();
            if (parameters.GroupCodes != null)
            {
                foreach (var g in parameters.GroupCodes)
                {
                    Group_DTO_Read gDto = new Group_DTO_Read();
                    gDto.GrpCode = g.GrpCode.ToString();
                    GroupCodes.Add(gDto);
                }
            }
        }
    }
}
