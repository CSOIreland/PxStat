IF EXISTS (
		SELECT *
		FROM sys.types
		WHERE is_table_type = 1
			AND name = 'ValueVarchar'
		)
BEGIN
	IF EXISTS (
			SELECT *
			FROM sys.objects
			WHERE object_id = OBJECT_ID(N'System_Navigation_Search')
			)
	BEGIN
		DROP PROCEDURE System_Navigation_Search
	END

	DROP TYPE [ValueVarchar]
END

CREATE TYPE [dbo].[ValueVarchar] AS TABLE ([Value] [varchar](256) NULL)
GO


