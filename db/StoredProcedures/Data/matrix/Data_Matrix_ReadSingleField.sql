SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 24/01/2022
-- Description:	Reads data from a single field
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Matrix_ReadSingleField @MtrId INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT MTD_DATA AS MtdData
	FROM TD_MATRIX_DATA
	WHERE MTD_MTR_ID = @MtrId
END
GO


