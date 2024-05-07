using API;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PxStat.Subscription
{
    public class Subscription_BSO
    {

        public dynamic TableRead(IADO ado, string tsbTable = null)
        {
            Subscription_ADO sAdo = new Subscription_ADO(ado);
            var subscriptions = sAdo.TableRead(tsbTable);


            return MergeUsers(subscriptions.data);

        }

        public dynamic ChannelRead(IADO ado, string lngIsoCode, string chnCode = null, bool singleLanguage = false)
        {
            Subscription_ADO sAdo = new Subscription_ADO(ado);
            var subscriptions = sAdo.ChannelRead(lngIsoCode, chnCode, null, singleLanguage);


            return MergeUsers(subscriptions.data);

        }

        private List<dynamic> MergeUsers(List<dynamic> subscriptionsData)
        {
            IDictionary<string, dynamic> adUsers = new Dictionary<string, dynamic>();
            if (!ApiServicesHelper.ApiConfiguration.Settings["API_AUTHENTICATION_TYPE"].Equals("ANONYMOUS"))
            {
                adUsers = AppServicesHelper.ActiveDirectory.List();
            }

            IDictionary<string, dynamic> fbUsers = AppServicesHelper.Firebase.GetAllUsers();
            if (fbUsers != null) fbUsers = new Dictionary<string, dynamic>();
            else return subscriptionsData;

            foreach (var s in subscriptionsData)
            {
                string email = "";
                string name = "";

                if (s.CcnEmail.Equals(DBNull.Value))
                {

                    if (!s.CcnUsername.Equals(DBNull.Value))
                    {
                        string adEntry = adUsers.Keys.Where(x => x.Equals(s.CcnUsername, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                        if (adEntry != null)
                        {
                            var ad = adUsers[adEntry];

                            if (ad != null)
                            {
                                email = ad.EmailAddress;
                                name = ad.GivenName + " " + ad.Surname;
                            }
                        }
                    }
                    else if (!s.SbrUserId.Equals(DBNull.Value))
                    {
                        var fbEntry = fbUsers.Keys.Where(y => y.Equals(s.SbrUserId)).FirstOrDefault();
                        if (fbEntry != null)
                        {
                            if (fbUsers.ContainsKey(fbEntry))
                            {
                                var fb = fbUsers[fbEntry];
                                if (fb != null)
                                {
                                    email = fb.Email;
                                    name = fb.DisplayName;
                                }
                            }
                        }
                    }
                    else continue;
                    s.CcnEmail = email;
                    s.FullName = name;
                }
            }

            return subscriptionsData.Where(x => !String.IsNullOrEmpty(x.CcnEmail)).ToList();
        }
    }


}
