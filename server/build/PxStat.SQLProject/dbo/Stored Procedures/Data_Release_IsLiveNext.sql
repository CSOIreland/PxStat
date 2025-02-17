
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 01/08/2019
-- Description:	Tests if a Release is LiveNext
-- exec Data_Release_IsLiveNext 2
-- =============================================
CREATE
	

 PROCEDURE Data_Release_IsLiveNext @RlsCode INT
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT TOP 1 VRX_RLS_ID
			FROM VW_RELEASE_LIVE_NEXT
			INNER JOIN TD_RELEASE
				ON VRX_RLS_ID = RLS_ID
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
