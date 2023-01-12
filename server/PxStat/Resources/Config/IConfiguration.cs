namespace PxStat.Resources
{
    /// <summary>
    /// This will be injected into parts of the app that are better off not reading static.config, web.config etc
    /// Add to it as you see fit
    /// </summary>
    public interface IConfiguration
    {

        //bool ApiMemcachedEnabled { get; set; }
        //int ApiMemcachedMaxSize { get; set; }
        //int ApiMemcachedMaxValidity { get; set; }
        //string ApiMemcachedSalsa { get; set; }
    }
}
