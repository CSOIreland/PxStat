-- drop Jobs and dependencies
-- drop StoredProcedures and dependencies
-- drop Views and dependencies
-- drop Types and dependencies
-- alter database structure

--Create a locking mechanism for data tables
CREATE TABLE [dbo].[TD_DATASET] (
	[DTT_ID] [int] IDENTITY(1, 1) NOT NULL
	,[DTT_MTR_CODE] NVARCHAR(20) NOT NULL UNIQUE
	,[DTT_DATETIME_LOCKED] DATETIME NULL
	,CONSTRAINT [PK_DTT_ID] PRIMARY KEY CLUSTERED ([DTT_ID] ASC) WITH (
		PAD_INDEX = OFF
		,STATISTICS_NORECOMPUTE = OFF
		,IGNORE_DUP_KEY = OFF
		,ALLOW_ROW_LOCKS = ON
		,ALLOW_PAGE_LOCKS = ON
		) ON [PRIMARY]
	) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_DTT_MTR_CODE] ON [dbo].[TD_DATASET] ([DTT_MTR_CODE] ASC)
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



	-- alter database data 
INSERT INTO TD_DATASET (DTT_MTR_CODE) (SELECT DISTINCT MTR_CODE FROM TD_MATRIX WHERE MTR_DELETE_FLAG = 0)

GO