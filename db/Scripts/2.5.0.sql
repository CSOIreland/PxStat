
-- drop Jobs and dependencies

-- drop StoredProcedures and dependencies


-- drop Views and dependencies

-- drop Types and dependencies

-- alter database structure
ALTER TABLE TD_ANALYTIC
ADD NLT_LNG_ISO_CODE CHAR(2);

CREATE NONCLUSTERED INDEX [IX_NLT_LNG_ISO_CODE] ON [dbo].[TD_ANALYTIC]
(
	[NLT_LNG_ISO_CODE] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

-- alter database data 

UPDATE TS_FORMAT
SET FRM_VERSION='1.1'
WHERE FRM_TYPE='JSON-stat'
AND FRM_VERSION='1.0'