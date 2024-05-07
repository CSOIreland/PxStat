using API;

namespace PxStat.Config
{
    public static class CacheHelper
    {
        public static ICacheD cache

        {
            get
            {
                if (cache == null)
                {
                    cache = (MemCacheD)AppServicesHelper.ServiceProvider.GetService(typeof(MemCacheD));
                }
                return cache;
            }
            set { cache = value; }

        }

    }
}
