-- drop Jobs and dependencies
-- drop StoredProcedures and dependencies
-- drop Views and dependencies
-- drop Types and dependencies
-- alter database structure
CREATE TABLE [dbo].TD_USER (
	USR_ID INT IDENTITY(1, 1) NOT NULL
	,USR_LNG_ID INT NOT NULL	
	,CONSTRAINT [PK_TD_USER] PRIMARY KEY CLUSTERED ([USR_ID] ASC) WITH (
		PAD_INDEX = OFF
		,STATISTICS_NORECOMPUTE = OFF
		,IGNORE_DUP_KEY = OFF
		,ALLOW_ROW_LOCKS = ON
		,ALLOW_PAGE_LOCKS = ON
		) ON [PRIMARY]
	) ON [PRIMARY]
GO

ALTER TABLE [dbo].TD_USER
	WITH CHECK ADD CONSTRAINT [FK_TD_USER_TS_LANGUAGE] FOREIGN KEY ([USR_LNG_ID]) REFERENCES [dbo].TS_LANGUAGE([LNG_ID])
GO

ALTER TABLE [dbo].TD_USER CHECK CONSTRAINT [FK_TD_USER_TS_LANGUAGE]
GO

CREATE NONCLUSTERED INDEX [IX_USR_LNG_ID] ON [dbo].[TD_USER] ([USR_LNG_ID] ASC)
	WITH (
			PAD_INDEX = OFF
			,STATISTICS_NORECOMPUTE = OFF
			,SORT_IN_TEMPDB = OFF
			,DROP_EXISTING = OFF
			,ONLINE = OFF
			,ALLOW_ROW_LOCKS = ON
			,ALLOW_PAGE_LOCKS = ON
			) ON [PRIMARY]
GO




ALTER TABLE TD_ACCOUNT ADD CCN_USR_ID INT
GO

UPDATE TD_ACCOUNT
SET CCN_USR_ID = CCN_ID
GO

ALTER TABLE TD_ACCOUNT

ALTER COLUMN CCN_USR_ID INT NOT NULL
GO

SET IDENTITY_INSERT TD_USER ON;

DECLARE @LngIdTdUser INT

SET @LngIdTdUser = (
		SELECT LNG_ID
		FROM TS_LANGUAGE
		WHERE LNG_ISO_CODE = 'en'
			AND LNG_DELETE_FLAG = 0
		)

INSERT INTO TD_USER (
	USR_ID
	,USR_LNG_ID
	) (
	SELECT CCN_USR_ID
	,@LngIdTdUser FROM TD_ACCOUNT
	)

SET IDENTITY_INSERT TD_USER OFF;

ALTER TABLE [dbo].[TD_ACCOUNT]
	WITH CHECK ADD CONSTRAINT [FK_TD_ACCOUNT_TD_USER] FOREIGN KEY ([CCN_USR_ID]) REFERENCES [dbo].[TD_USER]([USR_ID])
GO

ALTER TABLE [dbo].[TD_ACCOUNT] CHECK CONSTRAINT FK_TD_ACCOUNT_TD_USER
GO

CREATE NONCLUSTERED INDEX [IX_CCN_USER_ID] ON [dbo].[TD_ACCOUNT] ([CCN_USR_ID] ASC)
	WITH (
			PAD_INDEX = OFF
			,STATISTICS_NORECOMPUTE = OFF
			,SORT_IN_TEMPDB = OFF
			,DROP_EXISTING = OFF
			,ONLINE = OFF
			,ALLOW_ROW_LOCKS = ON
			,ALLOW_PAGE_LOCKS = ON
			) ON [PRIMARY]
GO

CREATE TABLE TS_CHANNEL (
	CHN_ID INT IDENTITY(1, 1) NOT NULL
	,CHN_CODE NVARCHAR(256) NOT NULL 
	,CHN_NAME NVARCHAR(256) NOT NULL CONSTRAINT [PK_TS_DOMAIN] PRIMARY KEY CLUSTERED ([CHN_ID] ASC) WITH (
		PAD_INDEX = OFF
		,STATISTICS_NORECOMPUTE = OFF
		,IGNORE_DUP_KEY = OFF
		,ALLOW_ROW_LOCKS = ON
		,ALLOW_PAGE_LOCKS = ON
		) ON [PRIMARY]
	) ON [PRIMARY]
GO

CREATE TABLE TS_CHANNEL_LANGUAGE(
	CHL_ID INT IDENTITY(1, 1) NOT NULL
	,CHL_LNG_ID INT NOT NULL
	,CHL_CHN_ID INT NOT NULL
	,CHL_CHN_NAME NVARCHAR(256) NOT NULL CONSTRAINT [PK_TS_CHANNEL_LANGUAGE] PRIMARY KEY CLUSTERED ([CHL_ID] ASC) WITH (
		PAD_INDEX = OFF
		,STATISTICS_NORECOMPUTE = OFF
		,IGNORE_DUP_KEY = OFF
		,ALLOW_ROW_LOCKS = ON
		,ALLOW_PAGE_LOCKS = ON
		) ON [PRIMARY]
	) ON [PRIMARY]
