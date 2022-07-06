SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 30/03/2022
-- Description:	Get Frequency Code and Value for a matrix. Required for Migration to version 5.
-- exec Migration_GetFrequencyDetails 640
-- =============================================
CREATE
	OR

ALTER PROCEDURE Migration_GetFrequencyDetails @MtrId INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT FRQ_CODE AS FrqCode
		,FRQ_VALUE AS FrqValue
	FROM TD_MATRIX
	INNER JOIN TD_FREQUENCY ON MTR_ID = FRQ_MTR_ID
		AND MTR_DELETE_FLAG = 0
		AND MTR_ID = @MtrId
END
GO


