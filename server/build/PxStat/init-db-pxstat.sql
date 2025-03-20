
SET IDENTITY_INSERT [dbo].[TD_AUDITING] ON 
IF NOT EXISTS (SELECT 1 FROM [dbo].[TD_AUDITING] WHERE [DTG_ID] = 1)
INSERT [dbo].[TD_AUDITING] ([DTG_ID]) VALUES (1)
IF NOT EXISTS (SELECT 1 FROM [dbo].[TD_AUDITING] WHERE [DTG_ID] = 2)
INSERT [dbo].[TD_AUDITING] ([DTG_ID]) VALUES (2)
SET IDENTITY_INSERT [dbo].[TD_AUDITING] OFF

SET IDENTITY_INSERT dbo.TS_LANGUAGE ON
IF NOT EXISTS (SELECT 1 FROM dbo.TS_LANGUAGE WHERE LNG_ID = 1)
INSERT INTO TS_LANGUAGE(LNG_ID,LNG_ISO_CODE,LNG_ISO_NAME,LNG_DTG_ID,LNG_DELETE_FLAG) VALUES(1,'en','English',1,0)
SET IDENTITY_INSERT dbo.TS_LANGUAGE OFF



SET IDENTITY_INSERT TD_USER ON;
DECLARE @LngIdTdUser INT
IF NOT EXISTS (SELECT 1 FROM TD_USER WHERE USR_ID = 1)
INSERT INTO TD_USER (
	USR_ID
	,USR_LNG_ID
	) VALUES(
	1,1
	)

SET IDENTITY_INSERT TD_USER OFF;




SET IDENTITY_INSERT [dbo].[TS_AUDITING_TYPE] ON 
IF NOT EXISTS (SELECT 1 FROM [dbo].[TS_AUDITING_TYPE] WHERE [DTP_ID] = 1)
INSERT [dbo].[TS_AUDITING_TYPE] ([DTP_ID], [DTP_CODE], [DTP_VALUE]) VALUES (1, N'CREATED', N'Created')
IF NOT EXISTS (SELECT 1 FROM [dbo].[TS_AUDITING_TYPE] WHERE [DTP_ID] = 2)
INSERT [dbo].[TS_AUDITING_TYPE] ([DTP_ID], [DTP_CODE], [DTP_VALUE]) VALUES (2, N'UPDATED', N'Updated')
IF NOT EXISTS (SELECT 1 FROM [dbo].[TS_AUDITING_TYPE] WHERE [DTP_ID] = 3)
INSERT [dbo].[TS_AUDITING_TYPE] ([DTP_ID], [DTP_CODE], [DTP_VALUE]) VALUES (3, N'DELETED', N'Deleted')
SET IDENTITY_INSERT [dbo].[TS_AUDITING_TYPE] OFF



SET IDENTITY_INSERT [dbo].[TS_FORMAT] ON 
IF NOT EXISTS (SELECT 1 FROM [dbo].[TS_FORMAT] WHERE [FRM_ID] = 1)
INSERT [dbo].[TS_FORMAT] ([FRM_ID], [FRM_TYPE], [FRM_VERSION], [FRM_DIRECTION]) VALUES (1, N'PX', N'2013', 'UPLOAD')
IF NOT EXISTS (SELECT 1 FROM [dbo].[TS_FORMAT] WHERE [FRM_ID] = 2)
INSERT [dbo].[TS_FORMAT] ([FRM_ID], [FRM_TYPE], [FRM_VERSION], [FRM_DIRECTION]) VALUES (2, N'PX', N'2013', 'DOWNLOAD') 
IF NOT EXISTS (SELECT 1 FROM [dbo].[TS_FORMAT] WHERE [FRM_ID] = 3)
INSERT [dbo].[TS_FORMAT] ([FRM_ID], [FRM_TYPE], [FRM_VERSION], [FRM_DIRECTION]) VALUES (3, N'JSON-stat', N'1.0', 'DOWNLOAD')  
IF NOT EXISTS (SELECT 1 FROM [dbo].[TS_FORMAT] WHERE [FRM_ID] = 4)
INSERT [dbo].[TS_FORMAT] ([FRM_ID], [FRM_TYPE], [FRM_VERSION], [FRM_DIRECTION]) VALUES (4, N'JSON-stat', N'2.0', 'DOWNLOAD')
IF NOT EXISTS (SELECT 1 FROM [dbo].[TS_FORMAT] WHERE [FRM_ID] = 5)
INSERT [dbo].[TS_FORMAT] ([FRM_ID], [FRM_TYPE], [FRM_VERSION], [FRM_DIRECTION]) VALUES (5, N'CSV', N'1.0', 'DOWNLOAD') 
IF NOT EXISTS (SELECT 1 FROM [dbo].[TS_FORMAT] WHERE [FRM_ID] = 6)
INSERT [dbo].[TS_FORMAT] ([FRM_ID], [FRM_TYPE], [FRM_VERSION], [FRM_DIRECTION]) VALUES (6, N'XLSX', N'2007', 'DOWNLOAD')
SET IDENTITY_INSERT [dbo].[TS_FORMAT] OFF


