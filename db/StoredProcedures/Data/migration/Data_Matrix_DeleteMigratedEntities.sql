SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 13/04/2022
-- Description:	Hard deletes all data associated with all migrated matrixes
-- exec Data_Matrix_DeleteMigratedEntities
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Matrix_DeleteMigratedEntities
AS
BEGIN
	SET NOCOUNT ON;
	SET IMPLICIT_TRANSACTIONS OFF;

	DECLARE @MatrixIds TABLE (MtrId INT)

	INSERT INTO @MatrixIds
	SELECT MTR_Id
	FROM TD_MATRIX
	WHERE MTR_MIGRATION_FLAG IS NOT NULL

	TRUNCATE TABLE TM_DATA_CELL

	TRUNCATE TABLE TD_DATA

	TRUNCATE TABLE TD_PERIOD

	TRUNCATE TABLE TD_FREQUENCY

	TRUNCATE TABLE TD_VARIABLE

	TRUNCATE TABLE TD_CLASSIFICATION

	TRUNCATE TABLE TD_STATISTIC

	RETURN (
			SELECT COUNT(*)
			FROM @MatrixIds
			)
END
GO


