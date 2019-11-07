SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 05/11/2018
-- Description:	Check if a Reason Code exists in the system
-- =============================================
CREATE
	OR

ALTER PROCEDURE System_Settings_Reason_Exists @RsnCode NVARCHAR(32)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @result INT

	SET @result = (
			SELECT iif(EXISTS (
						SELECT RSN_ID
						FROM TS_REASON
						WHERE RSN_CODE = @RsnCode
							AND RSN_DELETE_FLAG = 0
						), 1, 0)
			)

	RETURN @result
END
GO


