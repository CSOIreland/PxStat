using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Data;
using System.Dynamic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.InteropServices;
using System;
using System.Reflection.Metadata;
using System.Reflection;

namespace PxStatMigrateAppConfig
{
    internal class Helper
    {
        /// <summary>
        /// For entering passwords
        /// </summary>
        /// <returns></returns>
        internal static string InputConfidential()
        {
            StringBuilder passwordBuilder = new StringBuilder();
            bool continueReading = true;
            char newLineChar = '\r';
            int counter = 0;
            while (continueReading)
            {
                ConsoleKeyInfo consoleKeyInfo = Console.ReadKey(true);
                char passwordChar = consoleKeyInfo.KeyChar;

                if (passwordChar == newLineChar)
                {
                    continueReading = false;
                }
                else if (passwordChar == '\b')
                {
                    Console.CursorLeft--;
                    Console.Write('\0');
                    Console.CursorLeft--;
                    passwordBuilder.Remove(counter - 1, 1);
                }
                else if (passwordChar == ('\0'))
                {

                }
                else
                {
                    Console.Write("*");
                    passwordBuilder.Append(passwordChar.ToString());
                }
                counter++;
            }
            Console.WriteLine();
            return passwordBuilder.ToString();
        }

        /// <summary>
        /// Get text file contents as a string
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static string GetFileString(string path, Dictionary<string, string> replacements = null)
        {
            try
            {
                using (var streamReader = new StreamReader(path, Encoding.UTF8))
                {
                    string ostring = streamReader.ReadToEnd();
                    if (replacements != null)
                    {
                        foreach (var item in replacements)
                        {
                            ostring = ostring.Replace(item.Key, item.Value);
                        }
                    }
                    return ostring;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error opening file " + path + ":" + ex.Message);

                return null;
            }

        }

        /// <summary>
        /// Validate the json based on schema
        /// </summary>
        /// <param name="json"></param>
        /// <param name="schemaString"></param>
        /// <returns></returns>
        internal static bool ValidateJson(string json, string schemaString)
        {
            if (json == null || schemaString == null) return false;
            try
            {
                var schema = JSchema.Parse(schemaString);
                JObject jsonObject = JObject.Parse(json);
                IList<string> messages;

                bool isValid = jsonObject.IsValid(schema, out messages);
                if (messages != null)
                {
                    foreach (string message in messages)
                    {
                        Console.WriteLine("Validation error: " + message);
                    }
                }
                return isValid;
            }
            catch (Exception e)
            {
                Console.WriteLine("Parsing error: " + e.Message);
                return false;

            }
        }

        /// <summary>
        /// Write the new configuration to the database using the App_Settings_Write stored procedure
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="configName"></param>
        /// <param name="configValue"></param>
        /// <returns></returns>
        internal static bool WriteAppConfigToDatabase(string connectionString, string configName, string configValue, string ccnUsername)
        {
            try
            {

                using (SqlConnection openCon = new SqlConnection(connectionString))
                {
                    string createConfigCommand = "App_Settings_Migrate_Write";

                    using (SqlCommand queryCreateConfig = new SqlCommand(createConfigCommand))
                    {
                        queryCreateConfig.Connection = openCon;
                        queryCreateConfig.CommandType = CommandType.StoredProcedure;
                        queryCreateConfig.Parameters.Add("@appkey", SqlDbType.VarChar, 200).Value = configName;
                        queryCreateConfig.Parameters.Add("@appvalue", SqlDbType.VarChar, -1).Value = configValue;
                        queryCreateConfig.Parameters.Add("@appdescription", SqlDbType.VarChar, -1).Value = "Json config for " + configName;
                        queryCreateConfig.Parameters.Add("@userName", SqlDbType.VarChar, 256).Value = ccnUsername;


                        openCon.Open();

                        queryCreateConfig.ExecuteNonQuery();
                        openCon.Close();
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Database insert error: " + e.Message + "\n");
                return false;
            }
        }

        internal static decimal GetLatestVersion(string connectionString, int configType)
        {
            decimal version = 0;
            using (SqlConnection openCon = new SqlConnection(connectionString))
            {
                string readLatestCommand = "App_Settings_Read_Version";
                if (openCon.State.Equals(ConnectionState.Closed)) openCon.Open();

                using (SqlCommand queryReadLatest = new SqlCommand(readLatestCommand))
                {
                    queryReadLatest.CommandType = CommandType.StoredProcedure;
                    queryReadLatest.Connection = openCon;
                    queryReadLatest.Parameters.Add("@cst_id", SqlDbType.Int).Value = configType;

                    SqlDataReader dr = queryReadLatest.ExecuteReader();
                    while (dr.Read())
                    {
                        version = (decimal)dr[0];
                    }

                    dr.Close();
                }
            }

            return version;
        }


        internal static bool WriteApiConfigToDatabase(string connectionString, string configName, string configValue, string ccnUsername, bool sensitive = false)
        {
            try
            {
                using (SqlConnection openCon = new SqlConnection(connectionString))
                {
                    string readLatestCommand = "App_Settings_Read_Version";
                    if (openCon.State.Equals(ConnectionState.Closed)) openCon.Open();
                    decimal version = 0;
                    using (SqlCommand queryReadLatest = new SqlCommand(readLatestCommand))
                    {
                        queryReadLatest.CommandType = CommandType.StoredProcedure;
                        queryReadLatest.Connection = openCon;
                        queryReadLatest.Parameters.Add("@cst_id", SqlDbType.Int).Value = 1;

                        SqlDataReader dr = queryReadLatest.ExecuteReader();
                        while (dr.Read())
                        {
                            version = (decimal)dr[0];
                        }

                        dr.Close();
                    }

                    string createConfigCommand = "Api_Settings_Migrate_Write";

                    using (SqlCommand queryCreateConfig = new SqlCommand(createConfigCommand))
                    {
                        queryCreateConfig.Connection = openCon;
                        queryCreateConfig.CommandType = CommandType.StoredProcedure;

                        queryCreateConfig.Parameters.Add("@apiversion", SqlDbType.Decimal).Value = version;
                        queryCreateConfig.Parameters.Add("@apikey", SqlDbType.VarChar, 200).Value = configName;
                        queryCreateConfig.Parameters.Add("@apivalue", SqlDbType.VarChar, -1).Value = configValue;
                        queryCreateConfig.Parameters.Add("@apidescription", SqlDbType.VarChar, -1).Value = "Json config for " + configName;
                        queryCreateConfig.Parameters.Add("@userName", SqlDbType.NVarChar).Value = ccnUsername;

                        if (sensitive)
                        {
                            queryCreateConfig.Parameters.Add("@appsensitivevalue", SqlDbType.Bit).Value = sensitive;
                        }

                        if (openCon.State.Equals(ConnectionState.Closed)) openCon.Open();

                        queryCreateConfig.ExecuteNonQuery();
                        openCon.Close();
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Database insert error: " + e.Message + "\n");
                throw e;
            }
        }

        /// <summary>
        /// Create a new app config version
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        internal static bool CreateNewAppConfigVersion(string connectionString)
        {
            decimal version = 0;
            try
            {
                using (SqlConnection openCon = new SqlConnection(connectionString))
                {
                    // First get the latest config version
                    string readLatestCommand = "App_Settings_Read_Version";
                    openCon.Open();

                    using (SqlCommand queryReadLatest = new SqlCommand(readLatestCommand))
                    {
                        queryReadLatest.CommandType = CommandType.StoredProcedure;
                        queryReadLatest.Connection = openCon;
                        queryReadLatest.Parameters.Add("@cst_id", SqlDbType.Int).Value = 2;

                        SqlDataReader dr = queryReadLatest.ExecuteReader();
                        while (dr.Read())
                        {
                            version = (decimal)dr[0];
                        }
                        //Increment the version
                        version = version + 1;
                        dr.Close();
                    }



                    //Now we can write the new version details
                    string updateVersionCommand = "App_Settings_Insert_Version";

                    using (SqlCommand queryUpdateVersion = new SqlCommand(updateVersionCommand))
                    {
                        queryUpdateVersion.CommandType = CommandType.StoredProcedure;
                        queryUpdateVersion.Connection = openCon;

                        queryUpdateVersion.Parameters.Add("@version_id", SqlDbType.NVarChar).Value = version.ToString();
                        queryUpdateVersion.Parameters.Add("@cst_id", SqlDbType.Int).Value = 2;



                        queryUpdateVersion.ExecuteNonQuery();
                        openCon.Close();
                    }
                }


                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine("Error creating new app config version: " + e.Message + "\n");
                throw e;
            }
        }


        /// <summary>
        /// Create a new api config version
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        internal static decimal CreateNewApiConfigVersion(string connectionString)
        {
            decimal version = 0;
            try
            {
                using (SqlConnection openCon = new SqlConnection(connectionString))
                {
                    // First get the latest config version
                    string readLatestCommand = "App_Settings_Read_Version";
                    openCon.Open();

                    using (SqlCommand queryReadLatest = new SqlCommand(readLatestCommand))
                    {
                        queryReadLatest.CommandType = CommandType.StoredProcedure;
                        queryReadLatest.Connection = openCon;
                        queryReadLatest.Parameters.Add("@cst_id", SqlDbType.Int).Value = 1;

                        SqlDataReader dr = queryReadLatest.ExecuteReader();
                        while (dr.Read())
                        {
                            version = (decimal)dr[0];
                        }
                        //Increment the version
                        version = version + 1;
                        dr.Close();
                    }



                    //Now we can write the new version details
                    string updateVersionCommand = "ApP_Settings_Insert_Version";

                    using (SqlCommand queryUpdateVersion = new SqlCommand(updateVersionCommand))
                    {
                        queryUpdateVersion.CommandType = CommandType.StoredProcedure;
                        queryUpdateVersion.Connection = openCon;

                        queryUpdateVersion.Parameters.Add("@version_id", SqlDbType.NVarChar).Value = version.ToString();
                        queryUpdateVersion.Parameters.Add("@cst_id", SqlDbType.Int).Value = 1;



                        queryUpdateVersion.ExecuteNonQuery();
                        openCon.Close();
                    }
                }


                return version;

            }
            catch (Exception e)
            {
                Console.WriteLine("Error creating new api config version: " + e.Message + "\n");
                throw e;
            }
        }


        internal static bool ExecuteSql(string connectionString, string sql)
        {
            try
            {


                // split script on GO command
                IEnumerable<string> commandStrings = Regex.Split(sql, @"^\s*GO\s*$",
                                         RegexOptions.Multiline | RegexOptions.IgnoreCase);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    foreach (string commandString in commandStrings)
                    {
                        if (commandString.Trim() != "")
                        {
                            using (var command = new SqlCommand(commandString, connection))
                            {
                                try
                                {
                                    command.ExecuteNonQuery();
                                }
                                catch (SqlException ex)
                                {
                                    string spError = commandString.Length > 100 ? commandString.Substring(0, 100) + " ...\n..." : commandString;
                                    string message = "Error running sql: " + sql + " " + ex.Message;
                                    throw new Exception(message);
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                string fmessage = "Error running sql: " + sql + " " + ex.Message;

                throw new Exception(fmessage); ;
            }
        }


        internal static List<string> GetAllFiles(string folderPath)
        {
            List<string> files = new();
            if (Directory.Exists(folderPath))
            {
                foreach (string file in Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories))
                {
                    files.Add(file);
                }
            }
            List<string> sorted = new();
            var priority = files.Where(x => x.Contains("Security_Auditing_")).ToList();
            sorted.AddRange(priority);
            foreach (var file in files)
            {
                if (!sorted.Contains(file))
                { sorted.Add(file); }
            }
            return sorted;
        }

        internal static List<string> GetUpgradeList(string folderPath, string fromVersion)
        {
            List<string> files = GetAllFiles(folderPath);
            string thisVersionFilePath = folderPath + "\\" + fromVersion;
            if (files.Count == 0) return new List<string>();
            List<string> sortedList = files.Where(x => x.CompareTo(thisVersionFilePath) > 0).ToList();
            return sortedList.OrderBy(x => x).ToList();
        }

        internal static Dictionary<string, string> GetWebConfigOld(string folderPath)
        {
            Dictionary<string, string> config = new();
            try
            {
                using (var streamReader = new StreamReader(folderPath, Encoding.UTF8))
                {
                    while (!streamReader.EndOfStream)
                    {
                        string ostring = streamReader.ReadLine() ?? string.Empty;
                        if (ostring.Contains("<add key="))
                        {
                            var hits = Regex.Split(ostring, "\"(.*?)\"");
                            if (hits.Length == 5)
                                config.Add(hits[1], hits[3]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading config file " + folderPath + ": " + ex.Message);
                throw ex;
            }
            return config;
        }

        internal static Dictionary<string, string> GetWebConfig(string folderPath)
        {

            Dictionary<string, string> config = new();
            try
            {
                using (var streamReader = new StreamReader(folderPath, Encoding.UTF8))
                {
                    while (!streamReader.EndOfStream)
                    {
                        string ostring = streamReader.ReadLine() ?? string.Empty;
                        ostring = ostring.Trim();
                        if (ostring.Length > 9)
                        {
                            if (ostring.Substring(0, 9).Equals("<add key="))
                            {
                                while (ostring.Substring(ostring.Length - 2, 2) != "/>")
                                    ostring = ostring + streamReader.ReadLine().Trim();
                            }
                            if (ostring.Contains("<add key="))
                            {
                                var val = Regex.Matches(ostring, "value=(\\n|.)*?\\/>", RegexOptions.IgnoreCase);
                                var key = Regex.Matches(ostring, "key=(\n|.)*?value", RegexOptions.IgnoreCase);

                                if (val.Count > 0 && key.Count > 0)
                                {
                                    string vstring = val[0].Value.Trim();
                                    vstring = Regex.Replace(vstring, "^[^\"]*\"", "");//Remove everything up to and including first "
                                    vstring = Regex.Replace(vstring, "\"(?!.*\").*", "");//Remove last " and everything after it
                                    string kstring = key[0].Value.Trim();
                                    kstring = Regex.Replace(kstring, "^[^\"]*\"", "");
                                    kstring = Regex.Replace(kstring, "\"(?!.*\").*", "");
                                    config.Add(kstring.Trim(), vstring.Trim());
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading config file " + folderPath + ": " + ex.Message);
                throw ex;
            }
            return config;
        }

        //Copy the connection strings and memcached data from web.config an appsettings.json string
        internal static string GetUpdatedAppConfig(string filePath, string webConfigPath)
        {
            string appConfigText = "";
            var enyimMemcached = new ExpandoObject() as IDictionary<string, Object>;


            List<object> serverList = null;
            List<IDictionary<string, Object>> dbConfigs = new List<IDictionary<string, Object>>();
            Dictionary<string, object> dbDefault = new();
            using (var streamReader = new StreamReader(filePath, Encoding.UTF8))
            {
                appConfigText = streamReader.ReadToEnd();
            }

            var objJson = JsonConvert.DeserializeObject<ExpandoObject>(appConfigText, new ExpandoObjectConverter());

            var dJson = objJson as IDictionary<string, Object>;
            if (dJson == null) { return null; }
            //update connection strings in the app config to the version in the existing web.config

            try
            {
                bool memCacheTags = false;
                using (var streamReader = new StreamReader(webConfigPath, Encoding.UTF8))
                {
                    while (!streamReader.EndOfStream)
                    {
                        string ostring = streamReader.ReadLine() ?? string.Empty;
                        ostring = ostring.Trim();
                        if (ostring.Length > 9)
                        {
                            if (ostring.Contains("connectionString="))
                            {
                                while (ostring.Substring(ostring.Length - 2, 2) != "/>")
                                {
                                    string readString = streamReader.ReadLine().Trim();
                                    ostring = ostring + readString;
                                }
                                var cstringMatch = Regex.Matches(ostring, "connectionString\\s*=\\s*\"([^\"]*)\"", RegexOptions.IgnoreCase); //connection string plus the first thing in quotes after it
                                var nameMatch = Regex.Matches(ostring, "name\\s*=\\s*\"([^\"]*)\"", RegexOptions.IgnoreCase); //name plus the first thing in quotes after it
                                var providerMatch = Regex.Matches(ostring, "provider\\s*=\\s*\"([^\"]*)\"", RegexOptions.IgnoreCase);
                                var defaultConnectionMatch = ostring.ToLower().Contains("defaultconnection");

                                var nameValues = GetStringFromMatch(nameMatch, "name");
                                var cstringValues = GetStringFromMatch(cstringMatch, "connectionString");
                                var providerValues = GetStringFromMatch(providerMatch, "provider");

                                if (nameValues.Count > 0 && cstringValues.Count > 0 && defaultConnectionMatch)
                                {
                                    var dbConnection = new ExpandoObject() as IDictionary<string, Object>;
                                    dbConnection.Add(nameValues["name"], cstringValues["connectionString"]);
                                    dbConfigs.Add(dbConnection);
                                    dbDefault.Add("DefaultConnection", cstringValues["connectionString"]);
                                }

                            }
                            else if (ostring.Contains("<enyim.com>"))
                            {
                                memCacheTags = true;
                                serverList = new();


                            }

                            if (ostring.Contains("</enyim.com>")) //MemcacheD section
                            {
                                memCacheTags = false;
                            }
                            if (memCacheTags && ostring.Contains("<add address"))
                            {
                                while (ostring.Substring(ostring.Length - 2, 2) != "/>")
                                {
                                    string readString = streamReader.ReadLine().Trim();
                                    ostring = ostring + readString;
                                }
                                var addrMatch = Regex.Matches(ostring, "address\\s*=\\s*\"([^\"]*)\"", RegexOptions.IgnoreCase); //address string plus the first thing in quotes after it
                                var portMatch = Regex.Matches(ostring, "port\\s*=\\s*\"([^\"]*)\"", RegexOptions.IgnoreCase); //port plus the first thing in quotes after it

                                var addrValues = GetStringFromMatch(addrMatch, "address");
                                var portValues = GetStringFromMatch(portMatch, "port");

                                if (addrValues.Count > 0 && portValues.Count > 0)
                                {
                                    var server = new ExpandoObject() as IDictionary<string, Object>;
                                    server.Add("Address", addrValues["address"]);
                                    server.Add("Port", portValues["port"]);
                                    if (serverList != null)
                                    {
                                        serverList.Add(server);
                                        enyimMemcached.Add("Servers", serverList);
                                    }
;
                                }

                            }
                            if (memCacheTags && ostring.Contains("<socketPool")) //other MemcacheD settings
                            {


                                var minPoolSizeMatch = Regex.Matches(ostring, "minPoolSize\\s*=\\s*\"([^\"]*)\"", RegexOptions.IgnoreCase); //minPoolSize string plus the first thing in quotes after it
                                var maxPoolSizeMatch = Regex.Matches(ostring, "maxPoolSize\\s*=\\s*\"([^\"]*)\"", RegexOptions.IgnoreCase); //maxPoolSize plus the first thing in quotes after it
                                var connectionTimeoutMatch = Regex.Matches(ostring, "connectionTimeout\\s*=\\s*\"([^\"]*)\"", RegexOptions.IgnoreCase); //connectionTimeout string plus the first thing in quotes after it
                                var deadTimeoutMatch = Regex.Matches(ostring, "deadTimeout\\s*=\\s*\"([^\"]*)\"", RegexOptions.IgnoreCase); //deadTimeout plus the first thing in quotes after it
                                var queueTimeoutMatch = Regex.Matches(ostring, "queueTimeout\\s*=\\s*\"([^\"]*)\"", RegexOptions.IgnoreCase); //queueTimeout string plus the first thing in quotes after it
                                var receiveTimeoutMatch = Regex.Matches(ostring, "receiveTimeout\\s*=\\s*\"([^\"]*)\"", RegexOptions.IgnoreCase); //receiveTimeout plus the first thing in quotes after it

                                var minPoolSize = GetStringFromMatch(minPoolSizeMatch, "minPoolSize");
                                var maxPoolSize = GetStringFromMatch(maxPoolSizeMatch, "maxPoolSize");
                                var connectionTimeout = GetStringFromMatch(connectionTimeoutMatch, "connectionTimeout");
                                var deadTimeout = GetStringFromMatch(deadTimeoutMatch, "deadTimeout");
                                var queueTimeout = GetStringFromMatch(queueTimeoutMatch, "queueTimeout");
                                var receiveTimeout = GetStringFromMatch(receiveTimeoutMatch, "receiveTimeout");


                                var socketPool = new ExpandoObject() as IDictionary<string, Object>;

                                if (minPoolSize.Count > 0)
                                    socketPool.Add("minPoolSize", minPoolSize["minPoolSize"]);

                                if (maxPoolSize.Count > 0)
                                    socketPool.Add("maxPoolSize", maxPoolSize["maxPoolSize"]);

                                if (connectionTimeout.Count > 0)
                                    socketPool.Add("connectionTimeout", connectionTimeout["connectionTimeout"]);

                                if (deadTimeout.Count > 0)
                                    socketPool.Add("deadTimeout", deadTimeout["deadTimeout"]);

                                if (queueTimeout.Count > 0)
                                    socketPool.Add("queueTimeout", queueTimeout["queueTimeout"]);

                                if (receiveTimeout.Count > 0)
                                    socketPool.Add("receiveTimeout", receiveTimeout["receiveTimeout"]);

                                if (socketPool.Count > 0)
                                {
                                    enyimMemcached.Add("socketPool", socketPool);
                                }
                            }

                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Error converting appsettings: " + e.ToString());
                throw e;
            }

            IDictionary<string, object> appConfigs = (IDictionary<string, object>)dJson["APP_Config"];

            IDictionary<string, object> apiConfigs = (IDictionary<string, object>)dJson["API_Config"];


            dJson["APP_Config"] = appConfigs;
            dJson["API_Config"] = apiConfigs;
            dJson["ConnectionStrings"] = dbDefault;
            dJson["enyimMemcached"] = enyimMemcached;
            dJson["CacheSettings"] = GetCacheSettings();

            //GetLanguageSettings(dJson);

            string astring = JsonConvert.SerializeObject(dJson, Formatting.Indented);
            return astring;
        }

        private static IDictionary<string, object> GetLanguageSettings(IDictionary<string, object> dJson)
        {
            IDictionary<string, object> languageResource = (Dictionary<string, object>)dJson["LanguageResource"];
            IDictionary<string, object> languages = (IDictionary<string, object>)languageResource["languages"];
            foreach (var language in languages)
            {
                IDictionary<string, object> langConfig = (Dictionary<string, object>)language.Value;
                string langName = (string)langConfig["NAME"];
                string langLocation = (string)langConfig["PLUGIN_LOCATION"];
            }
            return languages;
        }

        private static IDictionary<string, object> GetCacheSettings()
        {
            var cacheSettings = new ExpandoObject() as IDictionary<string, Object>;
            Console.WriteLine("Please enter a suitable MemcacheD salsa for this environment:");
            string salsa = Console.ReadLine();
            cacheSettings.Add(new KeyValuePair<string, Object>("API_MEMCACHED_SALSA", salsa));
            cacheSettings.Add(new KeyValuePair<string, Object>("API_MEMCACHED_MAX_VALIDITY", "2592000"));
            cacheSettings.Add(new KeyValuePair<string, Object>("API_MEMCACHED_MAX_SIZE", "128"));
            cacheSettings.Add(new KeyValuePair<string, Object>("API_MEMCACHED_ENABLED", true));
            cacheSettings.Add(new KeyValuePair<string, Object>("API_CACHE_TRACE_ENABLED", false));
            return cacheSettings;
        }

        internal static string GetLog4NetConfigFromWebConfig(string webConfig)
        {
            try
            {
                bool log4NetTags = false;
                StringBuilder sb = new StringBuilder();
                using (var streamReader = new StreamReader(webConfig, Encoding.UTF8))
                {
                    while (!streamReader.EndOfStream)
                    {
                        string ostring = streamReader.ReadLine() ?? string.Empty;
                        ostring = ostring.Trim();
                        if (ostring.Length >= 9)
                        {
                            if (ostring.Contains("<log4net>"))
                            {
                                log4NetTags = true;
                                sb.Append(ostring + Environment.NewLine);
                                do
                                {

                                    ostring = streamReader.ReadLine() ?? string.Empty;
                                    if (ostring.Contains("<connectionType"))
                                        ostring = "<connectionType value=\"Microsoft.Data.SqlClient.SqlConnection,Microsoft.Data.SqlClient,Version=1.0.0.0,Culture=neutral,PublicKeyToken=23ec7fc2d6eaa4a5\"/>";
                                    sb.Append(ostring + Environment.NewLine);
                                } while (!ostring.Contains("</log4net>"));
                            }
                        }
                    }
                }
                return sb.ToString();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading web config: " + ex.Message);
                throw ex;
            }

        }

        internal static string GetIisWebConfigFromWebConfig(string webConfig)
        {
            bool stopWriting = false;
            try
            {
                bool log4NetTags = false;
                StringBuilder sb = new StringBuilder();
                using (var streamReader = new StreamReader(webConfig, Encoding.UTF8))
                {

                    while (!streamReader.EndOfStream)
                    {
                        string ostring = streamReader.ReadLine() ?? string.Empty;
                        ostring = ostring.Trim();
                        if (ostring.Length >= 9)
                        {
                            if (ostring.Contains("<system.webServer>"))
                            {

                                log4NetTags = true;
                                if (!stopWriting)
                                    sb.Append(ostring + Environment.NewLine);
                                sb.Append("<aspNetCore processPath=\"dotnet\" arguments=\".\\PxStat.dll\" stdoutLogEnabled=\"false\" stdoutLogFile=\".\\logs\\stdout\" hostingModel=\"inprocess\" />" + Environment.NewLine);
                                do
                                {
                                    ostring = streamReader.ReadLine() ?? string.Empty;
                                    if (ostring.Contains("<handlers>"))
                                    {
                                        sb.Append(GetHandlerString());
                                        stopWriting = true;
                                    }
                                    if (!stopWriting)
                                        sb.Append(ostring + Environment.NewLine);
                                    if (ostring.Contains("</handlers>")) stopWriting = false;

                                } while (!ostring.Contains("</system.webServer>"));
                            }
                            if (ostring.Contains("<system.web>"))
                            {

                                if (!stopWriting)
                                    sb.Append(ostring + Environment.NewLine);
                                do
                                {
                                    ostring = streamReader.ReadLine() ?? string.Empty;
                                    if (ostring.Contains("<sessionState"))
                                        sb.Append(ostring + Environment.NewLine);

                                } while (!ostring.Contains("</system.web>"));
                                sb.Append(ostring + Environment.NewLine);

                            }

                        }
                    }
                }
                return "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine + "<configuration>" +
                   Environment.NewLine + sb.ToString() + Environment.NewLine + "</configuration>";

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading web config: " + ex.Message);
                throw ex;
            }

        }

        private static string GetHandlerString()
        {
            string hstring = Environment.NewLine + "<handlers>" + Environment.NewLine;
            hstring = hstring + "<!-- Add the handler to instruct IIS to serve the JSON RPC webservice requests -->" + Environment.NewLine;
            hstring = hstring + "<add name=\"aspNetCore\" path=\"*\" verb=\"*\" modules=\"AspNetCoreModuleV2\" resourceType=\"Unspecified\" />" + Environment.NewLine;
            hstring = hstring + "</handlers>" + Environment.NewLine;
            return hstring;
        }

        private static string GetRules()
        {
            string rstring = Environment.NewLine + "<rules>" + Environment.NewLine;
            rstring = rstring + "" + Environment.NewLine;
            rstring = rstring + "<!-- Rule to allow api.jsonrpc URLs -->" + Environment.NewLine;
            rstring = rstring + "<rule name=\"AllowApiJsonRpcUrls\" stopProcessing=\"true\">" + Environment.NewLine;
            rstring = rstring + "<match url=\"^api\\.jsonrpc(/.*)?$\" />" + Environment.NewLine;
            rstring = rstring + "<action type=\"None\" />" + Environment.NewLine;
            rstring = rstring + "</rule>" + Environment.NewLine;
            rstring = rstring + "<!-- Rule to allow api.restful URLs -->" + Environment.NewLine;
            rstring = rstring + "<rule name=\"AllowApiRestfulUrls\" stopProcessing=\"true\">" + Environment.NewLine;
            rstring = rstring + "<match url=\"^api\\.restful(/.*)?$\" />" + Environment.NewLine;
            rstring = rstring + "<action type=\"None\" />" + Environment.NewLine;
            rstring = rstring + "</rule>" + Environment.NewLine;
            rstring = rstring + "<!-- Rule to allow api.static URLs -->" + Environment.NewLine;
            rstring = rstring + "<rule name=\"AllowApiStaticUrls\" stopProcessing=\"true\">" + Environment.NewLine;
            rstring = rstring + "<match url=\"^api\\.static(/.*)?$\" />" + Environment.NewLine;
            rstring = rstring + "<action type=\"None\" />" + Environment.NewLine;
            rstring = rstring + "</rule>" + Environment.NewLine;
            rstring = rstring + "<rule name=\"Return404ForOtherRequests\" stopProcessing=\"true\">" + Environment.NewLine;
            rstring = rstring + "<match url=\".*\" />" + Environment.NewLine;
            rstring = rstring + "<conditions>" + Environment.NewLine;
            rstring = rstring + "<add input=\"{REQUEST_URI}\" pattern=\"^/(api\\.jsonrpc|api\\.restful|api\\.static)/\" negate=\"true\" />" + Environment.NewLine;
            rstring = rstring + "</conditions>" + Environment.NewLine;
            rstring = rstring + "<action type=\"CustomResponse\" statusCode=\"404\" statusReason=\"Not Found\" statusDescription=\"The requested resource was not found.\" />" + Environment.NewLine;
            rstring = rstring + "</rule>" + Environment.NewLine;
            rstring = rstring + "</rules>" + Environment.NewLine;
            return rstring;

        }

        internal static string GetFirebaseKeyFromFile(string filePath)
        {
            return GetFileString(filePath);
        }

        private static Dictionary<string, string> GetStringFromMatch(MatchCollection matches, string key)
        {
            Dictionary<string, string> result = new();
            if (matches.Count == 0) return result;
            string rawHit = matches[0].Value.ToString().Trim();

            var nextMatch = Regex.Matches(rawHit, "\"([^\"]*)\"");
            if (nextMatch != null)
            {
                if (nextMatch.Count > 0)
                    result.Add(key, nextMatch[0].Value.ToString().Trim().Replace("\"", ""));
            }



            return result;
        }

        internal static decimal GetLatestVersion(string connectionString, string configType)
        {
            decimal version = 0;
            try
            {
                using (SqlConnection openCon = new SqlConnection(connectionString))
                {
                    // First get the latest config version
                    string readLatestCommand = "App_Settings_Read_Version";
                    openCon.Open();
                    int cstId = 0;
                    switch (configType.ToUpper())
                    {
                        case "API":
                            cstId = 1;
                            break;
                        case "APP":
                            cstId = 2;
                            break;
                        default:
                            cstId = -1;
                            break;
                    }
                    if (cstId <= 0) return 0;
                    using (SqlCommand queryReadLatest = new SqlCommand(readLatestCommand))
                    {
                        queryReadLatest.CommandType = CommandType.StoredProcedure;
                        queryReadLatest.Connection = openCon;
                        queryReadLatest.Parameters.Add("@cst_id", SqlDbType.Int).Value = cstId;

                        SqlDataReader dr = queryReadLatest.ExecuteReader();
                        while (dr.Read())
                        {
                            version = (decimal)dr[0];
                        }

                        dr.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting latest version: " + ex.Message);
                throw ex;
            }
            return version;
        }

        internal static void WriteToFile(string filePath, string data)
        {
            try
            {
                StreamWriter sw = new StreamWriter(filePath);
                sw.Write(data);
                sw.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing to " + filePath + ": " + ex.Message);
                throw ex;
            }
        }

        internal static void RunApiUpgradeJsonScript(string script, string connectionString, decimal version, string ccnUsername)
        {
            if (!string.IsNullOrEmpty(script))
            {
                DataSet ds = JsonConvert.DeserializeObject<DataSet>(script);
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {


                            string? key = dr["key"] != DBNull.Value ? (string)dr["key"] : null;
                            string? value = dr["value"] != DBNull.Value ? (string)dr["value"] : null;
                            string? action = dr["action"] != DBNull.Value ? (string)dr["action"] : null;
                            bool? sensitive = dr["sensitive"] != DBNull.Value ? (bool)dr["sensitive"] : null;
                            string? description = dr["description"] != DBNull.Value ? (string)dr["description"] : null;
                            if (key != null) Console.WriteLine("Updating key: " + key);
                            if (action != null)
                            {
                                switch (action.ToLower())
                                {
                                    case "append":
                                        RunApiAppend(version, connectionString, key, value, ccnUsername, description, sensitive);
                                        break;
                                    case "updatekey":
                                        RunApiUpdateKey(version, connectionString, key, value);
                                        break;
                                    case "updatevalue":
                                        RunApiUpdateValue(version, connectionString, key, value, description, sensitive);
                                        break;
                                    case "delete":
                                        RunApiDelete(version, connectionString, key);
                                        break;
                                    case "flag":
                                        RunApiAlterFlag(version, connectionString, key, sensitive);
                                        break;
                                    default:
                                        Console.Write("Invalid action in migration json file: " + action);
                                        break;

                                }
                            }

                        }
                    }
                }
            }
        }

        internal static void RunApiAppend(decimal version, string connectionString, string key, string value, string ccnUsername, string description = "", bool? sensitive = false)
        {
            if (key == null)
            {
                Console.WriteLine("Null key value");
                return;
            }
            if (value == null)
            {
                Console.WriteLine("Null value");
                return;
            }

            try
            {
                string writeSp = "Api_Settings_Migrate_Write";
                using (SqlConnection openCon = new SqlConnection(connectionString))
                {
                    openCon.Open();
                    using (SqlCommand qryWriteApi = new SqlCommand(writeSp))
                    {
                        qryWriteApi.CommandType = CommandType.StoredProcedure;
                        qryWriteApi.Connection = openCon;

                        qryWriteApi.Parameters.Add("@apiversion", SqlDbType.Decimal).Value = version;
                        qryWriteApi.Parameters.Add("@apikey", SqlDbType.NVarChar).Value = key;
                        qryWriteApi.Parameters.Add("@apivalue", SqlDbType.NVarChar).Value = value;
                        qryWriteApi.Parameters.Add("@apidescription", SqlDbType.NVarChar).Value = description;
                        qryWriteApi.Parameters.Add("@apisensitivevalue", SqlDbType.Bit).Value = sensitive == null ? false : sensitive;
                        qryWriteApi.Parameters.Add("@userName", SqlDbType.NVarChar).Value = ccnUsername;

                        qryWriteApi.ExecuteNonQuery();
                        openCon.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can't append record for key " + key + " to the database. " + ex.Message);
                throw ex;
            }

        }

        internal static void RunApiUpdateKey(decimal version, string connectionString, string key, string newKey)
        {
            if (key == null)
            {
                Console.WriteLine("Null key value");
                return;
            }
            if (newKey == null)
            {
                Console.WriteLine("Null new key value");
                return;
            }

            try
            {
                string writeSp = "Api_Settings_Update";
                using (SqlConnection openCon = new SqlConnection(connectionString))
                {
                    openCon.Open();
                    using (SqlCommand qryWriteApi = new SqlCommand(writeSp))
                    {
                        qryWriteApi.CommandType = CommandType.StoredProcedure;
                        qryWriteApi.Connection = openCon;

                        qryWriteApi.Parameters.Add("@apiversion", SqlDbType.Decimal).Value = version;
                        qryWriteApi.Parameters.Add("@apikey", SqlDbType.NVarChar).Value = key;
                        qryWriteApi.Parameters.Add("@apikeyNew", SqlDbType.NVarChar).Value = newKey;

                        qryWriteApi.ExecuteNonQuery();
                        openCon.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can't update key " + key + ". " + ex.Message);
                throw ex;
            }
        }

        internal static void RunApiUpdateValue(decimal version, string connectionString, string key, string value, string description, bool? sensitive)
        {
            if (key == null)
            {
                Console.WriteLine("Null key value");
                return;
            }
            if (value == null)
            {
                Console.WriteLine("Null value");
                return;
            }

            try
            {

                string writeSp = "Api_Settings_Update";
                using (SqlConnection openCon = new SqlConnection(connectionString))
                {
                    openCon.Open();
                    using (SqlCommand qryWriteApi = new SqlCommand(writeSp))
                    {
                        qryWriteApi.CommandType = CommandType.StoredProcedure;
                        qryWriteApi.Connection = openCon;

                        qryWriteApi.Parameters.Add("@apiversion", SqlDbType.Decimal).Value = version;
                        qryWriteApi.Parameters.Add("@apikey", SqlDbType.NVarChar).Value = key;
                        if (value != null)
                            qryWriteApi.Parameters.Add("@apivalue", SqlDbType.NVarChar).Value = value;
                        if (description != null)
                            qryWriteApi.Parameters.Add("@apidescription", SqlDbType.NVarChar).Value = description;
                        if (sensitive != null)
                            qryWriteApi.Parameters.Add("@apisensitivevalue", SqlDbType.Bit).Value = sensitive == null ? false : sensitive;

                        qryWriteApi.ExecuteNonQuery();
                        openCon.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can't update API value for key " + key + ". " + ex.Message);
                throw ex;
            }
        }

        internal static void RunApiAlterFlag(decimal version, string connectionString, string key, bool? sensitive)
        {
            if (key == null)
            {
                Console.WriteLine("Null key value");
                return;
            }
            if (sensitive == null)
            {
                Console.WriteLine("Null sensitive value");
                return;
            }
            try
            {
                string writeSp = "Api_Settings_Update";
                using (SqlConnection openCon = new SqlConnection(connectionString))
                {
                    openCon.Open();
                    using (SqlCommand qryWriteApi = new SqlCommand(writeSp))
                    {
                        qryWriteApi.CommandType = CommandType.StoredProcedure;
                        qryWriteApi.Connection = openCon;

                        qryWriteApi.Parameters.Add("@apiversion", SqlDbType.Decimal).Value = version;
                        qryWriteApi.Parameters.Add("@apikey", SqlDbType.NVarChar).Value = key;

                        if (sensitive != null)
                            qryWriteApi.Parameters.Add("@apisensitivevalue", SqlDbType.Bit).Value = sensitive == null ? false : sensitive;

                        qryWriteApi.ExecuteNonQuery();
                        openCon.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can't update flag for key " + key + ". " + ex.Message);
                throw ex;
            }
        }

        internal static void RunApiDelete(decimal version, string connectionString, string key)
        {
            if (key == null)
            {
                Console.WriteLine("Null key value");
                return;
            }
            try
            {
                string writeSp = "Api_Settings_Delete";
                using (SqlConnection openCon = new SqlConnection(connectionString))
                {
                    openCon.Open();
                    using (SqlCommand qryWriteApi = new SqlCommand(writeSp))
                    {
                        qryWriteApi.CommandType = CommandType.StoredProcedure;
                        qryWriteApi.Connection = openCon;

                        qryWriteApi.Parameters.Add("@apiversion", SqlDbType.Decimal).Value = version;
                        qryWriteApi.Parameters.Add("@apikey", SqlDbType.NVarChar).Value = key;

                        qryWriteApi.ExecuteNonQuery();
                        openCon.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can't delete API setting for key " + key + ". " + ex.Message);
                throw ex;
            }
        }
    }


}
