SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Paulo Patricio
-- Read date: 22 Oct 2018
-- Description:	Returns the previous release for comparison given one release code
-- exec Data_Stat_Release_ReadPrevious 194
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Release_ReadPrevious @RlsCode INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @MtrCode NVARCHAR(20)
	DECLARE @PreviousRlsCode INT = NULL

	SELECT @MtrCode = MTR_CODE
	FROM TD_RELEASE
	INNER JOIN TD_MATRIX
		ON MTR_RLS_ID = RLS_ID
			AND MTR_DELETE_FLAG = 0
			AND RLS_DELETE_FLAG = 0
	WHERE RLS_CODE = @RlsCode
	GROUP BY MTR_CODE

	SELECT TOP 1 @PreviousRlsCode = RLS_CODE
	FROM TD_RELEASE
	INNER JOIN TD_MATRIX
		ON MTR_RLS_ID = RLS_ID
			AND MTR_DELETE_FLAG = 0
			AND RLS_DELETE_FLAG = 0
	WHERE MTR_CODE = @MtrCode
		AND RLS_CODE < @RlsCode
	ORDER BY MTR_CODE ASC
		,RLS_CODE DESC
		,RLS_VERSION DESC
		,RLS_REVISION DESC

	IF (@PreviousRlsCode IS NULL)
	BEGIN
		SELECT 0 AS PreviousRlsCode
	END
	ELSE
	BEGIN
		SELECT @PreviousRlsCode AS PreviousRlsCode
	END
END
GO


