SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE
	OR

ALTER PROCEDURE [dbo].[Data_Matrix_ReadDataByRelease] @RlsCode VARCHAR(256)
	,@SttCodeList ValueVarchar READONLY
	,@PrdCodeList ValueVarchar READONLY
	,@ClsVrbCodeList KeyValueVarchar READONLY
AS
BEGIN
	DECLARE @MatrixIdInt AS INT;

	SET @MatrixIdInt = (
			SELECT MTR_ID
			FROM TD_RELEASE
			INNER JOIN TD_MATRIX
				ON MTR_RLS_ID = RLS_ID
					AND MTR_DELETE_FLAG = 0
			INNER JOIN TS_LANGUAGE
				ON MTR_LNG_ID = LNG_ID
					AND LNG_DELETE_FLAG = 0
					AND (
						MTR_DATA_FLAG=1
						)
			WHERE RLS_CODE = @RlsCode
				AND MTR_DELETE_FLAG = 0
			);

	DECLARE @StatList TABLE (
		stt_id INT NOT NULL
		,PRIMARY KEY (stt_id)
		);
	DECLARE @PeriodList TABLE (
		prd_id INT NOT NULL
		,PRIMARY KEY (prd_id)
		);
	DECLARE @VrbList TABLE (
		vrb_id INT NOT NULL
		,PRIMARY KEY (vrb_id)
		);

	--Populate the @VrbList table with the selected Key/Value pairs of Classification/Variable
	INSERT INTO @VrbList (vrb_id) (
		SELECT vrb.vrb_id FROM @ClsVrbCodeList AS ClsVrbCodeList INNER JOIN TD_CLASSIFICATION AS cls
		ON ClsVrbCodeList.[Key] = cls.CLS_CODE INNER JOIN TD_VARIABLE AS vrb
		ON cls.CLS_ID = vrb.VRB_CLS_ID
			AND ClsVrbCodeList.[Value] = vrb.VRB_CODE WHERE cls.CLS_MTR_ID = @MatrixIdInt
		);

	--Complement the @VrbList table with the Variables taken form the non selected Classifications
	INSERT INTO @VrbList (vrb_id) (
		SELECT vrb.vrb_id FROM TD_CLASSIFICATION AS cls INNER JOIN TD_VARIABLE AS vrb
		ON cls.CLS_ID = vrb.VRB_CLS_ID WHERE cls.CLS_MTR_ID = @MatrixIdInt
		AND cls.CLS_CODE NOT IN (
			SELECT [Key]
			FROM @ClsVrbCodeList
			)
		);

	--Either restrict ourselves to the supplied statistic codes or get all of them if none are supplied
	IF (
			SELECT count(*)
			FROM @SttCodeList
			) = 0
	BEGIN
		INSERT INTO @StatList (stt_id)
		SELECT stt_id
		FROM TD_STATISTIC
		WHERE STT_MTR_ID = @MatrixIdInt;
	END
	ELSE
	BEGIN
		INSERT INTO @StatList
		SELECT stt_id
		FROM td_statistic
		WHERE STT_MTR_ID = @MatrixIdInt
			AND STT_CODE IN (
				SELECT [Value]
				FROM @SttCodeList
				);
	END

	--Either restrict ourselves to the supplied period codes or get all of them if none are supplied
	IF (
			SELECT count(*)
			FROM @PrdCodeList
			) = 0
	BEGIN
		INSERT INTO @PeriodList (prd_id)
		SELECT PRD_ID
		FROM TD_PERIOD
		INNER JOIN TD_FREQUENCY
			ON FRQ_MTR_ID = @MatrixIdInt
				AND PRD_FRQ_ID = FRQ_ID;
	END
	ELSE
	BEGIN
		INSERT INTO @PeriodList
		SELECT PRD_ID
		FROM TD_FREQUENCY
		INNER JOIN TD_PERIOD
			ON FRQ_MTR_ID = @MatrixIdInt
				AND PRD_FRQ_ID = FRQ_ID
		WHERE PRD_CODE IN (
				SELECT [Value]
				FROM @PrdCodeList
				);
	END

	--Count the Classifications for the HAVING clause
	DECLARE @cCount AS INT;

	SET @cCount = (
			SELECT count(*)
			FROM TD_CLASSIFICATION AS cls
			WHERE cls.CLS_MTR_ID = @MatrixIdInt
			);

	SELECT TDT_ID AS TdtId
		,TDT_STT_ID AS SttId
		,TDT_PRD_ID AS PrdId
		,TDT_VALUE AS TdtValue
	FROM TD_DATA
	INNER JOIN TM_DATA_CELL
		ON TDT_MTR_ID = DTC_MTR_ID
			AND TDT_IX = DTC_TDT_IX
	WHERE TDT_MTR_ID = @MatrixIdInt
		AND TDT_STT_ID IN (
			SELECT stt_id
			FROM @StatList
			)
		AND TDT_PRD_ID IN (
			SELECT prd_id
			FROM @PeriodList
			)
		AND DTC_VRB_ID IN (
			SELECT vrb_id
			FROM @VrbList
			)
	GROUP BY TDT_ID
		,TDT_STT_ID
		,TDT_PRD_ID
		,TDT_VALUE
	HAVING count(tdt_id) = @cCount;
END
GO


