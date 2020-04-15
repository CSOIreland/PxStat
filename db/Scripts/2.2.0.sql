
-- drop Jobs and dependencies

-- drop StoredProcedures and dependencies

-- drop Views and dependencies

-- drop Types and dependencies

-- alter database structure

-- [ENHANCEMENT] Change Emergency Release to Exceptional Release #97
EXEC sp_rename 'TD_RELEASE.RLS_EMERGENCY_FLAG', 'RLS_EXCEPTIONAL_FLAG', 'COLUMN';  
EXEC sp_rename 'TD_RELEASE.IX_RLS_EMERGENCY_FLAG', 'IX_RLS_EXCEPTIONAL_FLAG', 'INDEX';  
EXEC sp_rename 'TD_WORKFLOW_REQUEST.WRQ_EMERGENCY_FLAG', 'WRQ_EXCEPTIONAL_FLAG', 'COLUMN';  
EXEC sp_rename 'TD_WORKFLOW_REQUEST.IX_WRQ_EMERGENCY_FLAG', 'IX_WRQ_EXCEPTIONAL_FLAG', 'INDEX';    

-- alter database data
INSERT [dbo].[TS_FORMAT] ([FRM_TYPE], [FRM_VERSION], [FRM_DIRECTION]) VALUES (N'XLSX', N'2007', 'DOWNLOAD') 