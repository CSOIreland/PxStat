
/*******************************************************************************
Application - Constant
*******************************************************************************/

// Application Version
const C_APP_VERSION = "8.0.0";

// Master language
const C_APP_MASTER_LANGUAGE = "en";

// URLs GitHub
const C_APP_URL_GITHUB = "https://github.com/CSOIreland/PxStat";
const C_APP_URL_GITHUB_EUG_DATA_PIVOT = "https://github.com/CSOIreland/PxStat/wiki/EUG-Data#pivoting";
const C_APP_URL_GITHUB_REPORT_ISSUE = "https://github.com/CSOIreland/PxStat/issues/new/choose";
const C_APP_URL_GITHUB_API_CUBE_JSONRPC = "https://github.com/CSOIreland/PxStat/wiki/API-Cube";
const C_APP_URL_GITHUB_API_CUBE_RESFFUL = "https://github.com/CSOIreland/PxStat/wiki/API-Cube-RESTful";
const C_APP_URL_GITHUB_API_CUBE_PXAPIV1 = "https://github.com/CSOIreland/PxStat/wiki/API-Cube-PxAPIv1";

// URLs Configuration
const C_APP_CONFIG_CLIENT = C_APP_URL_API + "public/api.restful/PxStat.Config.Config_API.ReadApp/config.client.json";
const C_APP_CONFIG_GLOBAL = C_APP_URL_API + "public/api.restful/PxStat.Config.Config_API.ReadApp/config.global.json";

// Public API
const C_APP_API_JSONRPC_VERSION = "2.0";
const C_APP_API_GET_PARAMATER_IDENTIFIER = "?data=";
const C_APP_API_RESTFUL_READ_DATASET_URL = "{0}PxStat.Data.Cube_API.ReadDataset/{1}/{2}/{3}/{4}";
const C_APP_API_RESTFUL_READ_COLLECTION_URL = "{0}PxStat.Data.Cube_API.ReadCollection/{1}/{2}";

// JSON-stat QUERY
const C_APP_JSONSTAT_QUERY_CLASS = "query"
const C_APP_JSONSTAT_QUERY_VERSION = "2.0";

// PxAPIv1
const C_APP_PXAPIV1_JSONSTAT_1X = "json-stat"
const C_APP_PXAPIV1_JSONSTAT_2X = "json-stat2";

// Names
const C_APP_NAME_LINK_EDIT = "link-edit";
const C_APP_NAME_LINK_DELETE = "link-delete";
const C_APP_NAME_LINK_VIEW = "link-view";
const C_APP_NAME_LINK_GEOJSON = "link-geojson";
const C_APP_NAME_LINK_INTERNAL = "link-internal";
const C_APP_NAME_LINK_ANALYTIC = "link-analytic";

