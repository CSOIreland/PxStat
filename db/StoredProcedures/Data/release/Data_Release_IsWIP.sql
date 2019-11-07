SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 01/08/2019
-- Description:	Tests if a Release is WIP (Work In Progress)
-- exec Data_Release_IsWIP 2
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Release_IsWIP @RlsCode INT
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT TOP 1 VRW_RLS_ID
			FROM VW_RELEASE_WIP
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


