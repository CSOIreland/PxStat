SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- Author:		Neil O'Keeffe
-- Create date:10/03/2021
-- Description:	Checks index fragmentation and automatically reorganizes any indexes with more than a set percentage amount of fragmenation
-- exec Security_Database_ReorganizeFragmented
CREATE
	OR

ALTER PROCEDURE [dbo].[Security_Database_ReorganizeFragmented]
	WITH EXECUTE AS OWNER
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @FRAGMENTATION_LIMIT AS INT;
	DECLARE @TableName AS NVARCHAR(128);
	DECLARE @MIN_ROW_COUNT AS INT;

	SET @FRAGMENTATION_LIMIT = 51;

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
		,'Security_Database_ReorganizeFragmented'
		,'Starting defragmentation'
		);

	DECLARE table_cursor CURSOR
	FOR
	SELECT DISTINCT OBJECT_NAME(ips.OBJECT_ID) AS 'table'
	FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'SAMPLED') AS ips
	INNER JOIN sys.indexes AS i
		ON ips.object_id = i.object_id
			AND ips.index_id = i.index_id
			AND ips.avg_fragmentation_in_percent > @FRAGMENTATION_LIMIT;

	OPEN table_cursor;

	FETCH NEXT
	FROM table_cursor
	INTO @TableName;

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
		,'Security_Database_ReorganizeFragmented'
		,'Starting index defragmentation for ' + CONVERT(VARCHAR(256), @@CURSOR_ROWS) + ' tables'
		);

	WHILE @@FETCH_STATUS = 0
	BEGIN
		BEGIN TRY
			EXECUTE Security_Database_UpdateStatistic @TableName;

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
				,'Security_Database_ReorganizeFragmented'
				,'INDEXES DEFRAGMENTED for ' + @TableName
				);

			SELECT @TableName;

			FETCH NEXT
			FROM table_cursor
			INTO @TableName;
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
				,'Security_Database_ReorganizeFragmented'
				,'Error amending indexes for ' + @TableName + ' ' + ERROR_MESSAGE()
				);
		END CATCH
	END

	CLOSE table_cursor;

	DEALLOCATE table_cursor;

	EXECUTE sp_updatestats;

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
		,'Security_Database_ReorganizeFragmented'
		,'Statistics Updated'
		);
END