GO


ALTER TABLE [dbo].TS_CHANNEL_LANGUAGE
	WITH CHECK ADD CONSTRAINT [FK_TS_CHANNEL_LANGUAGE_TS_LANGUAGE] FOREIGN KEY ([CHL_LNG_ID]) REFERENCES [dbo].TS_LANGUAGE([LNG_ID])
GO

ALTER TABLE [dbo].TS_CHANNEL_LANGUAGE CHECK CONSTRAINT [FK_TS_CHANNEL_LANGUAGE_TS_LANGUAGE]
GO

CREATE NONCLUSTERED INDEX [IX_CHL_CHN_ID] ON [dbo].[TS_CHANNEL_LANGUAGE] ([CHL_CHN_ID] ASC)
	WITH (
			PAD_INDEX = OFF
			,STATISTICS_NORECOMPUTE = OFF
			,SORT_IN_TEMPDB = OFF
			,DROP_EXISTING = OFF
			,ONLINE = OFF
			,ALLOW_ROW_LOCKS = ON
			,ALLOW_PAGE_LOCKS = ON
			) ON [PRIMARY]
GO


BEGIN
	INSERT INTO TS_CHANNEL (CHN_CODE,CHN_NAME)
	VALUES ('TECHNICAL','Technical')

	INSERT INTO TS_CHANNEL (CHN_CODE,CHN_NAME)
	VALUES ('BUSINESS','Business')
END

CREATE TABLE TM_TABLE_SUBSCRIPTION (
	TSB_ID INT IDENTITY(1, 1) NOT NULL
	,TSB_TABLE NVARCHAR(20) NOT NULL
	,TSB_USR_ID INT NOT NULL
	,TSB_DELETE_FLAG BIT NOT NULL CONSTRAINT [PK_TD_TABLE_SUBSCRIPTION] PRIMARY KEY CLUSTERED ([TSB_ID] ASC) WITH (
		PAD_INDEX = OFF
		,STATISTICS_NORECOMPUTE = OFF
		,IGNORE_DUP_KEY = OFF
		,ALLOW_ROW_LOCKS = ON
		,ALLOW_PAGE_LOCKS = ON
		) ON [PRIMARY]
	) ON [PRIMARY]
GO

ALTER TABLE [dbo].TM_TABLE_SUBSCRIPTION
	WITH CHECK ADD CONSTRAINT [FK_TM_TABLE_SUBSCRIPTION_TD_USER] FOREIGN KEY ([TSB_USR_ID]) REFERENCES [dbo].TD_USER([USR_ID])
GO

ALTER TABLE [dbo].TM_TABLE_SUBSCRIPTION CHECK CONSTRAINT [FK_TM_TABLE_SUBSCRIPTION_TD_USER]
GO



CREATE NONCLUSTERED INDEX [IX_TSB_TABLE] ON [dbo].[TM_TABLE_SUBSCRIPTION] ([TSB_TABLE] ASC)
	WITH (
			PAD_INDEX = OFF
			,STATISTICS_NORECOMPUTE = OFF
			,SORT_IN_TEMPDB = OFF
			,DROP_EXISTING = OFF
			,ONLINE = OFF
			,ALLOW_ROW_LOCKS = ON
			,ALLOW_PAGE_LOCKS = ON
			) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_TSB_USR_ID] ON [dbo].[TM_TABLE_SUBSCRIPTION] ([TSB_USR_ID] ASC)
	WITH (
			PAD_INDEX = OFF
			,STATISTICS_NORECOMPUTE = OFF
			,SORT_IN_TEMPDB = OFF
			,DROP_EXISTING = OFF
			,ONLINE = OFF
			,ALLOW_ROW_LOCKS = ON
			,ALLOW_PAGE_LOCKS = ON
			) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_TSB_DELETE_FLAG] ON [dbo].[TM_TABLE_SUBSCRIPTION] ([TSB_DELETE_FLAG] ASC)
	WITH (
			PAD_INDEX = OFF
			,STATISTICS_NORECOMPUTE = OFF
			,SORT_IN_TEMPDB = OFF
			,DROP_EXISTING = OFF
			,ONLINE = OFF
			,ALLOW_ROW_LOCKS = ON
			,ALLOW_PAGE_LOCKS = ON
			) ON [PRIMARY]
GO



