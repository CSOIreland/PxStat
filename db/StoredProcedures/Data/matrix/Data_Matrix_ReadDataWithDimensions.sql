SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 24/05/2019
-- Description:	Reads data points for a release with associated statistic,period,classification variables
-- exec Data_Stat_Matrix_ReadDataWithDimensions 199 ,'ga'199
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Matrix_ReadDataWithDimensions @RlsCode VARCHAR(256)
	,@LngIsoCode CHAR(2) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @MatrixID INT

	SET @MatrixID = (
			SELECT MTR_ID
			FROM TD_MATRIX
			INNER JOIN TD_RELEASE
				ON MTR_RLS_ID = RLS_ID
					AND MTR_DELETE_FLAG = 0
					AND RLS_DELETE_FLAG = 0
			LEFT JOIN TS_LANGUAGE
				ON MTR_LNG_ID = LNG_ID
					AND LNG_DELETE_FLAG = 0
			WHERE RLS_CODE = @RlsCode
				AND (
					@LngIsoCode IS NULL
					OR LNG_ISO_CODE = @LngIsoCode
					)
			)

	IF (
			@MatrixID = 0
			OR @MatrixID IS NULL
			)
	BEGIN
		RETURN
	END

	SELECT TDT_ID AS TdtId
		,TDT_VALUE AS TdtValue
		,STT_CODE AS SttCode
		,STT_VALUE AS SttValue
		,PRD_CODE AS PrdCode
		,PRD_VALUE AS PrdValue
	FROM TD_DATA
	INNER JOIN TD_MATRIX
		ON MTR_ID = TDT_MTR_ID
			AND MTR_DELETE_FLAG = 0
	INNER JOIN TD_STATISTIC
		ON TDT_STT_ID = STT_ID
	INNER JOIN TD_PERIOD
		ON TDT_PRD_ID = PRD_ID
	WHERE TDT_MTR_ID = @MatrixID
	ORDER BY TDT_ID

	SELECT TDT_ID AS TdtId
		,TDT_VALUE AS TdtValue
		,CLS_VALUE AS ClsValue
		,CLS_CODE AS ClsCode
		,VRB_VALUE AS VrbValue
		,VRB_CODE AS VrbCode
	FROM TD_DATA
	INNER JOIN TD_MATRIX
		ON MTR_ID = TDT_MTR_ID
			AND MTR_DELETE_FLAG = 0
	INNER JOIN TM_DATA_CELL
		ON TDT_IX = DTC_TDT_IX
	INNER JOIN TD_VARIABLE
		ON DTC_VRB_ID = VRB_ID
	INNER JOIN TD_CLASSIFICATION
		ON VRB_CLS_ID = CLS_ID
	WHERE TDT_MTR_ID = @MatrixID
		AND DTC_MTR_ID = @MatrixID
	ORDER BY TDT_ID
END
GO


