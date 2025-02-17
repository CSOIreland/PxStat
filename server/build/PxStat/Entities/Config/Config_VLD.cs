using API;
using FluentValidation;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace PxStat.Config
{
    /// <summary>
    /// Validator for Configuration Read
    /// </summary>
    internal class Config_VLD_Read : AbstractValidator<Config_DTO_Read>
    {

        internal Config_VLD_Read()
        {
            //Mandatory - name
            RuleFor(f => f.name).NotEmpty().WithMessage("Invalid name is null or empty, must require a value").When(x=>x.type.Equals("APP"));
        }
    }

    /// <summary>
    /// Validator for Configuration Update
    /// </summary>
    internal class Config_VLD_Create : AbstractValidator<Config_DTO_Create>
    {
        internal Config_VLD_Create()
        {
            //Mandatory - name
            RuleFor(f => f.name).NotEmpty().WithMessage("Invalid Configuration name is null or empty, must require a value");
            //Mandatory - value
            RuleFor(f => ValidateCreateConfig(f.name, f.value, f.type)).Equal(true).WithMessage("Invalid Configuration value is not valid, validation with schema failed");
        }

        public bool ValidateCreateConfig(string name, string value, string type)
        {
            if (type != null && type.Equals("API"))
            {
                return true;
            }
            var directory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            // Replace config with schema in name
            name = name.Replace("config", "schema");
            
            string schema = File.ReadAllText(directory + @"\Resources\Schemas\" + name);

            var model = JObject.Parse(value);
            var json_schema = JSchema.Parse(schema);

            IList<string> messages;
            bool valid = model.IsValid(json_schema, out messages);
            if (!valid)
            {
                foreach (var message in messages)
                {
                    Log.Instance.Debug(message);
                }
            }
            return valid;
        }
    }

    /// <summary>
    /// Validator for Configuration Update
    /// </summary>
    internal class Config_VLD_Update : AbstractValidator<Config_DTO_Create>
    {
        internal Config_VLD_Update()
        {
            //Mandatory - name
            RuleFor(f => f.name).NotEmpty().WithMessage("Invalid Configuration name is null or empty, must require a value");
            //Mandatory - value
            RuleFor(f => ValidateCreateConfig(f.name, f.value, f.type)).Equal(true).WithMessage("Invalid Configuration value is not valid, validation with schema failed");
        }

        public bool ValidateCreateConfig(string name, string value, string type)
        {
            if (type != null && type.Equals("API"))
            {
                return true;
            }
            var directory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            if (name.Contains("config"))
            {
                // Replace config with schema in name
                name = name.Replace("config", "schema");
            }
            else
            {
                // Name is FirebaseKey.json
                name = "schema." + name;
            }
            string schema = File.ReadAllText(directory + @"\Resources\Schemas\" + name);

            var model = JObject.Parse(value);
            var json_schema = JSchema.Parse(schema);

            IList<string> messages;
            bool valid = model.IsValid(json_schema, out messages);
            if (!valid)
            {
                foreach (var message in messages)
                {
                    Log.Instance.Debug(message);
                }
            }
            return valid;
        }
    }

    /// <summary>
    /// Validator for Configuration ReadSchema
    /// </summary>
    internal class Config_VLD_ReadSchema : AbstractValidator<Config_DTO_ReadSchema>
    {

        internal Config_VLD_ReadSchema()
        {
            //Mandatory - name
            RuleFor(f => f.name).NotEmpty().WithMessage("Invalid name is null or empty, must require a value");
        }
    }

    internal class Config_VLD_Ping : AbstractValidator<Config_DTO_Ping>
    {
        internal Config_VLD_Ping()
        {
        }
    }

    internal class Config_VLD_ReadVersions : AbstractValidator<Config_DTO_ReadVersions>
    {
        internal Config_VLD_ReadVersions()
        {
            //Mandatory - type
            RuleFor(f => f.type).NotEmpty().WithMessage("Invalid type is null or empty, must require a value");
        }
    }
}