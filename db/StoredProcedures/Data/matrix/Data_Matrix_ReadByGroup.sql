SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 07/02/2020
-- Description:	Returns a list of individual Matrix codes that are associated with a given group
-- EXEC Data_Matrix_ReadByGroup 'JCTEST','en','en'
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Matrix_ReadByGroup @GrpCode NVARCHAR(32)
	,@LngIsoCode CHAR(2)
	,@LngIsoCodeDefault CHAR(2)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @IsLive BIT
	DECLARE @IsNotLive BIT

	SET @IsLive=1
	SET @IsNotLive=0

	DECLARE @LngId INT
	DECLARE @LngDefaultId INT

	SET @LngId = (
			SELECT LNG_ID
			FROM TS_LANGUAGE
			WHERE LNG_ISO_CODE = @LngIsoCode
				AND LNG_DELETE_FLAG = 0
			)
	SET @LngDefaultId = (
			SELECT LNG_ID
			FROM TS_LANGUAGE
			WHERE LNG_ISO_CODE = @LngIsoCodeDefault
				AND LNG_DELETE_FLAG = 0
			)
	
	SELECT DISTINCT mtr.MTR_CODE MtrCode
	,coalesce(mtrLng.MTR_TITLE, mtr.MTR_TITLE) MtrTitle,RLS_CODE RlsCode
	,CASE WHEN VRN_MTR_ID IS NOT NULL THEN @IsLive ELSE @IsNotLive END AS IsLive
	FROM TD_MATRIX mtr
	LEFT JOIN (
			SELECT MTR_CODE
				,MTR_TITLE
				,MTR_RLS_ID
			FROM TD_MATRIX
			WHERE MTR_DELETE_FLAG = 0
				AND MTR_LNG_ID = @LngId
			) mtrLng
			ON mtr.MTR_CODE = mtrLng.MTR_CODE
				AND MTR.MTR_RLS_ID = mtrLng.MTR_RLS_ID
	INNER JOIN TD_RELEASE
		ON mtr.MTR_RLS_ID = RLS_ID
			AND MTR_DELETE_FLAG = 0
			AND RLS_DELETE_FLAG = 0
	INNER JOIN TD_GROUP
	ON RLS_GRP_ID=GRP_ID
	AND GRP_CODE=@GrpCode 
	AND GRP_DELETE_FLAG=0
	LEFT JOIN VW_RELEASE_LIVE_NOW 
	ON MTR_ID=VRN_MTR_ID 
	ORDER BY mtr.MTR_CODE
END
GO


