using API;
using Newtonsoft.Json;
using PxStat.Subscription;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace PxStat.Security
{
    internal static class Throttle_BSO
    {


        //Should we allow FirebaseId to be set by a DTO parameter?  -would make things easier here....
        //or maybe better idea, set it at the template level
        //also, maybe see about reading subscriptions from the cache rather than from a db read request
        internal static bool IsThrottled(ADO Ado, HttpRequest hRequest, IRequest  request, string samAccountName = null)
        {

            //We need MemcacheD to use this
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["API_MEMCACHED_ENABLED"]))
                return false;


            int window;
            int cutoff;
            string user = null;
            bool subscribed = false;
            //Did the user send a SubscriberKey in the header of the request?
            if (hRequest.Headers.AllKeys.Contains("SubscriberKey"))
            {
                //They send a SubscriberKey, but is it in our list of valid tokens?
                var keyListCache = MemCacheD.Get_BSO("PxStat.Subscription", "Subscriber_BSO", "RefreshSubscriberKeyCache", "RefreshSubscriberKeyCache");
                if (!keyListCache.hasData)
                {
                    //No cache - try creating one
                    new Subscriber_BSO().RefreshSubscriberKeyCache(Ado);
                    keyListCache = MemCacheD.Get_BSO("PxStat.Subscription", "Subscriber_BSO", "RefreshSubscriberKeyCache", "RefreshSubscriberKeyCache");
                }


                if (keyListCache.hasData)
                {
                    //Does the request contain a valid subscription token?
                    var keyValues = keyListCache.data.ToObject<List<string>>();

                    if (keyValues.Contains(hRequest.Headers.GetValues("SubscriberKey").FirstOrDefault()))
                    {
                        user = hRequest.Headers.GetValues("SubscriberKey").FirstOrDefault();
                        subscribed = true;
                    }
                }
            }

            //An AD or Local user is deemed to be already subscribed
            if (samAccountName != null)
            {
                user = samAccountName;
                subscribed = true;
            }

            //Different limits apply depending on whether the user is subscribed or not
            if (subscribed)
            {
                window = Configuration_BSO.GetCustomConfig(ConfigType.server, "throttle.subscribedWindowSeconds");
                cutoff = Configuration_BSO.GetCustomConfig(ConfigType.server, "throttle.subscribedCallLimit");

            }
            else
            {
                window = Configuration_BSO.GetCustomConfig(ConfigType.server, "throttle.nonSubscribedWindowSeconds");
                cutoff = Configuration_BSO.GetCustomConfig(ConfigType.server, "throttle.nonSubscribedCallLimit");
                user = request.userAgent + request.ipAddress;
            }


            //Now we check the usage for the current requester
            List<DateTime> workingList = new List<DateTime>();
            var cache = MemCacheD.Get_BSO("PxStat.Security", "Throttle", "Read", user);
            if (cache.hasData)
            {
                List<DateTime> userHistory = JsonConvert.DeserializeObject<List<DateTime>>(cache.data.ToString());

                //We only count the entries inside the current window
                workingList = userHistory.Where(x => x > DateTime.Now.AddSeconds(window * -1)).ToList();
                if (workingList.Count() > cutoff + 1)
                {
                    Log.Instance.Info(String.Format("Throttle event for user {0}, {1} requests in {2} seconds", user, workingList.Count, window));
                    return true;
                }

            }

            workingList.Add(DateTime.Now);
            MemCacheD.Store_BSO("PxStat.Security", "Throttle", "Read", user, workingList, default(DateTime));


            return false;
        }

        /// <summary>
        /// Check if host is not in the whitelist
        /// </summary>
        /// <param name="whitelist"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        internal static bool IsNotInTheWhitelist(string[] whitelist, string host)
        {

            // If host is null or empty, it is not in the whitelist
            if (String.IsNullOrEmpty(host))
            {
                return true;
            }

            foreach(string value in whitelist)
            {
                // If host contains a whitelist value
                if (host.Contains(value))
                {
                    return false;
                }
            }
            return true;
        }

    }
}