IF NOT EXISTS (SELECT 1 FROM [dbo].TS_CHANNEL WHERE CHN_CODE ='TECHNICAL')
INSERT INTO TS_CHANNEL (CHN_CODE,CHN_NAME)
VALUES ('TECHNICAL','Technical')

IF NOT EXISTS (SELECT 1 FROM [dbo].TS_CHANNEL WHERE CHN_CODE ='BUSINESS')
INSERT INTO TS_CHANNEL (CHN_CODE,CHN_NAME)
VALUES ('BUSINESS','Business')

SET IDENTITY_INSERT [dbo].[TS_PRIVILEGE] ON 
IF NOT EXISTS (SELECT 1 FROM [dbo].[TS_PRIVILEGE] WHERE [PRV_ID] = 1)
INSERT [dbo].[TS_PRIVILEGE] ([PRV_ID], [PRV_CODE], [PRV_VALUE]) VALUES (1, N'ADMINISTRATOR', N'administrator')
IF NOT EXISTS (SELECT 1 FROM [dbo].[TS_PRIVILEGE] WHERE [PRV_ID] = 2)
INSERT [dbo].[TS_PRIVILEGE] ([PRV_ID], [PRV_CODE], [PRV_VALUE]) VALUES (2, N'POWER_USER', N'power-user')
IF NOT EXISTS (SELECT 1 FROM [dbo].[TS_PRIVILEGE] WHERE [PRV_ID] = 3)
INSERT [dbo].[TS_PRIVILEGE] ([PRV_ID], [PRV_CODE], [PRV_VALUE]) VALUES (3, N'MODERATOR', N'moderator')
SET IDENTITY_INSERT [dbo].[TS_PRIVILEGE] OFF


SET IDENTITY_INSERT [dbo].[TD_ACCOUNT] ON 
IF NOT EXISTS (SELECT 1 FROM [dbo].[TD_ACCOUNT] WHERE [CCN_ID] = 1)
INSERT [dbo].[TD_ACCOUNT] 
([CCN_ID], 
[CCN_USERNAME], 
[CCN_DISPLAYNAME], 
[CCN_EMAIL], 
[CCN_AD_FLAG], 
[CCN_PRV_ID], 
[CCN_NOTIFICATION_FLAG], 
[CCN_DTG_ID], 
[CCN_DELETE_FLAG],
[CCN_USR_ID])
 VALUES 
 (1, 
 'pxstat@abc.ie', 
 'pxstat@abc.ie', 
 'pxstat@abc.ie', 
 0, 
 1, 
 1,
 1, 
 0,
 1)
 
SET IDENTITY_INSERT [dbo].[TD_ACCOUNT] OFF

SET IDENTITY_INSERT [dbo].[TD_LOGIN] ON 
IF NOT EXISTS (SELECT 1 FROM [dbo].[TD_LOGIN] WHERE [LGN_ID] = 1)
INSERT [dbo].[TD_LOGIN] 
([LGN_ID], 
[LGN_1FA],
[LGN_2FA],
[LGN_TOKEN_1FA],
[LGN_TOKEN_2FA],
[LGN_CCN_ID]) 
VALUES 
(1, 
'9eef74d4498e4de85069eb738b828903a8e3d4bf7892ff20cd36826985bc08b7',
'xwtwmhgzwXnSfxBo',
'dfed7c1406505d595611b39b136d927e64c22bdd0f2159af4508425f24d51aac',
NULL,
1
)

SET IDENTITY_INSERT [dbo].[TD_LOGIN] OFF

SET IDENTITY_INSERT [dbo].[TM_AUDITING_HISTORY] ON 
IF NOT EXISTS (SELECT 1 FROM [dbo].[TM_AUDITING_HISTORY] WHERE [DHT_ID] = 1)
INSERT [dbo].[TM_AUDITING_HISTORY] ([DHT_ID],[DHT_DTG_ID], [DHT_DTP_ID], [DHT_CCN_ID], [DHT_DATETIME]) VALUES (1, 1, 1, 1, GETDATE())
IF NOT EXISTS (SELECT 1 FROM [dbo].[TM_AUDITING_HISTORY] WHERE [DHT_ID] = 2)
INSERT [dbo].[TM_AUDITING_HISTORY] ([DHT_ID], [DHT_DTG_ID], [DHT_DTP_ID], [DHT_CCN_ID], [DHT_DATETIME]) VALUES (2, 2, 1, 1, GETDATE())
SET IDENTITY_INSERT [dbo].[TM_AUDITING_HISTORY] OFF

