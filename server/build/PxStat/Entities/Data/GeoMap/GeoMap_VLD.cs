using API;
using FluentValidation;
using PxStat.Security;
using System.Collections.Generic;
using System.Linq;

namespace PxStat.Data
{
    internal class GeoMapValidator : AbstractValidator<GeoMap_DTO_Validate>
    {
        internal GeoMapValidator()
        {
            RuleFor(x => x.GeoJson).NotNull().WithMessage("Failed to parse GeoJson");
            RuleFor(x => x.GmpGeoJson).NotNull().WithMessage("Failed to parse GeoJson");
            RuleFor(x => x).Must(new CustomGeoValidations().ValidateFeatures).WithMessage("Invalid Features");
            RuleFor(x => x).Must(new CustomGeoValidations().ValidateGeometry).WithMessage("Invalid Geometry");
        }
    }
    internal class GeoMap_VLD_Validate : AbstractValidator<GeoMap_DTO_Validate>
    {
        internal GeoMap_VLD_Validate()
        {

        }
    }
    internal class GeoMap_VLD_Create : AbstractValidator<GeoMap_DTO_Create>
    {
        internal GeoMap_VLD_Create()
        {
            RuleFor(x => x.GeoJson).NotNull().WithMessage("Failed to parse GeoJson");
            RuleFor(x => x.GmpGeoJson).NotNull().WithMessage("Failed to parse GeoJson");
            RuleFor(x => x).Must(new CustomGeoValidations().ValidateFeatures).WithMessage("Invalid Features");
            RuleFor(x => x).Must(new CustomGeoValidations().ValidateGeometry).WithMessage("Invalid Geometry");
            RuleFor(x => x.GmpName).Length(0, 256).WithMessage("GmpName invalid length");
        }

    }

    internal class CustomGeoValidations
    {
        internal bool ValidateProperties(dynamic dto)
        {
            if (dto.GeoJson?.Features == null) return false;
            List<string> codes = new List<string>();
            foreach (var feature in dto.GeoJson.Features)
            {
                if (feature.Type != "Feature") return false;
                if (!feature.Properties.ContainsKey(Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code"))) return false;
                if (!feature.Properties.ContainsKey("code")) return false;

                codes.Add(feature.Properties["code"]);
            }

            //check for duplicate "code" values
            var group = codes.GroupBy(x => x)
               .Where(g => g.Count() > 1)
               .Select(y => y.Key)
               .ToList();
            if (group.Count > 0) return false;
            return true;
        }

        internal bool ValidateFeatures(dynamic dto)
        {
            if (dto.GeoJson?.Features == null) return false;
            if (dto.GeoJson.Features.Length == 0) return false;
            return true;
        }
        /// <summary>
        /// Rules: You can only use a geometry that is listed either in APP_GEOMAP_ALLOWED_GEOMETRY_MULTI or APP_GEOMAP_ALLOWED_GEOMETRY_UNIQUE
        /// If a geometry is in APP_GEOMAP_ALLOWED_GEOMETRY_MULTI you can allow it to exist with other geometries in APP_GEOMAP_ALLOWED_GEOMETRY_MULTI
        /// If a geometry is in APP_GEOMAP_ALLOWED_GEOMETRY_UNIQUE then no other geometry can exist in the map
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal bool ValidateGeometry(dynamic dto)
        {
            if (dto.GeoJson?.Features == null) return false;
            //Get list of geometries that can be unique or otherwise
            List<string> multi = (Utility.GetCustomConfig("APP_GEOMAP_ALLOWED_GEOMETRY_MULTI")).Split(',').ToList<string>();
            List<string> unique = (Utility.GetCustomConfig("APP_GEOMAP_ALLOWED_GEOMETRY_UNIQUE")).Split(',').ToList<string>();
            string uniqueValue = null;
            int uniqueCount = 0;
            foreach (var feature in dto.GeoJson.Features)
            {
                //Unique value found, store it and count it
                if (unique.Contains(feature.Geometry.Type) && uniqueValue == null) uniqueValue = feature.Geometry.Type;

                if (unique.Contains(feature.Geometry.Type) && uniqueValue.Equals(feature.Geometry.Type)) uniqueCount++;


                //Don't allow geometries that are neither on the unique nor the multi list
                if (!multi.Contains(feature.Geometry.Type) && !unique.Contains(feature.Geometry.Type)) return false;
            }
            if (uniqueValue != null)
            {
                //unique value is not unique
                if (uniqueCount < dto.GeoJson.Features.Length) return false;
            }
            return true;
        }

        internal bool ValidateGeometryBasic(dynamic dto)
        {
            if (dto.GeoJson?.Features == null) return false;

            foreach (var feature in dto.GeoJson.Features)
            {
                if (feature.Geometry == null) return false;
            }

            return true;
        }

    }

    internal class GeoMap_VLD_Delete : AbstractValidator<GeoMap_DTO_Delete>
    {
        internal GeoMap_VLD_Delete()
        {
            RuleFor(x => x.GmpCode).Length(32).WithMessage("Invalid GmpCode");
        }
    }

    internal class GeoMap_VLD_ReadCollection : AbstractValidator<GeoMap_DTO_ReadCollection>
    {
        internal GeoMap_VLD_ReadCollection()
        {
            RuleFor(x => x.GmpCode).Length(0, 32).When(x => !string.IsNullOrEmpty(x.GmpCode)).WithMessage("GmpCode invalid length");
        }
    }

    internal class GeoMap_VLD_Update : AbstractValidator<GeoMap_DTO_Update>
    {
        internal GeoMap_VLD_Update()
        {
            RuleFor(x => x.GmpName).Length(0, 256).WithMessage("GmpName invalid length");
            RuleFor(x => x.GmpCode).Length(0, 32).WithMessage("GmpCode invalid length");
            RuleFor(x => x.GlrCode).Length(0, 256).WithMessage("GlrCode invalid length");
        }
    }
}
