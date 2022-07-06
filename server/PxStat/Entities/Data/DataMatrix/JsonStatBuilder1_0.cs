using PxStat.Resources;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace PxStat.Data
{

    public class JsonStatBuilder1_0
    {
        public JsonStatV1 Create(IDmatrix matrix, string lngIsoCode, bool showData = true)
        {
            Dspec spec = matrix.Dspecs[lngIsoCode];
            var jsStat = new JsonStatV1();

            jsStat.Dataset = new Dataset();

            List<string> Id = new List<string>();

            List<long> Size = new List<long>();


            jsStat.Dataset.Label = spec.Title;

            // cells
            if (matrix.Cells == null)
            {
                matrix.Cells = new List<dynamic>();
            }

            if (!showData)
            {
                matrix.Cells = new List<dynamic>();
            }

            var dimension = new ExpandoObject() as IDictionary<string, Object>;



            RoleV1 role = new RoleV1();
            var statDim = spec.Dimensions.Where(x => x.Role == Constants.C_DATA_DIMENSION_ROLE_STATISTIC).FirstOrDefault();
            var timeDim = spec.Dimensions.Where(x => x.Role == Constants.C_DATA_DIMENSION_ROLE_TIME).FirstOrDefault();
            role.Metric = new List<string>() { statDim.Code };
            role.Time = new List<string>() { timeDim.Code };

            foreach (var aDimension in spec.Dimensions)
            {
                Dictionary<string, UnitV1> statUnit = null;
                if (aDimension.Role == Constants.C_DATA_DIMENSION_ROLE_STATISTIC)
                {
                    statUnit = statDim.Variables.ToDictionary(v => v.Code, v => new UnitV1() { Label = v.Unit });
                }
                var theDimension = new DimensionV1()
                {
                    Label = aDimension.Code,
                    Category = new CategoryV1()
                    {
                        Index = aDimension.Variables.ToDictionary(v => v.Code, v => aDimension.Variables.IndexOf(v)),
                        Label = aDimension.Variables.ToDictionary(v => v.Code, v => v.Value),
                        Unit = statUnit ?? null

                    }
                };

                dimension.Add(aDimension.Code, theDimension);
                Id.Add(aDimension.Code);
                Size.Add(aDimension.Variables.Count);

                if (aDimension.GeoFlag && !string.IsNullOrEmpty(aDimension.GeoUrl))
                {
                    if (role.Geo == null)
                    {
                        role.Geo = new List<string>();
                    }
                    role.Geo.Add(aDimension.Code);


                }


            }
            dimension.Add("role", role);
            dimension.Add("id", Id);
            dimension.Add("size", Size);


            jsStat.Dataset.Dimension = dimension;

            jsStat.Dataset.Source = matrix.Copyright.CprValue;
            jsStat.Dataset.Updated = matrix.CreatedDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ");

            jsStat.Dataset.Value = new JsonStatValue() { AnythingArray = matrix.Cells.Select(c => (ValueElement)c).ToList() };

            return jsStat;
        }
    }
}
