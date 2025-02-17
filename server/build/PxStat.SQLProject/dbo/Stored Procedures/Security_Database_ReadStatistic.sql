
-- =============================================
-- Author:		Liam Millar
-- Create date: 08/03/2021
-- Description:	Returns details of fragmentation for Indices of a specific table or all tables
-- exec Security_Database_ReadStatistic 
-- =============================================
CREATE
	

 PROCEDURE [dbo].[Security_Database_ReadStatistic] @TableName NVARCHAR(128) = NULL
	WITH EXECUTE AS OWNER
AS
BEGIN
	SET NOCOUNT ON;

	IF @TableName IS NULL
	BEGIN
		SELECT OBJECT_NAME(ips.OBJECT_ID) AS 'table'
			,i.NAME AS 'index'
			,ips.index_type_desc AS 'type'
			,ips.record_count AS 'rows'
			,ROUND(ips.avg_record_size_in_bytes * ips.record_count, 0) AS 'size'
			,ROUND(ips.avg_fragmentation_in_percent, 0) AS 'fragmentation'
		FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'SAMPLED') ips
		INNER JOIN sys.indexes i ON ips.object_id = i.object_id
			AND ips.index_id = i.index_id
	END
	ELSE
	BEGIN
		SELECT OBJECT_NAME(ips.OBJECT_ID) AS 'table'
			,i.NAME AS 'index'
			,ips.index_type_desc AS 'type'
			,ips.record_count AS 'rows'
			,ROUND(ips.avg_record_size_in_bytes * ips.record_count, 0) AS 'size'
			,ROUND(ips.avg_fragmentation_in_percent, 0) AS 'fragmentation'
		FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'SAMPLED') ips
		INNER JOIN sys.indexes i ON ips.object_id = i.object_id
			AND ips.index_id = i.index_id
			AND OBJECT_NAME(ips.OBJECT_ID) = @TableName
	END
END
