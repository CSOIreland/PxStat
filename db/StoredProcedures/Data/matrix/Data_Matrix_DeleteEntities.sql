SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 10/07/2019
-- Description:	Hard deletes all data associated with all soft deleted matrixes
-- exec Data_Matrix_Delete Data_Matrix_DeleteEntities
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Matrix_DeleteEntities
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @MatrixIds TABLE (
		MtrId INT
		,MtrDtgId INT
		,Rownum INT
		)

	INSERT INTO @MatrixIds
	SELECT MTR_Id
		,MTR_DTG_ID
		,ROW_NUMBER() OVER (
			ORDER BY MTR_ID
			) AS RowNum
	FROM TD_MATRIX
	WHERE MTR_DELETE_FLAG = 1

	IF (
			SELECT count(*)
			FROM @matrixIds
			) = 0
	BEGIN
		RETURN 0
	END

	DELETE TM_DATA_CELL
	FROM TM_DATA_CELL
	INNER JOIN @MatrixIds
		ON MtrId = DTC_MTR_ID

	DELETE TD_DATA
	FROM TD_DATA
	INNER JOIN @MatrixIds
		ON MtrId = TDT_MTR_ID

	DELETE TD_VARIABLE
	FROM TD_VARIABLE
	INNER JOIN TD_CLASSIFICATION
		ON VRB_CLS_ID = CLS_ID
	INNER JOIN @MatrixIds
		ON MtrId = CLS_MTR_ID

	DELETE TD_STATISTIC
	FROM TD_STATISTIC
	INNER JOIN @MatrixIds
		ON MtrId = STT_MTR_ID

	DELETE TD_CLASSIFICATION
	FROM TD_CLASSIFICATION
	INNER JOIN @MatrixIds
		ON MtrId = CLS_MTR_ID

	DELETE TD_PERIOD
	FROM TD_PERIOD
	INNER JOIN TD_FREQUENCY
		ON PRD_FRQ_ID = FRQ_ID
	INNER JOIN @MatrixIds
		ON MtrId = FRQ_MTR_ID

	DELETE TD_FREQUENCY
	FROM TD_FREQUENCY
	INNER JOIN @MatrixIds
		ON MtrId = FRQ_MTR_ID

	RETURN 0
END
GO


