using API;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PxStat.Security
{
    /// <summary>
    /// Active Directory reading class
    /// </summary>
    internal class ActiveDirectory_ADO
    {

        /// <summary>
        /// Returns the entire Active Directory list if no CcnUsername parameter is supplied
        /// Otherwise return the AD entry for the specified user
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        internal static List<ActiveDirectory_DTO> Read(ADO ado, dynamic parameters)
        {
            List<ActiveDirectory_DTO> readList = new List<ActiveDirectory_DTO>();

            // Get Active Directory
            IDictionary<string, dynamic> adDirectory;


            if (!string.IsNullOrEmpty(parameters.CcnUsername)) // we are searching for one user
            {
                dynamic readAD;
                readAD = ActiveDirectory.Search(parameters.CcnUsername);
                if (readAD == null)
                {
                    return readList;
                }
                ActiveDirectory_DTO dto = new ActiveDirectory_DTO();
                dto.CcnUsername = readAD.SamAccountName;
                dto.CcnEmail = readAD.EmailAddress;
                dto.CcnDisplayName = readAD.GivenName + " " + readAD.Surname;
                readList.Add(dto);
                return readList;
            }

            // List all users
            adDirectory = ActiveDirectory.List();

            foreach (KeyValuePair<string, dynamic> pair in adDirectory)
            {
                ActiveDirectory_DTO dto = new ActiveDirectory_DTO(parameters);
                dto.CcnUsername = pair.Value.SamAccountName;
                dto.CcnEmail = pair.Value.EmailAddress;
                dto.CcnDisplayName = pair.Value.GivenName + " " + pair.Value.Surname;

                readList.Add(dto);
            }
            return readList;
        }

        /// <summary>
        /// Finds a single AD user in Active Directory
        /// </summary>
        /// <param name="Ado"></param>
        /// <param name="accountDto"></param>
        /// <returns></returns>
        //internal ActiveDirectory_DTO GetUser(ADO Ado, Account_DTO_Create accountDto)
        internal ActiveDirectory_DTO GetUser<T>(ADO Ado, T accountDto)
        {
            List<ActiveDirectory_DTO> result = Read(Ado, accountDto);
            ActiveDirectory_DTO adDTO = new ActiveDirectory_DTO();
            if (result.Count != default(int))
                adDTO = result.FirstOrDefault();
            return adDTO;
        }

        /// <summary>
        /// This method adds the groups to a list of users
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="users"></param>
        /// <returns></returns>
        internal void MergeGroupsToUsers(ADO ado, ref ADO_readerOutput resultUsers)
        {

            GroupAccount_ADO adoGroupAccount = new GroupAccount_ADO();
            //cycle through each user in the list and get the group data
            if (resultUsers.hasData)
            {
                foreach (var user in resultUsers.data)
                {
                    GroupAccount_DTO_Read gpAccDto = new GroupAccount_DTO_Read();
                    gpAccDto.CcnUsername = user.CcnUsername;

                    //Get the group data for the user
                    ADO_readerOutput resultGroups = adoGroupAccount.Read(ado, gpAccDto);
                    if (resultGroups.hasData)
                    {
                        user.UserGroups = new List<GroupAccount_DTO>();
                        foreach (var group in resultGroups.data)
                        {// add the new data to the output
                            GroupAccount_DTO groupAccountDTO = new GroupAccount_DTO();
                            groupAccountDTO.GrpName = group.GrpName;
                            groupAccountDTO.GrpCode = group.GrpCode;
                            groupAccountDTO.GccApproveFlag = group.GccApproveFlag;
                            // add group to the user
                            user.UserGroups.Add(groupAccountDTO);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// From an email address, get the rest of the information for a user from AD
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        internal dynamic GetAdSpecificDataForEmail(string email)
        {

            // Get Active Directory
            IDictionary<string, dynamic> adDirectory;

            // List all users
            adDirectory = ActiveDirectory.List();


            dynamic result = new ExpandoObject();
            var foundUser = adDirectory.Where(x => x.Value.EmailAddress == email);
            if (foundUser == null) return null;

            var key = foundUser.FirstOrDefault().Key;
            if (key == null) return null;

            result.CcnEmail = adDirectory[key].EmailAddress;
            result.CcnDisplayName = adDirectory[key].GivenName + " " + adDirectory[key].Surname;
            result.CcnUsername = key;


            return result;


        }

        /// <summary>
        /// Adds the retrieved Active Directory information to the list of users
        /// </summary>
        /// <param name="result"></param>
        internal void MergeAdToUsers(ref ADO_readerOutput result)
        {

            // Get Active Directory
            IDictionary<string, dynamic> adDirectory;

            // List all users
            adDirectory = ActiveDirectory.List();

            foreach (var user in result.data)
            {

                if (adDirectory.ContainsKey(user.CcnUsername))
                {
                    user.CcnEmail = adDirectory[user.CcnUsername].EmailAddress;
                    user.CcnDisplayName = adDirectory[user.CcnUsername].GivenName + " " + adDirectory[user.CcnUsername].Surname;
                }
                else
                {
                    if (!((IDictionary<string, Object>)user).ContainsKey("CcnDisplayName"))

                        user.CcnDisplayName = null;
                    if (user.CcnUsername != null && Regex.IsMatch(user.CcnUsername, Utility.GetCustomConfig("APP_REGEX_EMAIL")))
                        user.CcnEmail = user.CcnUsername;
                    else
                        user.CcnEmail = null;
                }

            }

        }

    }
}
