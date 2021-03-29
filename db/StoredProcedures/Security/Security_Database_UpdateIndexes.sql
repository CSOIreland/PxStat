SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date:10/03/2021
-- Description:	Checks index fragmentation and automatically reorganizes any indexes with more than a set percentage amount of fragmenation
-- exec Security_Database_UpdateIndexes
-- =============================================
CREATE
	OR

ALTER PROCEDURE Security_Database_UpdateIndexes
WITH EXECUTE AS OWNER  
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @FRAGMENTATION_LIMIT INT
	DECLARE @TableName NVARCHAR(128)
	DECLARE @MIN_ROW_COUNT INT

	SET @FRAGMENTATION_LIMIT = 51

	DECLARE table_cursor CURSOR
	FOR
	SELECT DISTINCT OBJECT_NAME(ips.OBJECT_ID) AS 'table'
	FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'SAMPLED') ips
	INNER JOIN sys.indexes i ON ips.object_id = i.object_id
		AND ips.index_id = i.index_id
		AND ips.avg_fragmentation_in_percent > @FRAGMENTATION_LIMIT

	OPEN table_cursor

	FETCH NEXT
	FROM table_cursor
	INTO @TableName

	INSERT INTO TD_LOGGING (
				LGG_DATETIME
				,LGG_THREAD
				,LGG_LEVEL
				,LGG_CLASS
				,LGG_METHOD
				,LGG_MESSAGE
				)
			VALUES (
				getdate()
				,'0'
				,'INFO'
				,'SQL SERVER AGENT'
				,'Security_Database_UpdateIndexes'
				,convert(varchar(256),@@CURSOR_ROWS) + ' tables with Indexes to reorganize'
				)

	WHILE @@FETCH_STATUS = 0
	BEGIN

	BEGIN TRY
			

			EXEC Security_Database_UpdateStatistic @TableName

			INSERT INTO TD_LOGGING (
				LGG_DATETIME
				,LGG_THREAD
				,LGG_LEVEL
				,LGG_CLASS
				,LGG_METHOD
				,LGG_MESSAGE
				)
			VALUES (
				getdate()
				,'0'
				,'INFO'
				,'SQL SERVER AGENT'
				,'Security_Database_UpdateIndexes'
				,'Indexes reorganized for table ' + @TableName
				)
	

		SELECT @TableName

		FETCH NEXT
		FROM table_cursor
		INTO @TableName
	END TRY
	BEGIN CATCH
		INSERT INTO TD_LOGGING (
				LGG_DATETIME
				,LGG_THREAD
				,LGG_LEVEL
				,LGG_CLASS
				,LGG_METHOD
				,LGG_MESSAGE
				)
			VALUES (
				getdate()
				,'0'
				,'INFO'
				,'SQL SERVER AGENT'
				,'Security_Database_UpdateIndexes'
				,'Error reorganizing Indexes for table ' + @TableName + ': ' + ERROR_MESSAGE() 
				)
	END CATCH
	END

	CLOSE table_cursor

	DEALLOCATE table_cursor


	EXEC sp_updatestats;  
		INSERT INTO TD_LOGGING (
				LGG_DATETIME
				,LGG_THREAD
				,LGG_LEVEL
				,LGG_CLASS
				,LGG_METHOD
				,LGG_MESSAGE
				)
			VALUES (
				getdate()
				,'0'
				,'INFO'
				,'SQL SERVER AGENT'
				,'Security_Database_UpdateIndexes'
				,'Statistics updated' 
				)

END
GO


