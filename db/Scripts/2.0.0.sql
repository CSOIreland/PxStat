
-- drop Jobs and dependencies

-- drop StoredProcedures and dependencies

-- drop Views and dependencies

-- drop Types and dependencies

-- alter database structure

-- [ENHANCEMENT] Make comment on "PxStat.Data.ReasonRelease_API.Create" optional #88
ALTER TABLE TM_REASON_RELEASE
ALTER COLUMN RSR_CMM_ID INT NULL;

-- alter database data