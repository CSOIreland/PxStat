IF EXISTS (
		SELECT *
		FROM sys.types
		WHERE is_table_type = 1
			AND name = 'KeyValueVarchar'
		)
BEGIN
	IF EXISTS (
			SELECT *
			FROM sys.objects
			WHERE object_id = OBJECT_ID(N'Data_Matrix_ReadDataByRelease')
			)
	BEGIN
		DROP PROCEDURE Data_Matrix_ReadDataByRelease
	END
	IF EXISTS (
			SELECT *
			FROM sys.objects
			WHERE object_id = OBJECT_ID(N'System_Navigation_Search')
			)
	BEGIN
		DROP PROCEDURE System_Navigation_Search
	END

	DROP TYPE [KeyValueVarchar]
END

CREATE TYPE [dbo].[KeyValueVarchar] AS TABLE (
	[Key] [nvarchar](256) NULL,
	[Value] [nvarchar](256) NULL
	)
GO