SET IDENTITY_INSERT [dbo].[TS_REQUEST] ON 
IF NOT EXISTS (SELECT 1 FROM [dbo].[TS_REQUEST] WHERE [RQS_ID] = 1)
INSERT [dbo].[TS_REQUEST] ([RQS_ID], [RQS_CODE], [RQS_VALUE]) VALUES (1, N'PUBLISH', N'publish')
IF NOT EXISTS (SELECT 1 FROM [dbo].[TS_REQUEST] WHERE [RQS_ID] = 2)
INSERT [dbo].[TS_REQUEST] ([RQS_ID], [RQS_CODE], [RQS_VALUE]) VALUES (2, N'PROPERTY', N'update-properties')
IF NOT EXISTS (SELECT 1 FROM [dbo].[TS_REQUEST] WHERE [RQS_ID] = 3)
INSERT [dbo].[TS_REQUEST] ([RQS_ID], [RQS_CODE], [RQS_VALUE]) VALUES (3, N'DELETE', N'delete')
IF NOT EXISTS (SELECT 1 FROM [dbo].[TS_REQUEST] WHERE [RQS_ID] = 4)
INSERT [dbo].[TS_REQUEST] ([RQS_ID], [RQS_CODE], [RQS_VALUE]) VALUES (4, N'ROLLBACK', N'rollback')
SET IDENTITY_INSERT [dbo].[TS_REQUEST] OFF

SET IDENTITY_INSERT [dbo].[TS_RESPONSE] ON 
IF NOT EXISTS (SELECT 1 FROM [dbo].[TS_RESPONSE] WHERE [RSP_ID] = 1)
INSERT [dbo].[TS_RESPONSE] ([RSP_ID], [RSP_CODE], [RSP_VALUE]) VALUES (1, N'APPROVED', N'approved')
IF NOT EXISTS (SELECT 1 FROM [dbo].[TS_RESPONSE] WHERE [RSP_ID] = 2)
INSERT [dbo].[TS_RESPONSE] ([RSP_ID], [RSP_CODE], [RSP_VALUE]) VALUES (2, N'REJECTED', N'rejected')
SET IDENTITY_INSERT [dbo].[TS_RESPONSE] OFF

SET IDENTITY_INSERT [dbo].[TS_SIGNOFF] ON 
IF NOT EXISTS (SELECT 1 FROM [dbo].[TS_SIGNOFF] WHERE [SGN_ID] = 1)
INSERT [dbo].[TS_SIGNOFF] ([SGN_ID], [SGN_CODE], [SGN_VALUE]) VALUES (1, N'APPROVED', N'published')
IF NOT EXISTS (SELECT 1 FROM [dbo].[TS_SIGNOFF] WHERE [SGN_ID] = 2)
INSERT [dbo].[TS_SIGNOFF] ([SGN_ID], [SGN_CODE], [SGN_VALUE]) VALUES (2, N'REJECTED', N'rejected')
SET IDENTITY_INSERT [dbo].[TS_SIGNOFF] OFF		

IF (SELECT COUNT(*) FROM TS_DIMENSION_ROLE WHERE DMR_CODE='STATISTIC')=0
BEGIN
	INSERT INTO TS_DIMENSION_ROLE 
	(DMR_CODE,DMR_VALUE)
	VALUES
	('STATISTIC','Statistic')
END

IF (SELECT COUNT(*) FROM TS_DIMENSION_ROLE WHERE DMR_CODE='TIME')=0
BEGIN
	INSERT INTO TS_DIMENSION_ROLE 
	(DMR_CODE,DMR_VALUE)
	VALUES
	('TIME','Time')
END

IF (SELECT COUNT(*) FROM TS_DIMENSION_ROLE WHERE DMR_CODE='CLASSIFICATION')=0
BEGIN
	INSERT INTO TS_DIMENSION_ROLE 
	(DMR_CODE,DMR_VALUE)
	VALUES
	('CLASSIFICATION','Classification')	
END	

SET IDENTITY_INSERT [dbo].[TS_COPYRIGHT] ON 

IF NOT EXISTS
(SELECT 1 FROM TS_COPYRIGHT WHERE CPR_DELETE_FLAG=0)
INSERT [dbo].[TS_COPYRIGHT](CPR_ID,CPR_CODE,CPR_VALUE,CPR_URL,CPR_DTG_ID,CPR_DELETE_FLAG)
VALUES (1,'NSI','National Statistics Institute','https://www.stats.com',1,0)

SET IDENTITY_INSERT [dbo].[TS_COPYRIGHT] OFF

IF NOT EXISTS
(SELECT 1
FROM TS_CONFIG_SETTING_TYPE
WHERE CST_CODE = 'API')
INSERT INTO TS_CONFIG_SETTING_TYPE VALUES('API','API CONFIG');

IF NOT EXISTS
(SELECT 1
FROM TS_CONFIG_SETTING_TYPE
WHERE CST_CODE = 'APP')
INSERT INTO TS_CONFIG_SETTING_TYPE VALUES('APP','APPPLICATION_CONFIG');

IF NOT EXISTS
(SELECT 1
FROM TM_APP_SETTING_CONFIG_VERSION
join TS_CONFIG_SETTING_TYPE on ASV_CST_ID = CST_ID where CST_CODE = 'API')
INSERT INTO TM_APP_SETTING_CONFIG_VERSION
SELECT 1.0,CST_ID FROM TS_CONFIG_SETTING_TYPE WHERE CST_CODE = 'API';

IF NOT EXISTS
(SELECT 1
FROM TM_APP_SETTING_CONFIG_VERSION
join TS_CONFIG_SETTING_TYPE on ASV_CST_ID = CST_ID where CST_CODE = 'APP')
INSERT INTO TM_APP_SETTING_CONFIG_VERSION
SELECT 1.0,CST_ID FROM TS_CONFIG_SETTING_TYPE WHERE CST_CODE = 'APP';