CREATE TABLE TM_CHANNEL_SUBSCRIPTION (
	CSB_ID [int] IDENTITY(1, 1) NOT NULL
	,CSB_CHN_ID INT NOT NULL
	,CSB_USR_ID INT NOT NULL
	,CSB_DELETE_FLAG BIT NOT NULL CONSTRAINT [PK_TD_DOMAIN_SUBSCRIPTION] PRIMARY KEY CLUSTERED ([CSB_ID] ASC) WITH (
		PAD_INDEX = OFF
		,STATISTICS_NORECOMPUTE = OFF
		,IGNORE_DUP_KEY = OFF
		,ALLOW_ROW_LOCKS = ON
		,ALLOW_PAGE_LOCKS = ON
		) ON [PRIMARY]
	) ON [PRIMARY]
GO

ALTER TABLE [dbo].TM_CHANNEL_SUBSCRIPTION
	WITH CHECK ADD CONSTRAINT [FK_TM_CHANNEL_SUBSCRIPTION_TS_CHANNEL] FOREIGN KEY ([CSB_CHN_ID]) REFERENCES [dbo].TS_CHANNEL([CHN_ID])
GO

ALTER TABLE [dbo].TM_CHANNEL_SUBSCRIPTION CHECK CONSTRAINT [FK_TM_CHANNEL_SUBSCRIPTION_TS_CHANNEL]
GO

ALTER TABLE [dbo].TM_CHANNEL_SUBSCRIPTION
	WITH CHECK ADD CONSTRAINT [FK_TD_CHANNEL_SUBSCRIPTION_TD_USER] FOREIGN KEY ([CSB_USR_ID]) REFERENCES [dbo].TD_USER([USR_ID])
GO

ALTER TABLE [dbo].TM_CHANNEL_SUBSCRIPTION CHECK CONSTRAINT [FK_TD_CHANNEL_SUBSCRIPTION_TD_USER]
GO



CREATE NONCLUSTERED INDEX [IX_DSB_DMN_ID] ON [dbo].TM_CHANNEL_SUBSCRIPTION ([CSB_CHN_ID] ASC)
	WITH (
			PAD_INDEX = OFF
			,STATISTICS_NORECOMPUTE = OFF
			,SORT_IN_TEMPDB = OFF
			,DROP_EXISTING = OFF
			,ONLINE = OFF
			,ALLOW_ROW_LOCKS = ON
			,ALLOW_PAGE_LOCKS = ON
			) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_CSB_USR_ID] ON [dbo].TM_CHANNEL_SUBSCRIPTION ([CSB_USR_ID] ASC)
	WITH (
			PAD_INDEX = OFF
			,STATISTICS_NORECOMPUTE = OFF
			,SORT_IN_TEMPDB = OFF
			,DROP_EXISTING = OFF
			,ONLINE = OFF
			,ALLOW_ROW_LOCKS = ON
			,ALLOW_PAGE_LOCKS = ON
			) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_CSB_DELETE_FLAG] ON [dbo].TM_CHANNEL_SUBSCRIPTION ([CSB_DELETE_FLAG] ASC)
	WITH (
			PAD_INDEX = OFF
			,STATISTICS_NORECOMPUTE = OFF
			,SORT_IN_TEMPDB = OFF
			,DROP_EXISTING = OFF
			,ONLINE = OFF
			,ALLOW_ROW_LOCKS = ON
			,ALLOW_PAGE_LOCKS = ON
			) ON [PRIMARY]
GO


CREATE TABLE TD_SUBSCRIBER (
	SBR_ID INT IDENTITY(1, 1) NOT NULL
	,SBR_USR_ID INT NOT NULL
	,SBR_UID NVARCHAR(256) NOT NULL
	,SBR_PREFERENCE NVARCHAR(MAX)
	,SBR_KEY NVARCHAR(256) NOT NULL 
	,SBR_DELETE_FLAG BIT NOT NULL CONSTRAINT [PK_TD_SUBSCRIBER] PRIMARY KEY CLUSTERED ([SBR_ID] ASC) WITH (
		PAD_INDEX = OFF
		,STATISTICS_NORECOMPUTE = OFF
		,IGNORE_DUP_KEY = OFF
		,ALLOW_ROW_LOCKS = ON
		,ALLOW_PAGE_LOCKS = ON
		) ON [PRIMARY]
	) ON [PRIMARY]

ALTER TABLE [dbo].TD_SUBSCRIBER
	WITH CHECK ADD CONSTRAINT [FK_TD_SUBSCRIBER_TD_USER] FOREIGN KEY ([SBR_USR_ID]) REFERENCES [dbo].TD_USER([USR_ID])
GO

ALTER TABLE [dbo].TD_SUBSCRIBER CHECK CONSTRAINT [FK_TD_SUBSCRIBER_TD_USER]
GO

