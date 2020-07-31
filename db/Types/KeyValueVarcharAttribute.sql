IF EXISTS (
		SELECT *
		FROM sys.types
		WHERE is_table_type = 1
			AND name = 'KeyValueVarcharAttribute'
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


	DROP TYPE [KeyValueVarcharAttribute]
END

CREATE TYPE [dbo].[KeyValueVarcharAttribute] AS TABLE (
	[Key] [nvarchar](256) NULL,
	[Value] [nvarchar](256) NULL,
	[Attribute] [int] NULL DEFAULT ((1))
	)
GO


