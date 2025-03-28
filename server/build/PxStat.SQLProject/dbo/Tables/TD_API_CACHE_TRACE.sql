﻿CREATE TABLE [dbo].[TD_API_CACHE_TRACE](
	[CCH_ID] [int] IDENTITY(1,1) NOT NULL,
	[CCH_CORRELATION_ID] varchar(256) NULL,
	[CCH_OBJECT] VARCHAR(MAX) NULL,
	[CCH_START_TIME] datetime,
	[CCH_DURATION] decimal(10,3) NULL,
	[CCH_ACTION] varchar(2048) NULL,
	[CCH_SUCCESS] bit,
	[CCH_COMPRESSED_SIZE] int,	
	[CCH_EXPIRES_AT] datetime,
    PRIMARY KEY CLUSTERED ([CCH_ID] ASC), 
)
