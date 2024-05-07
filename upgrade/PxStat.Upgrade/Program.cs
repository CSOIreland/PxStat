/*
 This app is used to (a) validate any json based APP configs for the upgrade of PxStat to version 6.0.0 and higher and,
(b) insert those validations (if valid) to the TS_APP_SETTINGS table
You will have to supply database login details, as well as paths to the config and corresponding schema files
 
 */

/*
 For NISRA change - check the version of the SQL Server client on log4net config.
Also check language.json file and location
Also check that db connection is sql and not AD (or not?)
 
 */

using PxStatMigrateAppConfig;
using System.Web;

string dbName;
string server;
string userName;
string password;
string ccnUsername;
Dictionary<string, string> configs = new();
Dictionary<string, string> schemas = new();
string connectionString;
string webConfigPath = null;
string upgradeFrom = null;
string baseFolder = null;
bool frontierChange = false; //To indicate to several functions if this a change from pre 6.0.0 to 6.0.0 or later
string startVersion = null;

//Get a pxstat username
Console.WriteLine("Please supply a valid PxStat admin username. Your app config changes will be shown against this name");
ccnUsername = Console.ReadLine();

//Get database and login details
Console.WriteLine("Please enter the database details:");
Console.WriteLine("");
Console.WriteLine("Please enter the database server name or ip address");
server = Console.ReadLine();
if (string.IsNullOrEmpty(server))
{
    Console.WriteLine("Invalid server name, closing application");
    System.Environment.Exit(0);
}
Console.WriteLine("Please enter the database name");
dbName = Console.ReadLine();
if (string.IsNullOrEmpty(dbName))
{
    Console.WriteLine("Invalid database name, closing application");
    System.Environment.Exit(0);
}
Console.WriteLine("Please enter a database user name (with at least dbo privilege). If your are using a Windows account, just press Enter to skip this.");
userName = Console.ReadLine();
if (!string.IsNullOrEmpty(userName))
{
    Console.WriteLine("Please enter your password");
    password = Helper.InputConfidential();
}
else password = null;


//Create the connection string
if (!string.IsNullOrEmpty(password))
    connectionString = "Server=" + server + ";Initial Catalog=" + dbName + ";User ID=" + userName + ";Password=" + password + ";Persist Security Info=False;Column Encryption Setting=enabled;Trust Server Certificate=true";
else //Integrated Security
    connectionString = "Server=" + server + ";Initial Catalog=" + dbName + ";Integrated Security=SSPI;Persist Security Info=False;Column Encryption Setting=enabled;Trust Server Certificate=true";
Console.WriteLine();

//Run the upgrade ddl
Console.WriteLine("Please enter the path to your target version database folder, e.g \"C:\\Development\\6.0.0\\db\"");
baseFolder = Console.ReadLine();
if (String.IsNullOrEmpty(baseFolder))
{
    Console.WriteLine("Invalid folder name, closing application");
    System.Environment.Exit(0);
}

else if (!baseFolder.Substring(baseFolder.Length - 1, 1).Equals("\\"))
    baseFolder = baseFolder + "\\";

Dictionary<string, string> replacements = new();
replacements.Add("$(DB_DATA)", dbName);


//Run the all upgrade scripts older than the one you're migrating from in succession
Console.WriteLine("Please enter the version of pxstat from which you are upgrading");
startVersion = Console.ReadLine();
upgradeFrom = startVersion;
if (!upgradeFrom.Substring(upgradeFrom.Length - 4, 4).Equals(".sql"))
    upgradeFrom = upgradeFrom + ".sql";
List<string> upgradeScripts = Helper.GetUpgradeList(baseFolder + "\\" + "scripts", upgradeFrom);

if (upgradeFrom.CompareTo("6.0.0") < 0) frontierChange = true;
Console.WriteLine("Frontier change: " + frontierChange);


