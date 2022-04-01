-- Drop Jobs
USE [msdb]
GO


DECLARE @jID VARCHAR(256)
SET @jID=(SELECT job_id FROM msdb.dbo.sysjobs where name=N'DataMatrixDeleteEntities_$(DB_DATA)')

IF @jID IS NOT NULL
BEGIN
EXEC msdb.dbo.sp_delete_job @job_id= @jID, @delete_unused_schedule=1
END

SET @jID=(SELECT job_id FROM msdb.dbo.sysjobs where name=N'DataMatrixUpdateIndexes_$(DB_DATA)')

IF @jID IS NOT NULL
BEGIN
	EXEC msdb.dbo.sp_delete_job @job_id= @jID, @delete_unused_schedule=1
END
GO