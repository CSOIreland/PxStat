SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 08/10/2021
-- Description:	Set an entry in the TD_DATASET table as locked/unlocked
-- 
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Dataset_Lock_Update @DttMtrCode NVARCHAR(20)
	,@DttDatetimeLocked DATETIME = NULL
AS
BEGIN

	SET NOCOUNT ON;

	-- If the Mtr Code is not in the table then we insert the entry
	IF (
			SELECT COUNT(*)
			FROM TD_DATASET
			WHERE DTT_MTR_CODE = @DttMtrCode
			) = 0
	BEGIN
		INSERT INTO TD_DATASET (DTT_MTR_CODE)
		VALUES (@DttMtrCode)
	END

	UPDATE TD_DATASET
	SET DTT_DATETIME_LOCKED = @DttDatetimeLocked
	WHERE DTT_MTR_CODE = @DttMtrCode

	RETURN @@ROWCOUNT
END
GO