Console.WriteLine("Do you wish to run a full database upgrade? y/n");
if (Console.ReadLine().ToUpper() == "Y")
{





    foreach (string script in upgradeScripts)
    {
        string sString = Helper.GetFileString(script, replacements);
        if (Helper.ExecuteSql(connectionString, sString))
            Console.WriteLine(script + " run ok");
        else
        {
            Console.WriteLine(script + " error!");
            return;
        }
    }

    Console.WriteLine();

    //Do de drops
    string fDrops = baseFolder + "Drop";
    List<string> drops = Helper.GetAllFiles(fDrops);
    foreach (string d in drops)
    {
        string dstring = Helper.GetFileString(d, replacements);
        if (Helper.ExecuteSql(connectionString, dstring))
            Console.WriteLine(d + " run ok");
    }


    string fTypes = baseFolder + "Types";
    List<string> types = Helper.GetAllFiles(fTypes);
    foreach (var type in types)
    {
        string tstring = Helper.GetFileString(type, replacements);
        if (Helper.ExecuteSql(connectionString, tstring))
            Console.WriteLine(type + " run ok");
    }

    string fViews = baseFolder + "Views";
    List<string> views = Helper.GetAllFiles(fViews);
    foreach (var view in views)
    {
        string vstring = Helper.GetFileString(view, replacements);
        if (Helper.ExecuteSql(connectionString, vstring))
            Console.WriteLine(view + " run ok");
    }

    string fStoredProcs = baseFolder + "StoredProcedures";
    List<string> storedProcs = Helper.GetAllFiles(fStoredProcs);
    foreach (var sp in storedProcs)
    {
        string spstring = Helper.GetFileString(sp, replacements);
        if (Helper.ExecuteSql(connectionString, spstring))
            Console.WriteLine(sp + " run ok");
    }
    Console.WriteLine();
}

if (frontierChange)
{

    Console.WriteLine();
    Console.WriteLine("Do you wish to create an API config from the web config? y/n");
    if (Console.ReadLine().ToUpper().Equals("Y"))
    {
        Console.WriteLine("Please enter the path of the web.config file from which you wish to read, e.g. C:\\PxStat\\web.config");
        Console.WriteLine("This will be the web.config of the version you are migrating FROM.");
        webConfigPath = Console.ReadLine();
        if (webConfigPath != null)
        {
            Dictionary<string, string> webConfigs = Helper.GetWebConfig(webConfigPath);
            foreach (var webConfig in webConfigs)
            {
                if (webConfig.Value.Contains(';'))
                    Helper.WriteApiConfigToDatabase(connectionString, webConfig.Key, HttpUtility.HtmlDecode(webConfig.Value), ccnUsername);
                else
                    Helper.WriteApiConfigToDatabase(connectionString, webConfig.Key, webConfig.Value, ccnUsername);
                Console.WriteLine(webConfig.Key + ":" + webConfig.Value);
            }
        }
    }



    Console.WriteLine();
    Console.WriteLine("Do you wish to create a Firebase Key API config entry from a Firebase.json file? y/n");
    if (Console.ReadLine().ToUpper().Equals("Y"))
    {
        Console.WriteLine("Please enter the full path of the Firebase.json file");
        string firebaseFileName = Console.ReadLine();
        if (firebaseFileName != null)
        {
            string firebaseToken = Helper.GetFileString(firebaseFileName);
            KeyValuePair<string, string> firebaseConfig = new KeyValuePair<string, string>("API_FIREBASE_CREDENTIAL", firebaseToken);

            Helper.WriteApiConfigToDatabase(connectionString, firebaseConfig.Key, firebaseConfig.Value, ccnUsername);
            Console.WriteLine(firebaseConfig.Key + ":" + firebaseConfig.Value);

        }

    }


   

    Console.WriteLine();
    Console.WriteLine("Do you wish to create an APP config on the database? y/n");
    if (Console.ReadLine().ToUpper().Equals("Y"))
    {

        //Get details of the config and schema files
        Console.WriteLine("Getting config details. When you have finished entering configurations, just press enter and the application" +
            " will continue to the next step. You may need to create an alternative (up to date) version of the config file(s) first. This is definitely necessary for server.config.");
        Console.WriteLine();
        string jsonFile = null;
        string schemaFile = null;
        string config = null;

        do
        {
            Console.WriteLine();
            Console.WriteLine("Please enter the name of a config (e.g. config.global.json)");
            config = Console.ReadLine();
            if (string.IsNullOrEmpty(config)) break;
            Console.WriteLine("Please enter the full or relative path and name of the json config file for " + config);
            string jsonFilePath = Console.ReadLine();
            if (string.IsNullOrEmpty(jsonFilePath)) break;
            jsonFile = Helper.GetFileString(jsonFilePath);
            if (jsonFile == null)
            {
                Console.WriteLine("File not found " + jsonFilePath);
                continue;
            }
            Console.WriteLine("Please enter the full or relative path and name of the schema file for " + config);
            string schemaFilePath = Console.ReadLine();
            if (string.IsNullOrEmpty(schemaFilePath)) break;

            schemaFile = Helper.GetFileString(schemaFilePath);
            if (schemaFile == null)
            {
                Console.WriteLine("File not found " + schemaFilePath);
                continue;
            }

            if (!Helper.ValidateJson(jsonFile, schemaFile))
            {
                Console.WriteLine("Validation error found for " + config + " end of process. Please attempt to input the details again, or else just press 'enter' to end this process.");

                continue;
            }

            if (!String.IsNullOrEmpty(jsonFile) && !String.IsNullOrEmpty(schemaFile) && !String.IsNullOrEmpty(config) && !configs.ContainsKey(config))
                configs.Add(config, jsonFile);

        } while (true);



        Console.WriteLine("Writing APP configuration to the database");
        //Writing the configurations
        foreach (var item in configs)
        {
            Helper.WriteAppConfigToDatabase(connectionString, item.Key, item.Value, ccnUsername);
        }

    }


    if (frontierChange)
    {

        Console.WriteLine();
        Console.WriteLine("Do you wish to update the appsettings.json file from the web.config? y/n");
        if (Console.ReadLine().ToUpper().Equals("Y"))
        {
            if (webConfigPath == null)
            {
                Console.WriteLine("Please enter the web.config path and filename");
                webConfigPath = Console.ReadLine();

            }
            if (webConfigPath != null)
            {
                Console.WriteLine("Please enter the appsettings.json path and filename. It is assumed that the appsettings.json file exists already");
                string appsettingsFile = Console.ReadLine();
                if (appsettingsFile != null)
                {

                    string appConfig = Helper.GetUpdatedAppConfig(appsettingsFile, webConfigPath);
                    if (appConfig != null)
                    {
                        Helper.WriteToFile(appsettingsFile, appConfig);
                    }

                    else
                    {
                        Console.WriteLine("Unable to create appsettings.json file. You must do this manually before deployment");
                    }
                }

            }


        }

        Console.WriteLine();
        Console.WriteLine("Do you wish to create the log4Net.config from the web.config? y/n");
        if (Console.ReadLine().ToUpper().Equals("Y"))
        {

            if (webConfigPath == null)
            {
                Console.WriteLine("Please enter the web.config path and filename. This file will not exist already, but please provide the full path and filename");
                webConfigPath = Console.ReadLine();

            }

            if (webConfigPath != null)
            {
                Console.WriteLine("Please enter the log4Net.config path and filename");
                string log4NetFilePath = Console.ReadLine();
                if (log4NetFilePath != null)
                {
                    string log4netConfig = Helper.GetLog4NetConfigFromWebConfig(webConfigPath);
                    if (log4netConfig != null)
                        Helper.WriteToFile(log4NetFilePath, log4netConfig);
                }
            }


        }

        Console.WriteLine();
        Console.WriteLine("Do you wish to create an IIS web.config file from the web.config? y/n");
        if (Console.ReadLine().ToUpper().Equals("Y"))
        {

            if (webConfigPath == null)
            {
                Console.WriteLine("Please enter the original web.config path and filename");
                webConfigPath = Console.ReadLine();

            }

            if (webConfigPath != null)
            {
                Console.WriteLine("Please enter the new IIS web.config path and filename");
                string webConfigIisPath = Console.ReadLine();
                if (webConfigIisPath != null)
                {
                    string iisWebConfig = Helper.GetIisWebConfigFromWebConfig(webConfigPath);
                    if (iisWebConfig != null)
                        Helper.WriteToFile(webConfigIisPath, iisWebConfig);
                }
            }


        }

    }
} // End of FrontierChange if statement

