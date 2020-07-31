SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 08/04/2019
-- Description:	Reads the next release date/time after the supplied release date
-- exec Data_Release_ReadNext '2020-02-04T10:58:00','CODE2'
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Release_ReadNext @ReleaseDate DATETIME
	,@PrcCode NVARCHAR(32) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @PrcID INT

	IF @PrcCode IS NOT NULL
	BEGIN
		SET @PrcID = (
				SELECT PRC_ID
				FROM TD_PRODUCT
				WHERE PRC_CODE = @PrcCode
					AND PRC_DELETE_FLAG = 0
				)
	END

	IF @PrcID IS NULL
		AND @PrcCode IS NOT NULL
	BEGIN
		SELECT NULL AS NextRelease
	END
	ELSE
	BEGIN
		SELECT 
				min(RLS_LIVE_DATETIME_FROM)
				
				AS NextRelease
		FROM TD_RELEASE
		INNER JOIN VW_RELEASE_LIVE_NEXT
			ON VRX_RLS_ID = RLS_ID
		WHERE RLS_LIVE_DATETIME_FROM >= @ReleaseDate
			AND RLS_LIVE_DATETIME_TO IS NULL
			AND (@PrcCode IS NULL OR @PrcID=RLS_PRC_ID)
	END
END
GO


