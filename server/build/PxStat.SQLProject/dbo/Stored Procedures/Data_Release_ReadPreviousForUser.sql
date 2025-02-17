
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 15/04/2021
-- Description:	Returns the previous release for comparison given one release code only if the user has viewing rights
-- exec Data_Release_ReadPreviousForUser 10,'okeeffene'
-- =============================================
CREATE
	

 PROCEDURE Data_Release_ReadPreviousForUser @RlsCode INT
,@CcnUsername NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;



	DECLARE @GroupUserHasAccess TABLE (GRP_ID INT NOT NULL);

	INSERT INTO @GroupUserHasAccess
	EXEC Security_Group_AccessList @CcnUsername



	DECLARE @MtrCode NVARCHAR(20)

	SELECT @MtrCode = MTR_CODE
	FROM TD_RELEASE
	INNER JOIN TD_MATRIX
		ON MTR_RLS_ID = RLS_ID
			AND MTR_DELETE_FLAG = 0
			AND RLS_DELETE_FLAG = 0
	WHERE RLS_CODE = @RlsCode
	GROUP BY MTR_CODE

	SELECT TOP 1 RLS_CODE as RlsCode

	FROM TD_RELEASE
	INNER JOIN TD_MATRIX
		ON MTR_RLS_ID = RLS_ID
			AND MTR_DELETE_FLAG = 0
			AND RLS_DELETE_FLAG = 0
	INNER JOIN @GroupUserHasAccess
	ON GRP_ID=RLS_GRP_ID 
	WHERE MTR_CODE = @MtrCode
		AND RLS_CODE < @RlsCode
		AND RLS_LIVE_DATETIME_FROM IS NOT NULL
		AND (RLS_LIVE_DATETIME_TO IS NULL OR( DATEDIFF(s, RLS_LIVE_DATETIME_FROM,RLS_LIVE_DATETIME_TO) >0))
	ORDER BY MTR_CODE ASC
		,RLS_CODE DESC
		,RLS_VERSION DESC
		,RLS_REVISION DESC

END
