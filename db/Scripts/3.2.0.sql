
-- drop Jobs and dependencies

-- drop StoredProcedures and dependencies


-- drop Views and dependencies

-- drop Types and dependencies

-- alter database structure


	
-- alter database data 

-- [ENHANCEMENT] Remove n/a from NLT_REFERER and replace with null where it's found #326
UPDATE TD_ANALYTIC
SET NLT_REFERER = NULL
WHERE NLT_REFERER = 'n/a'

