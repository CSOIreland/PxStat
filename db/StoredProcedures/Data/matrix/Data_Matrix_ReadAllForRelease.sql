SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 30/11/2018
-- Description:	Gets all matrices for a specific Release Code
-- EXEC Data_Matrix_ReadAllForRelease 'okeeffene', 12
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Matrix_ReadAllForRelease @CcnUsername NVARCHAR(256)
	,@RlsCode INT
AS
BEGIN
	DECLARE @GroupUserHasAccess TABLE (GRP_ID INT NOT NULL);

	INSERT INTO @GroupUserHasAccess
	EXEC Security_Group_AccessList @CcnUsername

	SELECT mtr.MTR_CODE AS MtrCode
		,mtr.MTR_TITLE AS MtrTitle
		,rls.RLS_CODE AS RlsCode
		,lng.LNG_ISO_CODE AS LngIsoCode
		,lng.LNG_ISO_NAME AS LngIsoName
		,rls.RLS_ID AS RlsID
		,cpr.CPR_CODE AS CprCode
		,cpr.CPR_VALUE AS CprValue
		,MDM_CODE AS FrqCode
		,MDM_VALUE AS FrqValue
	FROM TD_MATRIX mtr
	INNER JOIN TD_RELEASE rls
		ON rls.RLS_ID = mtr.MTR_RLS_ID
			AND rls.RLS_DELETE_FLAG = 0
			AND mtr.MTR_DELETE_FLAG = 0
	INNER JOIN TS_LANGUAGE lng
		ON mtr.MTR_LNG_ID = lng.LNG_ID
			AND mtr.MTR_DELETE_FLAG = 0
			AND lng.LNG_DELETE_FLAG = 0
	INNER JOIN @GroupUserHasAccess g
		ON g.GRP_ID = rls.RLS_GRP_ID
	INNER JOIN TS_COPYRIGHT cpr
		ON cpr.CPR_ID = mtr.MTR_CPR_ID
			AND cpr.CPR_DELETE_FLAG = 0
	INNER JOIN TD_MATRIX_DIMENSION 
		ON mtr.MTR_ID = MDM_MTR_ID 
	INNER JOIN TS_DIMENSION_ROLE 
	ON MDM_DMR_ID=DMR_ID 
	AND DMR_CODE='TIME'
	WHERE rls.RLS_CODE = @RlsCode
END
GO


