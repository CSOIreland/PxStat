-- Add Schedules
USE [msdb]
GO


EXEC sp_add_schedule  
    @schedule_name = N'DataMaintenance_$(DB_DATA)' ,  
    @freq_type = 4,  
    @freq_interval = 1,  
    @active_start_time = 012000 ;  
GO  
  
EXEC sp_attach_schedule  
   @job_name = N'DataMatrixDeleteEntities_$(DB_DATA)',  
   @schedule_name = N'DataMaintenance_$(DB_DATA)' ;  
GO  