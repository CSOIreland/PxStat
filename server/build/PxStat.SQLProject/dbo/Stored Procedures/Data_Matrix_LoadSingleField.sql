
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 24/01/2022
-- Description:	Load px data to a single field
-- =============================================
CREATE
	

 PROCEDURE Data_Matrix_LoadSingleField @MatrixData NVARCHAR(MAX)
	,@MtrId INT
AS
BEGIN
	SET NOCOUNT ON;


	INSERT INTO TD_MATRIX_DATA (
		MTD_MTR_ID
		,MTD_DATA
		)
	VALUES (
		@MtrId
		,@MatrixData
		)

	RETURN @@ROWCOUNT
END
