

  declare @counter int
  declare @total int
  set @counter=0
  declare @asvid int

  select  q.* ,ROW_NUMBER() OVER (ORDER BY api_asv_id) AS Row_Counter into #tmp from
  (
  select distinct api_asv_id
  from TS_API_SETTING 
  ) q

  select @total =(select max(row_counter) from #tmp)

  while @counter<@total
  begin
	set @counter=@counter + 1
	set @asvid =(select api_asv_id from #tmp where Row_Counter=@counter)
	insert into ts_api_setting (API_ASV_ID,API_KEY, API_VALUE,API_DESCRIPTION )
	values(@asvid,'API_MEMCACHED_CAS_FLUSH_TIMEOUT','60','Max number of seconds to wait for a CAS flush')

  end

  drop table #tmp
