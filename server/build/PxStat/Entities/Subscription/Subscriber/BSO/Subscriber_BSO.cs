using API;
using PxStat.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;

namespace PxStat.Subscription
{
    /// <summary>
    /// 
    /// </summary>
    public class Subscriber_BSO
    {
        /// <summary>
        /// Reads one or all subscribers
        /// set userId to a value if you only want to read one subscriber (this is not cached)
        /// set readCache to false if you want to NOT read the cache otherwise true or null
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="userId"></param>
        /// <param name="readCache"></param>
        /// <returns></returns>
        public List<dynamic> GetSubscribers(IADO ado, string userId = null, bool? readCache = true)
        {

            //We may also read the subscriber data from the cache under certain circumstances
            if ((bool)readCache && userId == null)
            {

                var subs =AppServicesHelper.CacheD.Get_BSO("PxStat.Subscription", "Subscriber_BSO", "GetSubscribers", "GetSubscribers");
                if (subs.hasData)
                {
                    return subs.data.ToObject<List<dynamic>>(); ;
                }
            }
            

            IDictionary<string, dynamic> fbUsers = AppServicesHelper.Firebase.GetAllUsers();
            if(fbUsers==null)   fbUsers = new Dictionary<string, dynamic>();
            Subscriber_ADO sAdo = new Subscriber_ADO(ado);


            var subscribers = sAdo.Read(userId);

            IDictionary<string, dynamic> adUsers = new Dictionary<string, dynamic>();
            if (!ApiServicesHelper.ApiConfiguration.Settings["API_AUTHENTICATION_TYPE"].Equals("ANONYMOUS"))
            {
                adUsers = AppServicesHelper.ActiveDirectory.List();
            }
            
            foreach (var s in subscribers.data)
            {

                string email = null;
                string displayName = null;
                if (s.CcnEmail == DBNull.Value)
                {
                    string adEntry = adUsers.Keys.Where(x => x.Equals(s.SbrUserId, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    if (adEntry != null)
                    {
                        var ad = adUsers[adEntry];
                        if (ad != null)
                        {
                            email = ad.EmailAddress.Value;
                            if (ad.GivenName != null && ad.Surname != null)
                                displayName = ad.GivenName + " " + ad.Surname;
                        }
                    }
                    else
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
                                    displayName = fb.DisplayName;
                                }
                            }
                        }
                    }
                    s.CcnEmail = email;
                    s.DisplayName = displayName;
                }
            }
            if (userId == null && subscribers.data.Count > 0)
               AppServicesHelper.CacheD.Store_BSO("PxStat.Subscription", "Subscriber_BSO", "GetSubscribers", "GetSubscribers", subscribers.data, DateTime.Now.AddHours(24));

            return subscribers.data;

        }

        internal bool Delete(IADO Ado, string subscriberUserId)
        {
            Subscriber_ADO ado = new Subscriber_ADO(Ado);
            if (ado.Delete(subscriberUserId))
            {
                RefreshSubscriberKeyCache(Ado);
                return true;
            }
            return false;
        }

        internal bool Create(IADO Ado, string sbrPreference, string firebaseId, string lngIsoCode)
        {
            
                Subscriber_ADO ado = new Subscriber_ADO(Ado);
                string subscriberKey = GetSubscriberKey(firebaseId);
                if (ado.Create(sbrPreference, firebaseId, lngIsoCode, subscriberKey))
                {
                    Ado.CommitTransaction();

                    //Refresh the cache of valid subscriber keys
                    //These are used for throttling
                    RefreshSubscriberKeyCache(Ado);

                    return true;
                }
                else
                {                   
                    return false;
                }
           

        }


        /// <summary>
        /// The Subscriber Key Cache is used for throttling traffic
        /// This method refreshes the Subscriber Key Cache
        /// </summary>
        /// <param name="ado"></param>
        internal void RefreshSubscriberKeyCache(IADO ado)
        {
            Subscriber_ADO sAdo = new Subscriber_ADO(ado);
            var response = sAdo.ReadSubscriberKeys();
            if (response.Count > 0)
            {
                List<string> sList = new List<string>();
                foreach (var item in response)
                {
                    sList.Add(item.SbrKey);
                }
               AppServicesHelper.CacheD.Store_BSO("PxStat.Subscription", "Subscriber_BSO", "RefreshSubscriberKeyCache", "RefreshSubscriberKeyCache", sList, DateTime.Now.AddHours(24));
            }

        }


        /// <summary>
        /// Generates a subscriber key based on the firebase uid and the salsa
        /// </summary>
        /// <param name="firebaseId"></param>
        /// <returns></returns>
        internal string GetSubscriberKey(string firebaseId)
        {
            //ConfigurationManager.AppSettings["APP_FIREBASE_SALSA"]
            return Utility.GetSHA256(new Random().Next() + Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "salsa.firebase") + firebaseId + DateTime.Now.Millisecond);
        }
    }




}
