SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 10/07/2019
-- Description:	Hard deletes all data associated with all soft deleted matrixes
-- exec Data_Matrix_DeleteEntities
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Matrix_DeleteEntities
AS
BEGIN
	SET NOCOUNT ON;
	SET IMPLICIT_TRANSACTIONS OFF;

	DECLARE @MatrixIds TABLE (MtrId INT)
	DECLARE @MdmIds TABLE (MdmId INT)
	DECLARE @StartTime DATETIME

	SET @StartTime = GETDATE()

	DECLARE @DLIMIT INT

	INSERT INTO @MatrixIds
	SELECT MTR_Id
	FROM TD_MATRIX
	WHERE MTR_DELETE_FLAG = 1

	INSERT INTO @MdmIds 
	
		SELECT DISTINCT MDM_ID
		FROM TD_MATRIX_DIMENSION 
		WHERE MDM_MTR_ID IN
		(
			SELECT MtrId from @MatrixIds
		)
	

	DELETE FROM TD_MATRIX_DATA 
	WHERE MTD_MTR_ID IN
	(
	SELECT MtrId from @MatrixIds
	)



	DELETE FROM TD_DIMENSION_ITEM 
	WHERE DMT_MDM_ID IN
	(
		SELECT MdmId from @MdmIds
	)



	DELETE FROM TD_MATRIX_DIMENSION 
	WHERE MDM_ID IN
	(
		SELECT MdmId from @MdmIds
	)


	INSERT INTO TD_LOGGING (
		LGG_DATETIME
		,LGG_THREAD
		,LGG_LEVEL
		,LGG_CLASS
		,LGG_METHOD
		,LGG_MESSAGE
		)
	VALUES (
		getdate()
		,'0'
		,'INFO'
		,'SQL SERVER AGENT'
		,'Data_Matrix_DeleteEntities'
		,'Unused Matrix entities deleted - exec time ' + (CAST(DATEDIFF(millisecond, @StartTime, GETDATE()) AS VARCHAR(256))) + ' ms'
		)

	RETURN 0
END
GO


