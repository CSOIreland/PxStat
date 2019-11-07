USE [msdb]
GO
CREATE USER [pxstat] FOR LOGIN [pxstat] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [SQLAgentOperatorRole] ADD MEMBER [pxstat]
GO
CREATE ROLE [db_executor]
GO
GRANT EXECUTE TO [db_executor]
GO
ALTER ROLE [db_executor] ADD MEMBER [pxstat]
GO
GRANT SELECT ON [sysjobactivity] TO [pxstat]
GO