DECLARE @APIID INT;
DECLARE @APPID INT;

SELECT @APIID = ASV_ID FROM TM_APP_SETTING_CONFIG_VERSION
INNER JOIN TS_CONFIG_SETTING_TYPE ON CST_ID = ASV_CST_ID
 WHERE CST_CODE = 'API';

 SELECT @APPID = ASV_ID FROM TM_APP_SETTING_CONFIG_VERSION
INNER JOIN TS_CONFIG_SETTING_TYPE ON CST_ID = ASV_CST_ID
 WHERE CST_CODE = 'APP';


if not exists (select 1 from ts_api_setting where api_key='API_AUTHENTICATION_TYPE')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_AUTHENTICATION_TYPE','ANONYMOUS','Json config for API_AUTHENTICATION_TYPE','0' )

if not exists (select 1 from ts_api_setting where api_key='API_STATELESS')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_STATELESS','TRUE','Json config for API_STATELESS','0' )

if not exists (select 1 from ts_api_setting where api_key='API_SUCCESS')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_SUCCESS','success','Json config for API_SUCCESS','0' )

if not exists (select 1 from ts_api_setting where api_key='API_SESSION_COOKIE')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_SESSION_COOKIE','session','Json config for API_SESSION_COOKIE','1' )

if not exists (select 1 from ts_api_setting where api_key='API_RESOURCE_I18N_DIRECTORY')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_RESOURCE_I18N_DIRECTORY','Resources/Internationalisation','Json config for API_RESOURCE_I18N_DIRECTORY','0' )

if not exists (select 1 from ts_api_setting where api_key='API_MASK_PARAMETERS')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_MASK_PARAMETERS','MtrInput,Lgn1Fa,Totp','Json config for API_JSONRPC_MASK_PARAMETERS','0' )

if not exists (select 1 from ts_api_setting where api_key='API_AD_DOMAIN')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_AD_DOMAIN','','Json config for API_AD_DOMAIN','0' )

if not exists (select 1 from ts_api_setting where api_key='API_AD_PATH')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_AD_PATH','','Json config for API_AD_PATH','0' )

if not exists (select 1 from ts_api_setting where api_key='API_AD_USERNAME')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_AD_USERNAME','','Json config for API_AD_USERNAME','0' )

if not exists (select 1 from ts_api_setting where api_key='API_AD_PASSWORD')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_AD_PASSWORD','','Json config for API_AD_PASSWORD','1' )

if not exists (select 1 from ts_api_setting where api_key='API_AD_CUSTOM_PROPERTIES')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_AD_CUSTOM_PROPERTIES','','Json config for API_AD_CUSTOM_PROPERTIES','0' )

if not exists (select 1 from ts_api_setting where api_key='API_RECAPTCHA_ENABLED')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_RECAPTCHA_ENABLED','FALSE','Json config for API_RECAPTCHA_ENABLED','0' )

if not exists (select 1 from ts_api_setting where api_key='API_RECAPTCHA_URL')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_RECAPTCHA_URL','','Json config for API_RECAPTCHA_URL','0' )

if not exists (select 1 from ts_api_setting where api_key='API_RECAPTCHA_PRIVATE_KEY')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_RECAPTCHA_PRIVATE_KEY','6LcmUSMjAAAAALgn-YtP-CLAr5xkbK7eFI-xhMWq','Json config for API_RECAPTCHA_PRIVATE_KEY','1' )

if not exists (select 1 from ts_api_setting where api_key='API_EMAIL_ENABLED')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_EMAIL_ENABLED','TRUE','Json config for API_EMAIL_ENABLED','0' )

if not exists (select 1 from ts_api_setting where api_key='API_EMAIL_MAIL_NOREPLY')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_EMAIL_MAIL_NOREPLY','noreply.pxstat@cso.ie','Json config for API_EMAIL_MAIL_NOREPLY','0' )

if not exists (select 1 from ts_api_setting where api_key='API_EMAIL_MAIL_SENDER')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_EMAIL_MAIL_SENDER','noreply.pxstat@cso.ie','Json config for API_EMAIL_MAIL_SENDER','0' )

if not exists (select 1 from ts_api_setting where api_key='API_EMAIL_SMTP_SERVER')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_EMAIL_SMTP_SERVER','3.1.2.22','Json config for API_EMAIL_SMTP_SERVER','0' )

if not exists (select 1 from ts_api_setting where api_key='API_EMAIL_SMTP_PORT')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_EMAIL_SMTP_PORT','25','Json config for API_EMAIL_SMTP_PORT','0' )

if not exists (select 1 from ts_api_setting where api_key='API_EMAIL_SMTP_AUTHENTICATION')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_EMAIL_SMTP_AUTHENTICATION','FALSE','Json config for API_EMAIL_SMTP_AUTHENTICATION','0' )

if not exists (select 1 from ts_api_setting where api_key='API_EMAIL_SMTP_USERNAME')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_EMAIL_SMTP_USERNAME','','Json config for API_EMAIL_SMTP_USERNAME','1' )

