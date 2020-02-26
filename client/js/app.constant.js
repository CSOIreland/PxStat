
/*******************************************************************************
Application - Constant
*******************************************************************************/

// Application Version
const C_APP_VERSION = "2.0.0";

// URLs
const C_APP_URL_GITHUB = "https://github.com/CSOIreland/PxStat";
const C_APP_URL_GITHUB_RELEASE_TAG = "https://github.com/CSOIreland/PxStat/releases/tag/{0}";
const C_APP_URL_GITHUB_REPORT_ISSUE = "https://github.com/CSOIreland/PxStat/issues/new/choose";
const C_APP_URL_GITHUB_API_CUBE = "https://github.com/CSOIreland/PxStat/wiki/API-Cube";

//Charts + Maps
/*******************************************************************************
PLEASE REFER TO https://shop.highsoft.com/highcharts
PLEASE DO NOT SET app.config.plugin.highcharts.enabled UNLESS TO TRUE UNLESS YOU HAVE A VALID HIGHCHARTS LICENSE
*******************************************************************************/
const C_APP_HIGHCHARTS_NO_LICENSE_MESSAGE = $("<a>", {
    href: "https://www.highcharts.com",
    text: "https://www.highcharts.com",
    target: "_blank"
}).get(0).outerHTML;

const C_APP_API_SUCCESS = "success";
const C_APP_API_JSONRPC_VERSION = "2.0";

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

// Cookie Links
const C_COOKIE_LINK_SEARCH = "search";
const C_COOKIE_LINK_PRODUCT = "product";
const C_COOKIE_LINK_COPYRIGHT = "copyright";
const C_COOKIE_LINK_TABLE = "table";
const C_COOKIE_LINK_RELEASE = "release";

//User Privileges (RQS_CODE)
const C_APP_PRIVILEGE_ADMINISTRATOR = "ADMINISTRATOR";
const C_APP_PRIVILEGE_POWER_USER = "POWER_USER";
const C_APP_PRIVILEGE_MODERATOR = "MODERATOR";

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

//Formats (FRM_DIRECTION)
const C_APP_TS_FORMAT_DIRECTION_UPLOAD = "UPLOAD";
const C_APP_TS_FORMAT_DIRECTION_DOWNLOAD = "DOWNLOAD";

//Default formats
const C_APP_FORMAT_TYPE_DEFAULT = C_APP_TS_FORMAT_TYPE_JSONSTAT;
const C_APP_FORMAT_VERSION_DEFAULT = "2.0";

// File extensions
const C_APP_EXTENSION_PX = "px";
const C_APP_EXTENSION_JSON = "json";
const C_APP_EXTENSION_CSV = "csv";
const C_APP_EXTENSION_XLSX = "xlsx";

// File Mimetype
const C_APP_MIMETYPE_PX = "text/plain";
const C_APP_MIMETYPE_JSON = "application/json";
const C_APP_MIMETYPE_CSV = "text/csv";
const C_APP_MIMETYPE_XLSX = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

// Datatable
const C_APP_DATATABLE_ROW_INDEX = "row-index";
const C_APP_DATATABLE_EXTRA_INFO_LINK = "extra-info-link";

// Upload
const C_APP_UPLOAD_FILE_ALLOWED_EXTENSION = ['.px'];
const C_APP_UPLOAD_FILE_ALLOWED_TYPE = ['', 'text/x-pcaxis', 'text/plain']; // PX file HAS NOT (yet) a registered MIME Type

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

//CSV structure
const C_APP_CSV_CODE = "CODE";
const C_APP_CSV_VALUE = "VALUE";
const C_APP_CSV_UNIT = "UNIT";
const C_APP_CSV_DECIMAL = "DECIMAL";

const C_APP_ANALYTIC_MODULE_HIGHCHARTS = "entity/analytic/js/analytic.library.highCharts.js";
const C_APP_ANALYTIC_MODULE_GOOGLE = "entity/analytic/js/analytic.library.google.js";
const C_APP_ANALYTIC_MODULE_DEFAULT = C_APP_ANALYTIC_MODULE_GOOGLE;

//Toggle Length
const C_APP_TOGGLE_LENGTH = 100;

//GeoJson
const C_APP_GEOJSON_FEATURE_COLLECTION = "FeatureCollection";

//Datetime 
const C_APP_DATETIME_DEFAULT = "0001-01-01T0:00:00.000Z";