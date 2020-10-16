
-- drop Jobs and dependencies

-- drop StoredProcedures and dependencies


-- drop Views and dependencies

-- drop Types and dependencies

-- alter database structure

-- [ENHANCEMENT] Remove dependency flag #227
ALTER TABLE [dbo].[TD_RELEASE] DROP CONSTRAINT [DF_TD_RELEASE_RLS_DEPENDENCY_FLAG]
GO
DROP INDEX [IX_RLS_DEPENDENCY_FLAG] ON [dbo].[TD_RELEASE]
GO
ALTER TABLE TD_RELEASE
DROP COLUMN RLS_DEPENDENCY_FLAG
GO

-- [BUG] Issues building Release 2.6.0 #246
EXEC sp_rename 'dbo.DF_TD_RELEASE_RLS_EMERGENCY_FLAG', 'DF_TD_RELEASE_RLS_EXCEPTIONAL_FLAG', 'OBJECT'; 

-- alter database data 