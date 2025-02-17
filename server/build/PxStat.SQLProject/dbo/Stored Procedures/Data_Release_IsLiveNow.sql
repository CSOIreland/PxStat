
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 01/08/2019
-- Description:	Tests if a Release is LiveNow
-- exec Data_Release_IsLiveNow 2
-- =============================================
CREATE
	

 PROCEDURE Data_Release_IsLiveNow @RlsCode INT
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT TOP 1 VRN_RLS_ID
			FROM VW_RELEASE_LIVE_NOW
			INNER JOIN TD_RELEASE
				ON VRN_RLS_ID = RLS_ID
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
