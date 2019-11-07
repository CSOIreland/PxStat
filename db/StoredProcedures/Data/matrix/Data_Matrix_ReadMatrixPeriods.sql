SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 14/06/2019
-- Description:	Reads the period data for a matrix id
-- exec Data_Matrix_ReadMatrixPeriods 4
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Matrix_ReadMatrixPeriods @MatrixId INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT PRD_CODE AS PrdCode
		,PRD_FRQ_ID AS PrdFrqId
		,PRD_ID AS PrdId
		,PRD_VALUE AS PrdValue
		,FRQ_CODE AS FrqCode
	FROM TD_PERIOD
	INNER JOIN TD_FREQUENCY
		ON PRD_FRQ_ID = FRQ_ID
	INNER JOIN TD_MATRIX
		ON FRQ_MTR_ID = MTR_ID
			AND MTR_DELETE_FLAG = 0
	WHERE MTR_ID = @MatrixId
END
GO


