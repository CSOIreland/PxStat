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
	DECLARE @rowcount INT

	INSERT INTO @MatrixIds
	SELECT MTR_Id
	FROM TD_MATRIX
	WHERE MTR_DELETE_FLAG = 1

	SET @rowcount = (
			SELECT count(*)
			FROM @matrixIds
			)

	IF @rowcount = 0
	BEGIN
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
			,'No Matrix data to delete'
			)

		RETURN 0
	END

	DELETE TM_DATA_CELL
	FROM TM_DATA_CELL
	INNER JOIN @MatrixIds ON MtrId = DTC_MTR_ID

	DELETE TD_DATA
	FROM TD_DATA
	INNER JOIN @MatrixIds ON MtrId = TDT_MTR_ID

	DELETE TD_VARIABLE
	FROM TD_VARIABLE
	INNER JOIN TD_CLASSIFICATION ON VRB_CLS_ID = CLS_ID
	INNER JOIN @MatrixIds ON MtrId = CLS_MTR_ID

	DELETE TD_STATISTIC
	FROM TD_STATISTIC
	INNER JOIN @MatrixIds ON MtrId = STT_MTR_ID

	DELETE TD_CLASSIFICATION
	FROM TD_CLASSIFICATION
	INNER JOIN @MatrixIds ON MtrId = CLS_MTR_ID

	DELETE TD_PERIOD
	FROM TD_PERIOD
	INNER JOIN TD_FREQUENCY ON PRD_FRQ_ID = FRQ_ID
	INNER JOIN @MatrixIds ON MtrId = FRQ_MTR_ID

	DELETE TD_FREQUENCY
	FROM TD_FREQUENCY
	INNER JOIN @MatrixIds ON MtrId = FRQ_MTR_ID

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
		,'Data for ' + convert(varchar(256),@rowcount) + ' Matrixes deleted'
		)
		
	RETURN 0
END
GO


