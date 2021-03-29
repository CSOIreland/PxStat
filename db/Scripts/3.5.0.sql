

-- drop Jobs and dependencies

-- drop StoredProcedures and dependencies

-- drop Views and dependencies

-- drop Types and dependencies

-- alter database structure

--[BUG] Intermittent row locked error when trying to extend session #379
CREATE NONCLUSTERED INDEX [IX_LGN_CCN_ID] ON [dbo].[TD_LOGIN]
(
	[LGN_CCN_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
 GO