// RegEx
const C_APP_REGEX_EMAIL = /^([\w-\.\+]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|([\w-\.\+]+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
const C_APP_REGEX_IP_MASK = /^\d{1,3}\.\d{1,3}\.\d{1,3}$|^\d{1,3}\.\d{1,3}\.?$|^\d{1,3}\.?$/;
const C_APP_REGEX_IP = /^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$/;
const C_APP_REGEX_BBCODE_NOT_ALLOWED = /\[\/?(img|code|color|quote)\]/gi;
const C_APP_REGEX_ALPHA = /[^a-z]/gi;
const C_APP_REGEX_NOHTML = /<[^>]*>/gi;
const C_APP_REGEX_NOSPAN = /<span.*>.*<\/span>/gi;
const C_APP_REGEX_NUMERIC = /[^0-9]/gi;
const C_APP_REGEX_ALPHANUMERIC = /[^a-z0-9]/gi;
const C_APP_REGEX_ALPHANUMERIC_DIACRITIC = /[^a-záéíóúÁÉÍÓÚ0-9]/gi;
const C_APP_REGEX_NOSPACE = /[\s]/gi;
const C_APP_REGEX_NODOUBLEQUOTE = /[\"]/gi;

// Cookies
const C_COOKIE_LANGUAGE = "language";
const C_COOKIE_CONSENT = "cookie-consent";
const C_COOKIE_LINK_SEARCH = "search";
const C_COOKIE_LINK_PRODUCT = "product";
const C_COOKIE_LINK_COPYRIGHT = "copyright";
const C_COOKIE_LINK_TABLE = "table";
const C_COOKIE_LINK_CHART = "chart";
const C_COOKIE_LINK_MAP = "map";
const C_COOKIE_LINK_RELEASE = "release";

//User Privileges (RQS_CODE)
const C_APP_PRIVILEGE_ADMINISTRATOR = "ADMINISTRATOR";
const C_APP_PRIVILEGE_POWER_USER = "POWER_USER";
const C_APP_PRIVILEGE_MODERATOR = "MODERATOR";

//Firebase auth provider id
const C_APP_FIREBASE_ID_PASSWORD = "password";
const C_APP_FIREBASE_ID_GOOGLE = "google.com";
const C_APP_FIREBASE_ID_FACEBOOK = "facebook.com";
const C_APP_FIREBASE_ID_TWITTER = "twitter.com";
const C_APP_FIREBASE_ID_GITHUB = "github.com";

//Firebase auth error codes
const C_APP_FIREBASE_ERROR_ACCOUNT_EXISTS = "auth/account-exists-with-different-credential";

//Workflow Request (RQS_CODE)
const C_APP_TS_REQUEST_PUBLISH = "PUBLISH";
const C_APP_TS_REQUEST_PROPERTY = "PROPERTY";
const C_APP_TS_REQUEST_DELETE = "DELETE";
const C_APP_TS_REQUEST_ROLLBACK = "ROLLBACK";

//Workflow Response (RSP_CODE)
const C_APP_TS_RESPONSE_APPROVED = "APPROVED";
const C_APP_TS_RESPONSE_REJECTED = "REJECTED";

//Workflow Signoff (SGN_CODE)
const C_APP_TS_SIGNOFF_APPROVED = "APPROVED";
const C_APP_TS_SIGNOFF_REJECTED = "REJECTED";

//Formats (FRM_TYPE)
const C_APP_TS_FORMAT_TYPE_PX = "PX";
const C_APP_TS_FORMAT_TYPE_JSONSTAT = "JSON-stat";
const C_APP_TS_FORMAT_TYPE_CSV = "CSV";
const C_APP_TS_FORMAT_TYPE_XLSX = "XLSX";

//Formats (FRM_VERSION)
const C_APP_TS_FORMAT_VERSION_JSONSTAT_1X = "1.0";
const C_APP_TS_FORMAT_VERSION_JSONSTAT_2X = "2.0";
const C_APP_TS_FORMAT_VERSION_XLSX = "2007";

//Formats (FRM_DIRECTION)
const C_APP_TS_FORMAT_DIRECTION_UPLOAD = "UPLOAD";
const C_APP_TS_FORMAT_DIRECTION_DOWNLOAD = "DOWNLOAD";

//Default formats
const C_APP_FORMAT_TYPE_DEFAULT = C_APP_TS_FORMAT_TYPE_JSONSTAT;
const C_APP_FORMAT_VERSION_DEFAULT = C_APP_TS_FORMAT_VERSION_JSONSTAT_2X;

// File extensions
const C_APP_EXTENSION_PX = "px";
const C_APP_EXTENSION_JSON = "json";
const C_APP_EXTENSION_CSV = "csv";
const C_APP_EXTENSION_HTML = "html";
const C_APP_EXTENSION_XLSX = "xlsx";

// File Mimetype
const C_APP_MIMETYPE_PX = "text/plain";
const C_APP_MIMETYPE_JSON = "application/json";
const C_APP_MIMETYPE_CSV = "text/csv";
const C_APP_MIMETYPE_HTML = "text/html";
const C_APP_MIMETYPE_XLSX = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

// Datatable
const C_APP_DATATABLE_ROW_INDEX = "row-index";
const C_APP_DATATABLE_EXTRA_INFO_LINK = "extra-info-link";

// Upload
const C_APP_UPLOAD_FILE_ALLOWED_EXTENSION = ['.px'];
const C_APP_UPLOAD_FILE_ALLOWED_TYPE = ['', 'text/x-pcaxis', 'text/plain']; // PX file HAS NOT (yet) a registered MIME Type

//Importing maps
const C_APP_UPLOAD_MAP_FILE_ALLOWED_EXTENSION = ['.json', '.geojson'];
const C_APP_UPLOAD_MAP_FILE_ALLOWED_TYPE = ['', 'application/json', 'text/plain']; // PX file HAS NOT (yet) a registered MIME Type

//Create Metadata
const C_APP_CREATE_FILE_ALLOWED_EXTENSION = ['.csv'];
const C_APP_CREATE_FILE_ALLOWED_TYPE = ['', 'text/csv', 'text/plain', "application/vnd.ms-excel"];

// Update Metadata
const C_APP_UPDATEDATASET_FILE_ALLOWED_EXTENSION = ['.px'];
const C_APP_UPDATEDATASET_FILE_ALLOWED_TYPE = ['', 'text/x-pcaxis', 'text/plain']; // PX file HAS NOT (yet) a registered MIME Type
const C_APP_UPDATEDATASET_DATA_FILE_ALLOWED_EXTENSION = ['.csv'];
const C_APP_UPDATEDATASET_DATA_FILE_ALLOWED_TYPE = ['', 'text/csv', 'text/plain', "application/vnd.ms-excel"]; // CSV a registered MIME Type

// Updatedata
const C_APP_UPDATEDATA_DATA_FILE_ALLOWED_EXTENSION = ['.csv'];
const C_APP_UPDATEDATA_DATA_FILE_ALLOWED_TYPE = ['', 'text/csv', 'text/plain', "application/vnd.ms-excel"]; // CSV a registered MIME Type
const C_APP_UPDATEDATA_SOURCE_FILE_ALLOWED_EXTENSION = ['.px'];
const C_APP_UPDATEDATA_SOURCE_FILE_ALLOWED_TYPE = ['', 'text/x-pcaxis', 'text/plain']; // PX file HAS NOT (yet) a registered MIME Type


// Build Update Data Upload
const C_APP_UPDATE_PERIOD_FILE_ALLOWED_EXTENSION = ['.csv'];
const C_APP_UPDATE_PERIOD_FILE_ALLOWED_TYPE = ['', 'text/csv', 'text/plain', "application/vnd.ms-excel"]; // CSV a registered MIME Type

//Build Threshold
const C_APP_CREATE_UPDATE_HARD_THRESHOLD = 10000000;

//CSV structure
const C_APP_CSV_STATISTIC = "STATISTIC";
const C_APP_CSV_CODE = "CODE";
const C_APP_CSV_VALUE = "VALUE";
const C_APP_CSV_UNIT = "UNIT";
const C_APP_CSV_DECIMAL = "DECIMAL";

//Toggle Length
const C_APP_TOGGLE_LENGTH = 100;

//GeoJson
const C_APP_GEOJSON_FEATURE_COLLECTION = "FeatureCollection";

//Datetime 
const C_APP_DATETIME_DEFAULT = "0001-01-01T0:00:00.000Z";

//Search results sort options
const C_APP_SORT_RELEVANCE = "relevance";
const C_APP_SORT_ALPHABETICAL = "alphabetical";
const C_APP_SORT_NEWEST = "newest";
const C_APP_SORT_OLDEST = "oldest";

// Widget
const C_APP_PXWIDGET_TYPE_CHART = 'chart';
const C_APP_PXWIDGET_TYPE_TABLE = 'table';
const C_APP_PXWIDGET_TYPE_MAP = 'map';
const C_APP_PXWIDGET_CHART_TYPES = ["line", "bar", "horizontalBar", "pyramid", "pie", "doughnut", "mixed", "polarArea", "radar"];
const C_APP_PXWIDGET_CHART_TYPES_MIXED = ["line", "bar"];
const C_APP_PXWIDGET_CHART_TYPES_DUAL_POSITION = ["y-axis-1", "y-axis-2"];
const C_APP_PXWIDGET_CHART_LEGEND_POSITION = ["top", "left", "bottom", "right"];

//GeoJSON
C_APP_GEOJSON_PROPERTIES_UNIQUE_IDENTIFIER = "code";

//GoTo Params
C_APP_GOTO_PARAMS = "params";

//Format number max precision 
const C_APP_MAX_PRECISION = 6;