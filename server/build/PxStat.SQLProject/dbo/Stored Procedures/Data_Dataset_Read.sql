
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 08/10/2021
-- Description:	Reads the lock state of a dataset
-- =============================================
CREATE   PROCEDURE Data_Dataset_Read 
	@DttMtrCode NVARCHAR(20)
AS
BEGIN

	SET NOCOUNT ON;

	SELECT DTT_DATETIME_LOCKED as DttDatetimeLocked
	FROM TD_DATASET
	WHERE DTT_MTR_CODE=@DttMtrCode

END
