SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 29/11/2019
-- Description:	Update a matrix to set the Update flag. This flag indicates that this matrix has data.
-- Typically where multiple languages are represented in a single release, only one matrix will have the data attached.
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Matrix_Update_Data_Flag @MtrId INT
	,@MtrDataFlag BIT
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE TD_MATRIX
	SET MTR_DATA_FLAG = @MtrDataFlag
	WHERE MTR_ID = @MtrId

	RETURN @@rowcount
END
GO


