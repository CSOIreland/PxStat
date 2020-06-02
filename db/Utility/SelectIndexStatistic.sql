-- Select All Index Statistics
SELECT 'SAMPLED' AS 'statistic'
	,OBJECT_NAME(ips.OBJECT_ID) AS 'table'
	,i.NAME AS 'index'
	,ips.index_type_desc
	,ips.record_count
	,ROUND(ips.avg_record_size_in_bytes * ips.record_count / 1024 / 1024, 0) AS avg_table_size_in_megabytes
	,ROUND(ips.avg_fragmentation_in_percent, 1) AS avg_fragmentation_in_percent
FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'SAMPLED') ips
INNER JOIN sys.indexes i
	ON (ips.object_id = i.object_id)
		AND (ips.index_id = i.index_id)
ORDER BY record_count DESC
