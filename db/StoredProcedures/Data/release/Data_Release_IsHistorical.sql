SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 06/10/2020
-- Description:	Tests if a Release is Historical
-- exec Data_Release_IsHistorical 2
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Release_IsHistorical @RlsCode INT
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT TOP 1 VRW_RLS_ID
			FROM VW_RELEASE_HISTORICAL
			INNER JOIN TD_RELEASE
				ON VRW_RLS_ID = RLS_ID
					AND RLS_CODE = @RlsCode
			)
	BEGIN
		RETURN CAST('TRUE' AS BIT)
	END
	ELSE
	BEGIN
		RETURN CAST('FALSE' AS BIT)
	END
END
GO


