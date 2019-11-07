SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 15/07/2019
-- Description:	Tests if a specific job name is running at the moment
-- exec Data_Matrix_IsJobRunning 'DataMatrixDeleteEntities_pxstat.td11'
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Matrix_IsJobRunning @JobName VARCHAR(1024)
AS
BEGIN
	DECLARE @Result BIT

	SET NOCOUNT ON;

	IF NOT EXISTS (
			SELECT 1
			FROM msdb.dbo.sysjobs_view job
			INNER JOIN msdb.dbo.sysjobactivity activity
				ON job.job_id = activity.job_id
			WHERE activity.run_Requested_date IS NOT NULL
				AND activity.stop_execution_date IS NULL
				AND job.name = @JobName
			)
	BEGIN
		SET @Result = 0
	END
	ELSE
	BEGIN
		SET @Result = 1
	END

	RETURN @Result
END
GO


