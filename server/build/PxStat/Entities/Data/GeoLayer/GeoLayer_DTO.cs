namespace PxStat.Data
{
    public class GeoLayer_DTO_Create
    {
        public string GlrName { get; set; }

        public GeoLayer_DTO_Create(dynamic parameters)
        {
            if (parameters.GlrName != null)
                GlrName = parameters.GlrName;
        }
    }

    public class GeoLayer_DTO_Read
    {
        public string GlrCode { get; set; }

        public string GlrName { get; set; }

        public GeoLayer_DTO_Read(dynamic parameters)
        {
            if (parameters.GlrCode != null)
                GlrCode = parameters.GlrCode;

            if (parameters.GlrName != null)
                GlrName = parameters.GlrName;
        }

        public GeoLayer_DTO_Read()
        {
        }
    }

    public class GeoLayer_DTO_Update
    {
        public string GlrCode { get; set; }

        public string GlrName { get; set; }

        public GeoLayer_DTO_Update(dynamic parameters)
        {
            if (parameters.GlrCode != null)
                GlrCode = parameters.GlrCode;

            if (parameters.GlrName != null)
                GlrName = parameters.GlrName;
        }
    }

    public class GeoLayer_DTO_Delete
    {
        public string GlrCode { get; set; }

        public GeoLayer_DTO_Delete(dynamic parameters)
        {
            if (parameters.GlrCode != null)
                GlrCode = parameters.GlrCode;
        }
    }
}
