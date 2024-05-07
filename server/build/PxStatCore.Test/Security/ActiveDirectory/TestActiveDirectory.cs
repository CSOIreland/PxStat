using API;
using Dynamitey;
using PxStat;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PxStatCore.Test.Security.ActiveDirectory
{
    public class TestActiveDirectory
    {
        [Fact]
        public void TestListDirectory()
        {
            Helper.SetupTests();
            var result = AppServicesHelper.ActiveDirectory.List();
            Assert.True(result.Count > 300);
            Assert.True(result.Count < 3000);
        }

        [Fact]
        public void TestFindUser()
        {
            Helper.SetupTests();
            string? SamAccountName = ApiServicesHelper.Configuration.GetSection("Test:SamAccountName").Value;
            string? EmailAddress = ApiServicesHelper.Configuration.GetSection("Test:EmailAddress").Value;

            var result = AppServicesHelper.ActiveDirectory.Search(SamAccountName);
            Assert.False(result.EmailAddress.ToString().ToLower().Trim().Equals(EmailAddress));
            
        }
        //[Fact]
        //public void TestGetAdHierarchy()
        //{
        //    Helper.SetupTests();
        //    dynamic ADDetails = AppServicesHelper.ActiveDirectory.Search<UserPrincipalExtended>("okeeffene");
        //    dynamic returnAdDetails = new ExpandoObject();
        //    Assert.True(ADDetails.Count > 0);
        //}

        [Fact]
        public void TestSearch() 
        {
            Helper.SetupTests();
            dynamic ADDetails = AppServicesHelper.ActiveDirectory.Search("okeeffene");
            var adType = ADDetails.GetType();
            Assert.True(ADDetails!=null);
        }

        //[Fact]
        //public void TestSearchTyped()
        //{
        //    Helper.SetupTests();
            
        //    dynamic ADDetails = AppServicesHelper.ActiveDirectory.Search<UserPrincipalExtended>("okeeffene");
        //    var adType = ADDetails.GetType();
        //    Assert.True(ADDetails != null);
        //}

        [Fact]
        public void TestHierarchicalSearch()
        {
            
            Helper.SetupTests();
            
            string samAccountName = "okeeffene";

            dynamic ADDetails = AppServicesHelper.ActiveDirectory.Search<UserPrincipalExtended>(samAccountName);
            
            dynamic returnAdDetails = new ExpandoObject();

            //user not found
            var nullOrEmpty = (ADDetails == null || ADDetails.HeadOfDivision == null || string.IsNullOrEmpty(ADDetails.HeadOfDivision.ToString()));

            if (!nullOrEmpty)
                nullOrEmpty = (ADDetails.Manager == null || String.IsNullOrEmpty(ADDetails.Manager.ToString()));

            if (nullOrEmpty)
            {
                Log.Instance.Error("Acitve directory user details not found for : " + samAccountName);
                returnAdDetails.HeadOfDivision = "";
                returnAdDetails.Manager = "";

                returnAdDetails.Hierarchy = "";
                returnAdDetails.DirectManager = "";
                returnAdDetails.DirectManagerTitle = "";
                returnAdDetails.HeadOfDivision = "";
                returnAdDetails.Division = "";
                returnAdDetails.Directorate = "";
                returnAdDetails.Department = "";
                returnAdDetails.Title = "";
            }
            else
            {
                int i = 0;
                Dictionary<string, dynamic> dict = new Dictionary<string, dynamic>();

                var manager = UserPrincipalExtended.FindByIdentity(ADDetails.Manager.ToString());
                if (manager == null)
                {
                    Log.Instance.Error("Unable to determine manager information for samaccountname : " + samAccountName);
                }


                var headOfDivisionUN = UserPrincipalExtended.FindByIdentity(ADDetails.HeadOfDivision.ToString());
                if (headOfDivisionUN == null)
                {
                    Log.Instance.Error("Unable to determine Head of Division information for samaccountname : " + samAccountName);
                }

                dynamic temp = new ExpandoObject();
                temp.DisplayName = ADDetails.DisplayName;
                temp.Title = ADDetails.Title;
                dict.Add(i.ToString(), temp);
                i++;

                temp = new ExpandoObject();
                temp.DisplayName = manager.DisplayName;
                temp.Title = manager.Title;
                dict.Add(i.ToString(), temp);

                var managersManager = UserPrincipalExtended.FindByIdentity(manager.Manager.ToString());
                bool finishedFlag = false;
                while (finishedFlag == false)
                {
                    i++;
                    temp = new ExpandoObject();
                    if (managersManager.Manager.ToString() == "")
                    {
                        finishedFlag = true;
                        temp.DisplayName = managersManager.DisplayName;
                        temp.Title = managersManager.Title;
                        dict.Add(i.ToString(), temp);
                    }
                    else
                    {
                        temp.DisplayName = managersManager.DisplayName;
                        temp.Title = managersManager.Title;
                        dict.Add(i.ToString(), temp);
                        //get next step in the hierarchy
                        managersManager = UserPrincipalExtended.FindByIdentity(managersManager.Manager.ToString());
                    }
                    if (i == 20)
                    {
                        //in case of infinite loop stop after 20 loops
                        finishedFlag = true;
                    }
                }

                //map dictionary to dynamic
                var eo = new ExpandoObject();

                var eoColl = (ICollection<KeyValuePair<string, dynamic>>)eo;

                foreach (var kvp in dict)
                {
                    eoColl.Add(kvp);
                }

                dynamic eoDynamic = eo;

                returnAdDetails.Hierarchy = eoDynamic;
                returnAdDetails.DirectManager = manager == null ? "" : manager.DisplayName;
                returnAdDetails.DirectManagerTitle = manager == null ? "" : manager.Title;
                returnAdDetails.HeadOfDivision = headOfDivisionUN == null ? "" : headOfDivisionUN.DisplayName;
                returnAdDetails.Division = ADDetails == null ? "" : ADDetails.Division;
                returnAdDetails.Directorate = ADDetails == null ? "" : ADDetails.Directorate;
                returnAdDetails.Department = ADDetails == null ? "" : ADDetails.Department;
                returnAdDetails.Title = ADDetails == null ? "" : ADDetails.Title;
            }
            Assert.True( returnAdDetails!=null);

        }

    }

    public partial class UserPrincipalExtended : UserPrincipal//, IUserPrincipalExtended
    {
        public UserPrincipalExtended() : base(new PrincipalContext(ContextType.Domain))
        {
        }
        public UserPrincipalExtended(PrincipalContext context) : base(context)
        {
        }


        // Create the "Manager" property.    
        [DirectoryProperty("manager")]
        public string Manager
        {
            get
            {
                if (ExtensionGet("manager").Length != 1)
                    return string.Empty;

                return (string)ExtensionGet("manager")[0];
            }
            set { ExtensionSet("manager", value); }
        }

        // Create the "division" property.    
        [DirectoryProperty("division")]
        public string Division
        {
            get
            {
                if (ExtensionGet("division").Length != 1)
                    return string.Empty;

                return (string)ExtensionGet("division")[0];
            }
            set { ExtensionSet("division", value); }
        }


        // Create the "title" property.    
        [DirectoryProperty("title")]
        public string Title
        {
            get
            {
                if (ExtensionGet("title").Length != 1)
                    return string.Empty;

                return (string)ExtensionGet("title")[0];
            }
            set { ExtensionSet("title", value); }
        }

        // Create the "department" property.  -- department is section   
        [DirectoryProperty("department")]
        public string Department
        {
            get
            {
                if (ExtensionGet("department").Length != 1)
                    return string.Empty;

                return (string)ExtensionGet("department")[0];
            }
            set { ExtensionSet("department", value); }
        }

        // Create the "directorate" property.  
        [DirectoryProperty("msDS-cloudExtensionAttribute1")]
        public string Directorate
        {
            get
            {
                if (ExtensionGet("msDS-cloudExtensionAttribute1").Length != 1)
                    return string.Empty;

                return (string)ExtensionGet("msDS-cloudExtensionAttribute1")[0];
            }
            set { ExtensionSet("msDS-cloudExtensionAttribute1", value); }
        }



        // Create the "HeadOfDivision" property.  
        [DirectoryProperty("msDS-cloudExtensionAttribute2")]
        public string HeadOfDivision
        {
            get
            {
                if (ExtensionGet("msDS-cloudExtensionAttribute2").Length != 1)
                    return string.Empty;

                return (string)ExtensionGet("msDS-cloudExtensionAttribute2")[0];
            }
            set { ExtensionSet("msDS-cloudExtensionAttribute2", value); }
        }

        // Create the "ExternalAccess" property.  
        [DirectoryProperty("ExternalAccess")]
        public string ExternalAccess
        {
            get
            {
                if (ExtensionGet("msDS-cloudExtensionAttribute3").Length != 1)
                    return string.Empty;

                return (string)ExtensionGet("msDS-cloudExtensionAttribute3")[0];
            }
            set { ExtensionSet("msDS-cloudExtensionAttribute3", value); }
        }

        // Implement the overloaded search method FindByIdentity.
        public static UserPrincipalExtended FindByIdentity(string identityValue)
        {
            var context = AppServicesHelper.ActiveDirectory.adContext;
            return (UserPrincipalExtended)FindByIdentityWithType(context, typeof(UserPrincipalExtended), identityValue);
        }

        public bool ClearCache<T>() where T : UserPrincipalExtended
        {
            var inputDTO = Utility.JsonSerialize_IgnoreLoopingReference(Activator.CreateInstance(typeof(T), new object[] { new PrincipalContext(ContextType.Domain) }) as T);
            bool successRemove = AppServicesHelper.CacheD.Remove_BSO<dynamic>("API", "ActiveDirectory", "GetDirectory", inputDTO);
            return successRemove;
        }

        /// <summary>
        /// Returns full active directory hierarchy for a given user
        /// <param name="samAccountName"></param>
        /// </summary>
        public static dynamic GetAdHierarchy(string samAccountName)
        {
            Log.Instance.Debug("GetAdHierarchy for : " + samAccountName);

            dynamic ADDetails = AppServicesHelper.ActiveDirectory.Search<UserPrincipalExtended>(samAccountName);
            dynamic returnAdDetails = new ExpandoObject();

            //user not found
            var nullOrEmpty = (ADDetails == null || ADDetails.HeadOfDivision == null || string.IsNullOrEmpty(ADDetails.HeadOfDivision.ToString()));

            if (!nullOrEmpty)
                nullOrEmpty = (ADDetails.Manager == null || String.IsNullOrEmpty(ADDetails.Manager.ToString()));

            if (nullOrEmpty)
            {
                Log.Instance.Error("Acitve directory user details not found for : " + samAccountName);
                returnAdDetails.HeadOfDivision = "";
                returnAdDetails.Manager = "";

                returnAdDetails.Hierarchy = "";
                returnAdDetails.DirectManager = "";
                returnAdDetails.DirectManagerTitle = "";
                returnAdDetails.HeadOfDivision = "";
                returnAdDetails.Division = "";
                returnAdDetails.Directorate = "";
                returnAdDetails.Department = "";
                returnAdDetails.Title = "";
            }
            else
            {
                int i = 0;
                Dictionary<string, dynamic> dict = new Dictionary<string, dynamic>();

                var manager = UserPrincipalExtended.FindByIdentity(ADDetails.Manager.ToString());
                if (manager == null)
                {
                    Log.Instance.Error("Unable to determine manager information for samaccountname : " + samAccountName);
                }


                var headOfDivisionUN = UserPrincipalExtended.FindByIdentity(ADDetails.HeadOfDivision.ToString());
                if (headOfDivisionUN == null)
                {
                    Log.Instance.Error("Unable to determine Head of Division information for samaccountname : " + samAccountName);
                }

                dynamic temp = new ExpandoObject();
                temp.DisplayName = ADDetails.DisplayName;
                temp.Title = ADDetails.Title;
                dict.Add(i.ToString(), temp);
                i++;

                temp = new ExpandoObject();
                temp.DisplayName = manager.DisplayName;
                temp.Title = manager.Title;
                dict.Add(i.ToString(), temp);

                var managersManager = UserPrincipalExtended.FindByIdentity(manager.Manager.ToString());
                bool finishedFlag = false;
                while (finishedFlag == false)
                {
                    i++;
                    temp = new ExpandoObject();
                    if (managersManager.Manager.ToString() == "")
                    {
                        finishedFlag = true;
                        temp.DisplayName = managersManager.DisplayName;
                        temp.Title = managersManager.Title;
                        dict.Add(i.ToString(), temp);
                    }
                    else
                    {
                        temp.DisplayName = managersManager.DisplayName;
                        temp.Title = managersManager.Title;
                        dict.Add(i.ToString(), temp);
                        //get next step in the hierarchy
                        managersManager = UserPrincipalExtended.FindByIdentity(managersManager.Manager.ToString());
                    }
                    if (i == 20)
                    {
                        //in case of infinite loop stop after 20 loops
                        finishedFlag = true;
                    }
                }

                //map dictionary to dynamic
                var eo = new ExpandoObject();

                var eoColl = (ICollection<KeyValuePair<string, dynamic>>)eo;

                foreach (var kvp in dict)
                {
                    eoColl.Add(kvp);
                }

                dynamic eoDynamic = eo;

                returnAdDetails.Hierarchy = eoDynamic;
                returnAdDetails.DirectManager = manager == null ? "" : manager.DisplayName;
                returnAdDetails.DirectManagerTitle = manager == null ? "" : manager.Title;
                returnAdDetails.HeadOfDivision = headOfDivisionUN == null ? "" : headOfDivisionUN.DisplayName;
                returnAdDetails.Division = ADDetails == null ? "" : ADDetails.Division;
                returnAdDetails.Directorate = ADDetails == null ? "" : ADDetails.Directorate;
                returnAdDetails.Department = ADDetails == null ? "" : ADDetails.Department;
                returnAdDetails.Title = ADDetails == null ? "" : ADDetails.Title;
            }
            return returnAdDetails;
        }
    }
}
