using API;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PxStat.Subscription
{
    internal class Subscription_BSO
    {

        internal dynamic TableRead(ADO ado, string tsbTable = null)
        {
            Subscription_ADO sAdo = new Subscription_ADO(ado);
            var subscriptions = sAdo.TableRead(tsbTable);


            return MergeUsers(subscriptions.data);

        }

        internal dynamic ChannelRead(ADO ado, string lngIsoCode, string chnCode = null, bool singleLanguage = false)
        {
            Subscription_ADO sAdo = new Subscription_ADO(ado);
            var subscriptions = sAdo.ChannelRead(lngIsoCode, chnCode, null, singleLanguage);


            return MergeUsers(subscriptions.data);

        }

        private dynamic MergeUsers(List<dynamic> subscriptionsData)
        {
            var adUsers = ActiveDirectory.List();

            IDictionary<string, dynamic> fbUsers = API.Firebase.GetAllUsers();

            foreach (var s in subscriptionsData)
            {
                string email = "";
                string name = "";

                if (s.CcnEmail.Equals(DBNull.Value))
                {

                    if (!s.CcnUsername.Equals(DBNull.Value))
                    {
                        string adEntry = adUsers.Keys.Where(x => x.Equals(s.CcnUsername, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                        var ad = adUsers[adEntry];
                        if (ad != null)
                        {
                            email = ad.EmailAddress;
                            name = ad.GivenName + " " + ad.Surname;
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

            return subscriptionsData.Where(x => !String.IsNullOrEmpty(x.CcnEmail));
        }
    }


}
