
DROP INDEX td_matrix.IX_MTR_DELETE_FLAG


CREATE NONCLUSTERED INDEX IX_MTR_DELETE_FLAG
ON [dbo].[TD_MATRIX] ([MTR_DELETE_FLAG])
INCLUDE ([MTR_CODE],[MTR_TITLE],[MTR_RLS_ID])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO




