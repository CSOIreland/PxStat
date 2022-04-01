-- Drop Schedules
USE [msdb]
GO

DECLARE @sID INT
SET @sID=(SELECT SCHEDULE_ID FROM sysschedules WHERE name=N'DataMaintenance_$(DB_DATA)');

IF @sID IS NOT NULL
BEGIN
EXEC dbo.sp_delete_schedule  
    @schedule_name =N'DataMaintenance_$(DB_DATA)';  
END