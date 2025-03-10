﻿
CREATE
	

 PROCEDURE [dbo].[Data_Matrix_Read_Live] @MtrCode NVARCHAR(20)
	,@LngIsoCode CHAR(2)
	,@LngIsoCodeDefault CHAR(2)
AS
BEGIN
	SET NOCOUNT ON;

	IF (@LngIsoCode = @LngIsoCodeDefault)
	BEGIN
		SELECT MTR_CODE AS MtrCode
			,MTR_ID AS MtrId
			,MTR_OFFICIAL_FLAG AS MtrOfficialFlag
			,FRM_TYPE AS FrmType
			,FRM_VERSION AS FrmVersion
			,LNG_ISO_CODE AS LngIsoCode
			,CPR_CODE AS CprCode
			,CPR_VALUE AS CprValue
			,CPR_URL AS CprUrl
			,MTR_TITLE AS MtrTitle
			,MTR_NOTE AS MtrNote
		FROM TD_MATRIX
		INNER JOIN VW_RELEASE_LIVE_NOW ON MTR_ID = VRN_MTR_ID
		INNER JOIN TS_LANGUAGE ON MTR_LNG_ID = LNG_ID
			AND LNG_DELETE_FLAG = 0
			AND LNG_ISO_CODE = @LngIsoCode
			AND MTR_CODE = @MtrCode
			AND MTR_DELETE_FLAG = 0
		INNER JOIN TS_FORMAT ON FRM_ID = MTR_FRM_ID
		INNER JOIN TS_COPYRIGHT ON MTR_CPR_ID = CPR_ID
			AND CPR_DELETE_FLAG = 0;
	END
	ELSE
	BEGIN
		IF (
				SELECT COUNT(*)
				FROM TD_MATRIX
				INNER JOIN VW_RELEASE_LIVE_NOW ON MTR_ID = VRN_MTR_ID
				INNER JOIN TS_LANGUAGE ON MTR_LNG_ID = LNG_ID
					AND LNG_DELETE_FLAG = 0
					AND LNG_ISO_CODE = @LngIsoCode
					AND MTR_CODE = @MtrCode
					AND MTR_DELETE_FLAG = 0
				) > 0
		BEGIN
			SELECT MTR_CODE AS MtrCode
				,MTR_ID AS MtrId
				,MTR_OFFICIAL_FLAG AS MtrOfficialFlag
				,FRM_TYPE AS FrmType
				,FRM_VERSION AS FrmVersion
				,LNG_ISO_CODE AS LngIsoCode
				,CPR_CODE AS CprCode
				,CPR_VALUE AS CprValue
				,CPR_URL AS CprUrl
				,MTR_TITLE AS MtrTitle
				,MTR_NOTE AS MtrNote
			FROM TD_MATRIX
			INNER JOIN VW_RELEASE_LIVE_NOW ON MTR_ID = VRN_MTR_ID
			INNER JOIN TS_LANGUAGE ON MTR_LNG_ID = LNG_ID
				AND LNG_DELETE_FLAG = 0
				AND LNG_ISO_CODE = @LngIsoCode
				AND MTR_CODE = @MtrCode
				AND MTR_DELETE_FLAG = 0
			INNER JOIN TS_FORMAT ON FRM_ID = MTR_FRM_ID
			INNER JOIN TS_COPYRIGHT ON MTR_CPR_ID = CPR_ID
				AND CPR_DELETE_FLAG = 0;
		END
		ELSE
		BEGIN
			SELECT MTR_CODE AS MtrCode
				,MTR_ID AS MtrId
				,MTR_OFFICIAL_FLAG AS MtrOfficialFlag
				,FRM_TYPE AS FrmType
				,FRM_VERSION AS FrmVersion
				,LNG_ISO_CODE AS LngIsoCode
				,CPR_CODE AS CprCode
				,CPR_VALUE AS CprValue
				,CPR_URL AS CprUrl
				,MTR_TITLE AS MtrTitle
				,MTR_NOTE AS MtrNote
			FROM TD_MATRIX
			INNER JOIN VW_RELEASE_LIVE_NOW ON MTR_ID = VRN_MTR_ID
			INNER JOIN TS_LANGUAGE ON MTR_LNG_ID = LNG_ID
				AND LNG_DELETE_FLAG = 0
				AND LNG_ISO_CODE = @LngIsoCodeDefault
				AND MTR_CODE = @MtrCode
				AND MTR_DELETE_FLAG = 0
			INNER JOIN TS_FORMAT ON FRM_ID = MTR_FRM_ID
			INNER JOIN TS_COPYRIGHT ON MTR_CPR_ID = CPR_ID
				AND CPR_DELETE_FLAG = 0;
		END
	END
END