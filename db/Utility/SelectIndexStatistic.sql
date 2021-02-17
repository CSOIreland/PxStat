-- Select All Index Statistics
SELECT 
	 OBJECT_NAME(ips.OBJECT_ID) AS 'table'
	,i.NAME AS 'index'
	,ips.index_type_desc as 'type'
	,ips.record_count as 'count'
	,ROUND(ips.avg_record_size_in_bytes * ips.record_count, 0) AS 'size'
	,ROUND(ips.avg_fragmentation_in_percent, 0) AS 'fragmentation'
FROM 
sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'SAMPLED') ips
INNER JOIN sys.indexes i
	ON ips.object_id = i.object_id
		AND ips.index_id = i.index_id
