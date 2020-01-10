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
			WHERE object_id = OBJECT_ID(N'Data_Classification_Search')
			)
	BEGIN
		DROP PROCEDURE Data_Classification_Search
	END
	
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
			WHERE object_id = OBJECT_ID(N'Security_GroupAccount_ReadMultiple')
			)
	BEGIN
		DROP PROCEDURE Security_GroupAccount_ReadMultiple
	END

	DROP TYPE [ValueVarchar]
END

CREATE TYPE [dbo].[ValueVarchar] AS TABLE ([Value] [nvarchar](256) NULL)
GO