Console.WriteLine();
Console.WriteLine("Do you wish to run all of the json config upgrade scripts after the chosen version to the current version of the API config? y/n");
if (Console.ReadLine().ToUpper().Equals("Y"))
{
    if (baseFolder == null)
    {
        Console.WriteLine("Please enter the path to your database folder, e.g \"C:\\Development\\6.0.0\\db\"");
        baseFolder = Console.ReadLine();
    }
    if (!string.IsNullOrEmpty(baseFolder))
    {
        if (baseFolder.Length > 1)
        {
            if (!baseFolder.Substring(baseFolder.Length - 1, 1).Equals("\\"))
                baseFolder = baseFolder + "\\";

        }

    }

    if (baseFolder != null)
    {
        decimal version = Helper.GetLatestVersion(connectionString, 1);
        if (startVersion == null)
        {
            //Run the all upgrade scripts older than the one you're migrating from in succession
            Console.WriteLine("Please enter the version of pxstat from which you are upgrading");
            startVersion = Console.ReadLine();
            upgradeFrom = startVersion;
            if (!upgradeFrom.Substring(upgradeFrom.Length - 4, 4).Equals(".json"))
                upgradeFrom = upgradeFrom + ".json";

        }

        List<string> apiScripts = Helper.GetUpgradeList(baseFolder + "Config\\Api", upgradeFrom);
        foreach (var script in apiScripts)
        {
            string scriptString = Helper.GetFileString(script);
            Helper.RunApiUpgradeJsonScript(scriptString, connectionString, version, ccnUsername);
        }

    }



}

//All done
Console.WriteLine("Configuration complete, press any key to exit");
Console.Read();