if not exists (select 1 from ts_api_setting where api_key='API_EMAIL_SMTP_PASSWORD')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_EMAIL_SMTP_PASSWORD','','Json config for API_EMAIL_SMTP_PASSWORD','1' )

if not exists (select 1 from ts_api_setting where api_key='API_EMAIL_SMTP_SSL')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_EMAIL_SMTP_SSL','FALSE','Json config for API_EMAIL_SMTP_SSL','0' )

if not exists (select 1 from ts_api_setting where api_key='API_EMAIL_DATETIME_MASK')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_EMAIL_DATETIME_MASK','dd/MM/yyyy - HH:mm:ss','Json config for API_EMAIL_DATETIME_MASK','0' )

if not exists (select 1 from ts_api_setting where api_key='API_ADO_DEFAULT_CONNECTION')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_ADO_DEFAULT_CONNECTION','defaultConnection','Json config for API_ADO_DEFAULT_CONNECTION','0' )

if not exists (select 1 from ts_api_setting where api_key='API_ADO_DB_CONNECTION_TYPE')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_ADO_DB_CONNECTION_TYPE','SQL','Json config for API_ADO_DB_CONNECTION_TYPE','0' )

if not exists (select 1 from ts_api_setting where api_key='API_ADO_DB_AD_DOMAIN')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_ADO_DB_AD_DOMAIN','','Json config for API_ADO_DB_AD_DOMAIN','0' )

if not exists (select 1 from ts_api_setting where api_key='API_ADO_DB_AD_CONNECTION_USERNAME')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_ADO_DB_AD_CONNECTION_USERNAME','','Json config for API_ADO_DB_AD_CONNECTION_USERNAME','1' )

if not exists (select 1 from ts_api_setting where api_key='API_ADO_DB_AD_CONNECTION_PASSWORD')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_ADO_DB_AD_CONNECTION_PASSWORD','','Json config for API_ADO_DB_AD_CONNECTION_PASSWORD','1' )

if not exists (select 1 from ts_api_setting where api_key='API_FIREBASE_ENABLED')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_FIREBASE_ENABLED','TRUE','Json config for API_FIREBASE_ENABLED','0' )

if not exists (select 1 from ts_api_setting where api_key='API_FIREBASE_APP_NAME')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_FIREBASE_APP_NAME','test-data.cso.ie','Json config for API_FIREBASE_APP_NAME','0' )

if not exists (select 1 from ts_api_setting where api_key='API_FIREBASE_SALSA')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_FIREBASE_SALSA','dev-data-netcore.cso.ie','','0' )

if not exists (select 1 from ts_api_setting where api_key='API_FIREBASE_CREDENTIAL')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_FIREBASE_CREDENTIAL','','Json config for API_FIREBASE_CREDENTIAL','1' )

if not exists (select 1 from ts_api_setting where api_key='API_AD_CONNECTION')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_AD_CONNECTION','','','0' )

if not exists (select 1 from ts_api_setting where api_key='API_AD_QUERY_FILTER')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_AD_QUERY_FILTER','','AD Query Filter','0' )

if not exists (select 1 from ts_api_setting where api_key='API_AD_BLACKLIST_OUS')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_AD_BLACKLIST_OUS','','Forbidden OUs in Active Directory query','0' )

if not exists (select 1 from ts_api_setting where api_key='API_TRACE_ENABLED')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_TRACE_ENABLED','true','API level trace enabled','0' )

if not exists (select 1 from ts_api_setting where api_key='API_TRACE_RECORD_IP')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'API_TRACE_RECORD_IP','false','When API level trace enabled, record the ip address','0' )

if not exists (select 1 from ts_api_setting where api_key='SANITIZER_REMOVE_ALLOWED_TAGS')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'SANITIZER_REMOVE_ALLOWED_TAGS','','List of allowed tags to remove','0' )

if not exists (select 1 from ts_api_setting where api_key='SANITIZER_REMOVE_ALLOWED_ATTRIBUTES')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'SANITIZER_REMOVE_ALLOWED_ATTRIBUTES','','List of allowed attributes to remove','0' )

if not exists (select 1 from ts_api_setting where api_key='SANITIZER_REMOVE_ALLOWED_CSSCLASSESS')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'SANITIZER_REMOVE_ALLOWED_CSSCLASSESS','','List of css classes to remove','0' )

if not exists (select 1 from ts_api_setting where api_key='SANITIZER_REMOVE_ALLOWED_ATRULES')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'SANITIZER_REMOVE_ALLOWED_ATRULES','','List of at rules to remove','0' )

if not exists (select 1 from ts_api_setting where api_key='SANITIZER_REMOVE_ALLOWED_SCHEMES')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'SANITIZER_REMOVE_ALLOWED_SCHEMES','','List of schemes to remove','0' )

