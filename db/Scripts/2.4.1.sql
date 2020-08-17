
-- drop Jobs and dependencies

-- drop StoredProcedures and dependencies


-- drop Views and dependencies

-- drop Types and dependencies

-- alter database structure

-- alter database data 
-- [BUG] JSON-stat 1.0 missing #177
INSERT [dbo].[TS_FORMAT] ([FRM_TYPE], [FRM_VERSION], [FRM_DIRECTION]) VALUES (N'JSON-stat', N'1.0', 'DOWNLOAD')