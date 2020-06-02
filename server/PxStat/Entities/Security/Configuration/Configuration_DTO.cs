namespace PxStat.Security
{
    internal class Configuration_DTO_Read
    {
        public string node;
        public Configuration_DTO_Read(dynamic parameters)
        {
            if (parameters.node != null)
                this.node = parameters.node;
        }
    }

    internal class Configuration_DTO_Update
    {
        public Configuration_DTO_Update(dynamic parameters) { }
    }
}
