SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 13/06/2024
-- Description:	Update a matrix 
-- Add new parameters as required (allowing that nulls will result in no update)
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Matrix_Update @MtrId INT
	,@MtrDataFlag BIT=NULL
	,@MtrInput NVARCHAR(MAX)=NULL
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE TD_MATRIX
	SET MTR_DATA_FLAG =COALESCE (@MtrDataFlag,MTR_DATA_FLAG)
	,MTR_INPUT=COALESCE(@MtrInput,MTR_INPUT)
	WHERE MTR_ID = @MtrId

	RETURN @@rowcount
END
GO