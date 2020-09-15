namespace PxStat.Security
{
    internal class Cache_DTO_Read
    {
        public string node;
        public Cache_DTO_Read(dynamic parameters)
        {
            if (parameters.node != null)
                this.node = parameters.node;
        }
    }

    internal class Cache_DTO_Update
    {
        public Cache_DTO_Update(dynamic parameters) { }
    }
}