if not exists (select 1 from ts_api_setting where api_key='SANITIZER_REMOVE_URI_ATTRIBUTES')
insert into ts_api_setting (api_asv_id,api_key,api_value,API_DESCRIPTION,API_SENSITIVE_VALUE)
values (@APIID,'SANITIZER_REMOVE_URI_ATTRIBUTES','','List of URI attributes to remove','0' )


  Declare @asvID int
  DECLARE @cstId int
  set @cstId=(select CST_ID FROM TS_CONFIG_SETTING_TYPE WHERE CST_CODE='APP')
  IF(@cstId is null)
  BEGIN
	INSERT INTO TS_CONFIG_SETTING_TYPE(CST_CODE,CST_VALUE)
	VALUES('APP','APP CONFIG')
	set @cstId=(select CST_ID FROM TS_CONFIG_SETTING_TYPE WHERE CST_CODE='APP')
  END


  set @asvID=(select max(asv_id) from TM_APP_SETTING_CONFIG_VERSION where ASV_CST_ID=@cstId)

  IF @asvID IS NULL
  BEGIN
	INSERT INTO TM_APP_SETTING_CONFIG_VERSION (ASV_VERSION,ASV_CST_ID)
	VALUES(1.00,@cstId)
	set @asvID=(select max(asv_id) from TM_APP_SETTING_CONFIG_VERSION where ASV_CST_ID=@cstId) 
  END


  
