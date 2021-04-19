SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 02/01/2020
-- Description:	Reads current releases, referencing metadata
-- exec Data_Release_ReadCollection 'en','ga','2019-01-21','ELEC'
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Release_ReadCollection @LngIsoCodeDefault CHAR(2)
	,@LngIsoCodeRead CHAR(2) = NULL
	,@DateFrom DATE = NULL
	,@PrcCode NVARCHAR(32) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @LngIdDefault INT
	DECLARE @LngIdRead INT
	DECLARE @PrcID INT

	SET @LngIdDefault = (
			SELECT LNG_ID
			FROM TS_LANGUAGE
			WHERE LNG_ISO_CODE = @LngIsoCodeDefault
				AND LNG_DELETE_FLAG = 0
			)

	IF @LngIsoCodeRead IS NOT NULL
	BEGIN
		SET @LngIdRead = (
				SELECT LNG_ID
				FROM TS_LANGUAGE
				WHERE LNG_ISO_CODE = @LngIsoCodeRead
					AND LNG_DELETE_FLAG = 0
				)
	END
	ELSE
	BEGIN
		SET @LngIdRead = 0
	END

	IF @LngIsoCodeDefault = @LngIsoCodeRead
	BEGIN
		SET @LngIdRead = 0
	END

	IF @PrcCode IS NOT NULL
	BEGIN
		SET @PrcID = (
				SELECT PRC_ID
				FROM TD_PRODUCT
				WHERE PRC_CODE = @PrcCode
					AND PRC_DELETE_FLAG = 0
				)
	END

	SELECT DISTINCT *
	FROM (
		SELECT RLS_CODE AS RlsCode
			,mtr.MTR_CODE AS MtrCode
			,coalesce(lngMTR.LNG_ISO_CODE, TS_LANGUAGE.LNG_ISO_CODE) AS LngIsoCode
			,coalesce(lngMtr.LNG_ISO_NAME, TS_LANGUAGE.LNG_ISO_NAME) AS LngIsoName
			,coalesce(lngMTR.MTR_TITLE, mtr.MTR_TITLE) AS MtrTitle
			,RLS_LIVE_DATETIME_FROM AS RlsLiveDatetimeFrom
			,RLS_LIVE_DATETIME_TO AS RlsLiveDatetimeTo
			,RLS_EXCEPTIONAL_FLAG AS ExceptionalFlag
			,CPR_VALUE AS CprValue
			,CPR_URL AS CprUrl
			,CPR_CODE AS CprCode
			,TD_CLASSIFICATION.CLS_CODE AS ClsCode
			,coalesce(lngMTR.CLS_VALUE, TD_CLASSIFICATION.CLS_VALUE) AS ClsValue
			,FRQ_CODE AS FrqCode
			,coalesce(lngMTR.FRQ_VALUE, TD_FREQUENCY.FRQ_VALUE) AS FrqValue
		FROM TD_RELEASE rls
		INNER JOIN VW_RELEASE_LIVE_NOW
			ON VRN_RLS_ID = RLS_ID
				AND (
					@DateFrom IS NULL
					OR RLS_LIVE_DATETIME_FROM >= @DateFrom
					)
		INNER JOIN (
			SELECT *
			FROM TD_MATRIX
			WHERE MTR_LNG_ID = @LngIdDefault
			) mtr
			ON RLS_ID = MTR_RLS_ID
				AND MTR_DELETE_FLAG = 0
				AND RLS_DELETE_FLAG = 0
				AND MTR_ID = VRN_MTR_ID
		INNER JOIN TS_COPYRIGHT
			ON CPR_ID = MTR_CPR_ID
				AND CPR_DELETE_FLAG = 0
		INNER JOIN TS_LANGUAGE
			ON LNG_ID = MTR_LNG_ID
				AND LNG_DELETE_FLAG = 0
		INNER JOIN TD_CLASSIFICATION
		ON MTR_ID = CLS_MTR_ID
		INNER JOIN TD_FREQUENCY
			ON FRQ_MTR_ID = MTR_ID
		LEFT JOIN (
			SELECT MTR_CODE
				,MTR_ID
				,MTR_TITLE
				,MTR_RLS_ID
				,LNG_ISO_CODE
				,LNG_ISO_NAME
				,CLS_VALUE
				,CLS_CODE
				,FRQ_VALUE 
			FROM TD_MATRIX
			INNER JOIN TS_LANGUAGE
				ON MTR_LNG_ID = LNG_ID
					AND LNG_DELETE_FLAG = 0
			INNER JOIN TD_CLASSIFICATION
			ON MTR_ID=CLS_MTR_ID 
			INNER JOIN TD_FREQUENCY 
			ON FRQ_MTR_ID=MTR_ID 

			INNER JOIN TD_RELEASE ON MTR_RLS_ID = RLS_ID
			INNER JOIN VW_RELEASE_LIVE_NOW ON VRN_RLS_ID = RLS_ID
				AND VRN_MTR_ID = MTR_ID

			WHERE Mtr_Lng_ID = @LngIdRead
				AND MTR_DELETE_FLAG = 0
			

			) lngMtr
			ON lngMtr.MTR_CODE = mtr.MTR_CODE
			and lngMtr.CLS_CODE=TD_CLASSIFICATION.CLS_CODE
		WHERE (
				@PrcID IS NULL
				OR @PrcID = RLS_PRC_ID
				)
		) q
		
END
GO


