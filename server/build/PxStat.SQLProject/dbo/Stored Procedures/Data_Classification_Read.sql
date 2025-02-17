
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 19/03/2019
-- Description:	Reads an individual Classification based on ClassificationID
-- exec Data_Classification_Read 3
-- =============================================
CREATE
	

 PROCEDURE Data_Classification_Read @ClsID INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT MDM_CODE AS ClsCode
		,MDM_VALUE AS ClsValue
		,CASE WHEN MDM_GEO_FLAG=0 THEN NULL ELSE MDM_GEO_URL END AS ClsGeoUrl
		,DMT_CODE AS VrbCode
		,DMT_VALUE AS VrbValue
		,MTR_CODE AS MtrCode
		,DMT_ELIMINATION_FLAG AS VrbEliminationFlag
		,CASE 
			WHEN rcount IS NULL
				THEN 0
			ELSE rcount
			END AS VrbCount
		,MDM_ID as ClsID
	FROM TD_MATRIX_DIMENSION
	INNER JOIN TS_DIMENSION_ROLE ON MDM_DMR_ID = DMR_ID
		AND DMR_CODE = 'CLASSIFICATION'
	INNER JOIN TD_DIMENSION_ITEM ON DMT_MDM_ID = MDM_ID
	INNER JOIN TD_MATRIX ON MDM_MTR_ID = MTR_ID
	INNER JOIN VW_RELEASE_LIVE_NOW ON MTR_ID = VRN_MTR_ID
	LEFT JOIN (
		SELECT MDM_ID AS mdmClsID
			,COUNT(*) AS rcount
		FROM TD_MATRIX_DIMENSION
		INNER JOIN TS_DIMENSION_ROLE ON MDM_DMR_ID = DMR_ID
			AND DMR_CODE = 'CLASSIFICATION'
		INNER JOIN TD_DIMENSION_ITEM ON DMT_MDM_ID = MDM_ID
		GROUP BY MDM_ID
		) countQuery ON MDM_ID = mdmClsID
	WHERE MDM_ID = @ClsID
END