if not exists (select 1 from ts_app_setting where app_key='config.client.json')  
insert into ts_app_setting (app_asv_id,app_key,app_value,APP_DESCRIPTION,APP_SENSITIVE_VALUE)
values (@asvId,'config.client.json','{
	"organisation": "Demo",
	"corsDomain": "cso.ie",
	"url": {
		"api": {
			"jsonrpc": {
				"public": "http://localhost:5000/api.jsonrpc",
				"private": "http://localhost:5000/api.jsonrpc"
			}
		}
	},
	"template": {
		"footer": {
			"contact": {
				"address": "Your full address here.",
				"phone": "(+353) 21 453 5000",
				"email": "dissemination@cso.ie"
			},
			"social": [

			],
			"links": [
				{
					"text": "",
					"url": ""
				}
			],
			"watermark": {
				"src": "",
				"alt": "",
				"url": ""
			}
		},
		"quickLinks": []
	},
	"mask": {
		"datetime": {
			"ajax": "YYYY-MM-DDTHH:mm:ss",
			"display": "DD/MM/YYYY HH:mm:ss",
			"file": "YYYYMMDDTHHMMss",
			"dateRangePicker": "DD/MM/YYYY HH:mm"
		},
		"date": {
			"ajax": "YYYY-MM-DD",
			"display": "DD/MM/YYYY",
			"dateRangePicker": "DD/MM/YYYY"
		},
		"time": {
			"display": "HH:mm:ss"
		}
	},
	"transfer": {
		"timeout": 3600000,
		"threshold": {
			"soft": 1048576,
			"hard": 1073741824
		},
		"unitsPerSecond": {
			"PxStat.Build.Build_API.Validate": 250000,
			"PxStat.Build.Build_API.Read": 250000,
			"PxStat.Build.Build_API.ReadTemplate": 250000,
			"PxStat.Build.Build_API.ReadDataset": 250000,
			"PxStat.Build.Build_API.Update": 40000,
			"PxStat.Data.Matrix_API.Validate": 500000,
			"PxStat.Data.Matrix_API.Create": 700000,
			"PxStat.Data.GeoMap_API.Validate": 500000
		}
	},
	"entity": {
		"data": {
			"datatable": {
				"length": 100,
				"null": ".."
			},
			"threshold": {
				"soft": 1000,
				"hard": 1000000
			},
			"pagination": 10,
			"lastUpdatedTables": {
				"defaultPageLength": 10,
				"defaultNumDaysFrom": 6
			},
			"chartJs": {
				"chart": {
					"enabled": true
				},
				"map": {
					"enabled": true
				}
			},
			"snippet": null,
			"properties": {
				"archive": "https://www.cso.ie/en/methods/archivedoutputs/",
				"underReservation": "https://www.cso.ie/en/methods/statisticsunderreservation/",
				"experimental": "https://www.cso.ie/en/methods/frontierseriesoutputs/"
			},
			"display": {
				"copyright": true,
				"language": true,
				"wipWidgetStandard": false
			}
		},
		"map": {
			"baseMap": {
				"leaflet": [
					{
						"url": "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png",
						"options": {
							"attribution": "&copy; <a target=\"_blank\" href=\"https://www.openstreetmap.org/copyright\">OpenStreetMap</a>"
						}
					}
				],
				"esri": [
					{
						"url": "https://utility.arcgis.com/usrsvcs/servers/88f1db9e4ae04df69e499223b8295843/rest/services/MapGeniePremiumWM/MapServer"
					}
				]
			}
		},
		"build": {
			"threshold": {
				"soft": 1000000,
				"geoJson": 3145728
			},
			"geoJsonLookup": {
				"enabled": true,
				"href": "https://geojson.cso.ie/"
			}
		},
		"openAccess": {
			"recaptcha": {
				"siteKey": ""
			},
			"authenticator": "dev-data.cso.ie"
		},
		"release": {
			"comparison": {
				"threshold": {
					"soft": 1048576
				},
				"differenceClass": "table-danger"
			}
		},
		"analytic": {
			"dateRangePicker": 30
		}
	},
	"plugin": {
		"sharethis": {
			"enabled": true,
			"apiURL": "https://platform-api.sharethis.com/js/sharethis.js#property={0}&product=inline-share-buttons",
			"apiKey": ""
		},
		"jscookie": {
			"session": {
				"path": "/",
				"secure": "true"
			},
			"persistent": {
				"path": "/",
				"secure": "true",
				"expires": 365
			}
		},
		"datatable": {
			"lengthMenu": [
				[
					10,
					25,
					50,
					100,
					-1
				],
				[
					10,
					25,
					50,
					100,
					"All"
				]
			],
			"responsive": true,
			"fixedHeader": true,
			"dom": "fltip",
			"deferRender": true
		},
		"chartJs": {
			"chart": {
				"options": {
					"responsive": true,
					"maintainAspectRatio": false,
					"title": {
						"display": true,
						"text": []
					},
					"tooltips": {
						"mode": "index",
						"intersect": false,
						"callbacks": {
							"label": null
						}
					},
					"hover": {
						"mode": "nearest",
						"intersect": true
					},
					"scales": {
						"xAxes": [
							{
								"ticks": {
									"beginAtZero": false,
									"maxTicksLimit": null
								},
								"gridLines": {
									"display": false
								},
								"scaleLabel": {
									"display": false,
									"labelString": null
								}
							}
						],
						"yAxes": [
							{
								"display": true,
								"position": "left",
								"id": null,
								"ticks": {
									"beginAtZero": false
								},
								"callback": null,
								"scaleLabel": {
									"display": false,
									"labelString": null
								}
							}
						]
					},
					"plugins": {
						"stacked100": {
							"enable": false,
							"replaceTooltipLabel": false
						},
						"colorschemes": {
							"scheme": null
						}
					},
					"legend": {
						"display": true,
						"position": "bottom"
					},
					"elements": {
						"line": {
							"tension": 0.4
						}
					}
				},
				"colours": [
					"#405381",
					"#5BC1A5",
					"#FCBE72",
					"#0091AB",
					"#90989F",
					"#3D5999",
					"#C6CC5C",
					"#579599",
					"#FBAA34",
					"#007780",
					"#B9BEC3",
					"#00AF86",
					"#6BC2C2",
					"#00758C",
					"#FCB053",
					"#6A7794",
					"#A3CCC1",
					"#F68B58",
					"#36A0B3"
				]
			}
		},
		"subscriber": {
			"enabled": true,
			"firebase": {
				"config": {
					"apiKey": "",
					"authDomain": "",
					"projectId": "",
					"storageBucket": "",
					"messagingSenderId": "",
					"appId": ""
				},
				"providers": {
					"emailPassword": true,
					"google": true,
					"facebook": true,
					"gitHub": true,
					"twitter": true
				}
			}
		},
		"leaflet": {
			"colourScale": [
				{
					"value": "red",
					"name": "red"
				},
				{
					"value": "yellow",
					"name": "yellow"
				},
				{
					"value": "blue",
					"name": "blue"
				},
				{
					"value": "darkorange",
					"name": "orange"
				},
				{
					"value": "darkviolet",
					"name": "violet"
				}
			],
			"mode": [
				{
					"value": "q",
					"label": "quantile"
				},
				{
					"value": "e",
					"label": "equidistant"
				},
				{
					"value": "k",
					"label": "k-means"
				}
			],
			"defaultMode": "q",
			"baseMap": {
				"leaflet": [],
				"esri": [
					{
						"url": "https://utility.arcgis.com/usrsvcs/servers/88f1db9e4ae04df69e499223b8295843/rest/services/MapGeniePremiumWM/MapServer",
						"disclaimer": "You can use the OSi basemap layer only in conjunction with the CSO map widgets, all other rights are reserved by OSi."
					}
				]
			}
		}
	}
}','Json config for config.client.json','0' )
if not exists (select 1 from ts_app_setting where app_key='config.global.json')  
insert into ts_app_setting (app_asv_id,app_key,app_value,APP_DESCRIPTION,APP_SENSITIVE_VALUE)
values (@asvId,'config.global.json','{
    "title": "PxStat on Docker",
    "language": {
        "iso": {
            "code": "en",
            "name": "English"
        },
        "culture": "en-ie"
    },
    "url": {
        "api": {
            "restful": "http://localhost:5000/api.restful",
            "static": "http://localhost:5000/api.static"
        },
        "logo": "https://cdn.jsdelivr.net/gh/CSOIreland/Client-API-Library@4.2.2/test/image/logo.png",
        "application": "http://localhost:5000/"
    },
    "dataset": {
        "officialStatistics": true,
        "download": {
            "threshold": {
                "csv": 1048575,
                "xlsx": 1048575
            }
        },
        "analytical": {
            "label": "dependency",
            "icon": "fa-solid fa-circle-nodes",
            "colour": "text-analytical",
            "display": true
        }
    },
    "regex": {
        "phone": {
            "pattern": "^(.*)$",
            "placeholder": "(+353)214522253"
        },
        "password": "^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[\\W]).{8,}$",
        "matrix-name": ".*",
        "product-code": "^[a-zA-Z0-9]+$"
    },
    "workflow": {
        "embargo": {
            "time": "08:00:00",
            "day": [
                1,
                2,
                3,
                4,
                5
            ]
        },
        "fastrack": {
            "response": {
                "approver": true,
                "poweruser": true,
                "administrator": true
            },
            "signoff": {
                "poweruser": true,
                "administrator": true
            }
        },
        "release": {
            "reasonRequired": false
        }
    },
    "build": {
        "create": {
            "moderator": true
        },
        "update": {
            "moderator": true
        },
        "import": {
            "moderator": true
        }
    },
    "session": {
        "length": 1200
    },
    "security": {
        "adOpenAccess": false,
        "demo": true,
        "tokenApiAccessIpMaskWhitelist": [
            {
                "prefix": "255.255.255.255",
                "length": 0
            }
        ]
    },
    "search": {
        "maximumResults": 100
    },
    "report": {
        "date-validation": {
            "minDate": 365,
            "maxDate": -1
        }
    }
}
','Json config for config.global.json','0' )
if not exists (select 1 from ts_app_setting where app_key='config.server.json')  
insert into ts_app_setting (app_asv_id,app_key,app_value,APP_DESCRIPTION,APP_SENSITIVE_VALUE)
values (@asvId,'config.server.json','{
    "bulkcopy-tranche-multiplier": 10,
    "px": {
        "confidential-value": "..",
        "default-units": "-"
    },
    "analytic": {
        "read-os-item-limit": 10,
        "read-browser-item-limit": 10,
        "read-referrer-item-limit": 100,
        "referrer-not-applicable": "n/a",
        "read-environment-language-limit": 100
    },
    "search": {
        "synonym-multiplier": 1,
        "search-word-multiplier": 15,
        "release_word_multiplier": 1,
        "product_word_multiplier": 10,
        "subject_word_multiplier": 100
    },
    "language": {
        "iso": {
            "code": "en",
            "name": "English"
        }
    },
    "release": {
        "lockTimeMinutes": 60,
        "defaultReason": "02ROUTINE"
    },
    "subscription": {
        "query-threshold": 1000
    },
    "maintenance": {
        "post-import-delete": false
    },
    "throttle": {
        "subscribedWindowSeconds": 60,
        "subscribedCallLimit": 30,
        "nonSubscribedWindowSeconds": 180,
        "nonSubscribedCallLimit": 10
    },
    "whitelist": [
        "cso.ie"
    ],
    "pxapiv1": {
        "formats": [
            "json-stat",
            "json-stat2",
            "csv",
            "px",
            "xlsx"
        ]
    }
}','Json config for config.server.json','0' )