CREATE NONCLUSTERED INDEX [IX_SBR_USR_ID] ON [dbo].TD_SUBSCRIBER (SBR_USR_ID ASC)
	WITH (
			PAD_INDEX = OFF
			,STATISTICS_NORECOMPUTE = OFF
			,SORT_IN_TEMPDB = OFF
			,DROP_EXISTING = OFF
			,ONLINE = OFF
			,ALLOW_ROW_LOCKS = ON
			,ALLOW_PAGE_LOCKS = ON
			) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX IX_SBR_UID ON [dbo].TD_SUBSCRIBER (SBR_UID ASC)
	WITH (
			PAD_INDEX = OFF
			,STATISTICS_NORECOMPUTE = OFF
			,SORT_IN_TEMPDB = OFF
			,DROP_EXISTING = OFF
			,ONLINE = OFF
			,ALLOW_ROW_LOCKS = ON
			,ALLOW_PAGE_LOCKS = ON
			) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX IX_SBR_KEY ON [dbo].TD_SUBSCRIBER (SBR_KEY ASC)
	WITH (
			PAD_INDEX = OFF
			,STATISTICS_NORECOMPUTE = OFF
			,SORT_IN_TEMPDB = OFF
			,DROP_EXISTING = OFF
			,ONLINE = OFF
			,ALLOW_ROW_LOCKS = ON
			,ALLOW_PAGE_LOCKS = ON
			) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX IX_SBR_DELETE_FLAG ON [dbo].TD_SUBSCRIBER (SBR_DELETE_FLAG ASC)
	WITH (
			PAD_INDEX = OFF
			,STATISTICS_NORECOMPUTE = OFF
			,SORT_IN_TEMPDB = OFF
			,DROP_EXISTING = OFF
			,ONLINE = OFF
			,ALLOW_ROW_LOCKS = ON
			,ALLOW_PAGE_LOCKS = ON
			) ON [PRIMARY]
GO

CREATE TABLE TD_USER_QUERY (
	SQR_ID INT IDENTITY(1, 1) NOT NULL
	,SQR_SNIPPET_TYPE NVARCHAR(256) NOT NULL
	,SQR_SNIPPET_TAGNAME NVARCHAR(256) NOT NULL
	,SQR_SNIPPET_MATRIX NVARCHAR(256) NOT NULL
	,SQR_SNIPPET_QUERY VARBINARY(MAX) NOT NULL
	,SQR_SNIPPET_ISOGRAM NVARCHAR(256) NOT NULL
	,SQR_USR_ID INT NOT NULL CONSTRAINT [PK_TD_USER_QUERY] PRIMARY KEY CLUSTERED ([SQR_ID] ASC) WITH (
		PAD_INDEX = OFF
		,STATISTICS_NORECOMPUTE = OFF
		,IGNORE_DUP_KEY = OFF
		,ALLOW_ROW_LOCKS = ON
		,ALLOW_PAGE_LOCKS = ON
		) ON [PRIMARY]
	) ON [PRIMARY]

ALTER TABLE [dbo].TD_USER_QUERY
	WITH CHECK ADD CONSTRAINT [TD_USER_QUERY_TD_USER] FOREIGN KEY ([SQR_USR_ID]) REFERENCES [dbo].TD_USER([USR_ID])
GO

ALTER TABLE [dbo].TD_USER_QUERY CHECK CONSTRAINT [TD_USER_QUERY_TD_USER]
GO

CREATE NONCLUSTERED INDEX IX_SQR_USR_ID ON [dbo].TD_USER_QUERY (SQR_USR_ID ASC)
	WITH (
			PAD_INDEX = OFF
			,STATISTICS_NORECOMPUTE = OFF
			,SORT_IN_TEMPDB = OFF
			,DROP_EXISTING = OFF
			,ONLINE = OFF
			,ALLOW_ROW_LOCKS = ON
			,ALLOW_PAGE_LOCKS = ON
			) ON [PRIMARY]
GO

ALTER TABLE TD_ANALYTIC ADD NLT_WIDGET BIT
GO

UPDATE TD_ANALYTIC
SET NLT_WIDGET = 0
GO

ALTER TABLE TD_ANALYTIC

ALTER COLUMN NLT_WIDGET BIT NOT NULL
GO

CREATE NONCLUSTERED INDEX IX_NLT_WIDGET ON [dbo].TD_ANALYTIC (NLT_WIDGET ASC)
	WITH (
			PAD_INDEX = OFF
			,STATISTICS_NORECOMPUTE = OFF
			,SORT_IN_TEMPDB = OFF
			,DROP_EXISTING = OFF
			,ONLINE = OFF
			,ALLOW_ROW_LOCKS = ON
			,ALLOW_PAGE_LOCKS = ON
			) ON [PRIMARY]
GO
