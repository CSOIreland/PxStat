
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 26/01/2022
-- Description:	For a matrix id, read the single field data
-- =============================================
CREATE
	

 PROCEDURE Data_Matrix_ReadDataField @MtrId INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT MTD_DATA AS MtdData
	FROM TD_MATRIX_DATA
	WHERE MTD_MTR_ID = @MtrId
END