IF NOT EXISTS (SELECT 1 FROM TD_THEME WHERE THM_CODE=1)
  INSERT INTO TD_THEME(THM_CODE,THM_VALUE,THM_LNG_ID,THM_DTG_ID,THM_DELETE_FLAG)
  VALUES(1,'Default',1,1,0)
  IF NOT EXISTS (SELECT 1 FROM TD_GROUP WHERE GRP_CODE='GRP01')
  INSERT INTO TD_GROUP(GRP_CODE,GRP_NAME,GRP_CONTACT_NAME,GRP_CONTACT_PHONE,GRP_CONTACT_EMAIL,GRP_DTG_ID,GRP_DELETE_FLAG)
  VALUES('GRP01','DEFAULT',NULL,NULL,'pxstat@abc.ie',1,0)

  IF NOT EXISTS (SELECT 1 FROM TS_REASON WHERE RSN_CODE='01NORMAL')
  INSERT INTO TS_REASON(RSN_CODE,RSN_VALUE_INTERNAL,RSN_VALUE_EXTERNAL,RSN_LNG_ID,RSN_DTG_ID,RSN_DELETE_FLAG )
  VALUES('01NORMAL','Planned dissemnation/release','Planned Release',1,1,0)

  IF NOT EXISTS (SELECT 1 FROM TS_REASON WHERE RSN_CODE='02ROUTINE')
  INSERT INTO TS_REASON(RSN_CODE,RSN_VALUE_INTERNAL,RSN_VALUE_EXTERNAL,RSN_LNG_ID,RSN_DTG_ID,RSN_DELETE_FLAG )
  VALUES('02ROUTINE','Routine Revision','Planned Routine Revision',1,1,0)

  IF NOT EXISTS (SELECT 1 FROM TS_REASON WHERE RSN_CODE='03MAJOR')
  INSERT INTO TS_REASON(RSN_CODE,RSN_VALUE_INTERNAL,RSN_VALUE_EXTERNAL,RSN_LNG_ID,RSN_DTG_ID,RSN_DELETE_FLAG )
  VALUES('03MAJOR','Major Revision','Planned Major Revision',1,1,0)

  IF NOT EXISTS (SELECT 1 FROM TS_REASON WHERE RSN_CODE='04UNPLANNED')
  INSERT INTO TS_REASON(RSN_CODE,RSN_VALUE_INTERNAL,RSN_VALUE_EXTERNAL,RSN_LNG_ID,RSN_DTG_ID,RSN_DELETE_FLAG )
  VALUES('04UNPLANNED','Unplanned Revision','Unplanned Revision',1,1,0)

