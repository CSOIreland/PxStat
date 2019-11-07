-- =============================================
--
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Matrix_ReadDataByRelease @RlsCode VARCHAR(256)
	,@LngIsoCode CHAR(2)
	,@VrbCodeList KeyValueVarchar READONLY
	,@SttCodeList ValueVarchar READONLY
	,@PrdCodeList ValueVarchar READONLY
AS
BEGIN
	DECLARE @ParmDefinition NVARCHAR(1000);
	DECLARE @MatrixIdInt INT

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
						@LngIsoCode IS NULL
						OR @LngIsoCode = LNG_ISO_CODE
						)
			WHERE RLS_CODE = @RlsCode
				AND MTR_DELETE_FLAG = 0
			)

	DECLARE @StatList TABLE (stt_id INT INDEX IX_stt_sttlist NONCLUSTERED)
	DECLARE @PeriodList TABLE (prd_id INT INDEX IX_prd_prdlist NONCLUSTERED)
	DECLARE @ClsList TABLE (cls_id INT INDEX IX_cls_clslist NONCLUSTERED)
	DECLARE @VrbList TABLE (
		vrb_id INT INDEX IX_vrb_vrblist NONCLUSTERED
		,cls_id INT INDEX IX_cls_vrblist NONCLUSTERED
		,vrb_code NVARCHAR(256)
		,cls_code NVARCHAR(256)
		)

	--create the @VrbList table
	INSERT INTO @VrbList (
		vrb_code
		,cls_code
		) (
		SELECT [value]
		,[key] FROM @VrbCodeList
		)

	--Do a correlated update (joins won't work to the @VrbCodeList parameter) for the rest of the data
	UPDATE vlist
	SET vlist.vrb_id = tdv.VRB_ID
		,vlist.cls_id = tdv.VRB_CLS_ID
	FROM @VrbList vlist
	INNER JOIN TD_VARIABLE tdv
		ON vlist.vrb_code = tdv.VRB_CODE
	INNER JOIN TD_CLASSIFICATION cls
		ON tdv.VRB_CLS_ID = cls.CLS_ID
			AND cls.CLS_MTR_ID = @MatrixIdInt
			AND cls.CLS_CODE = vlist.cls_code

	--either restrict ourselves to the supplied statistic codes or get all of them if none are supplied
	IF (
			SELECT count(*)
			FROM @SttCodeList
			) = 0
	BEGIN
		INSERT INTO @StatList
		SELECT stt_id
		FROM TD_STATISTIC
		WHERE STT_MTR_ID = @MatrixIdInt
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
				)
	END

	--either restrict ourselves to the supplied period codes or get all of them if none are supplied
	IF (
			SELECT count(*)
			FROM @PrdCodeList
			) = 0
	BEGIN
		INSERT INTO @PeriodList
		SELECT PRD_ID
		FROM TD_PERIOD
		INNER JOIN TD_FREQUENCY
			ON PRD_FRQ_ID = FRQ_ID
		WHERE FRQ_MTR_ID = @MatrixIdInt
	END
	ELSE
	BEGIN
		INSERT INTO @PeriodList
		SELECT PRD_ID
		FROM TD_PERIOD
		INNER JOIN TD_FREQUENCY
			ON PRD_FRQ_ID = FRQ_ID
		WHERE FRQ_MTR_ID = @MatrixIdInt
			AND PRD_CODE IN (
				SELECT [Value]
				FROM @PrdCodeList
				)
	END

	--We assume that if a variable is not passed in for a given classification then we will want everything from that classification
	INSERT INTO @ClsList
	SELECT cls_id
	FROM TD_CLASSIFICATION
	WHERE CLS_MTR_ID = @MatrixIdInt
		AND cls_id NOT IN (
			SELECT cls_id
			FROM @VrbList
			)

	DECLARE @cCount INT

	SET @cCount = (
			SELECT count(*)
			FROM TD_CLASSIFICATION
			WHERE cls_id IN (
					SELECT DISTINCT vrb_cls_id
					FROM td_variable
					WHERE VRB_ID IN (
							SELECT vrb_id
							FROM @VrbList
							)
					)
			)

	-- get a list of candidate data cells - there will be too many but we will order them anyway
	DECLARE @otable TABLE (
		tdt_id INT INDEX IX_tdt_otable NONCLUSTERED
		,TDT_STT_ID INT
		,TDT_PRD_ID INT
		,cls_id INT
		,vrb_id INT
		,spc INT
		)
	--get an unordered list of the correct data cells
	DECLARE @dtable TABLE (tdt_id INT INDEX IX_tdt_dtable NONCLUSTERED)

	-- Fill the table with the correct data but unordered
	INSERT INTO @dtable
	SELECT TDT_ID
	FROM TM_DATA_CELL
	JOIN td_data
		ON tdt_ix = dtc_tdt_ix
	JOIN @PeriodList
		ON TDT_PRD_ID = prd_id
	JOIN @StatList
		ON TDT_STT_ID = stt_id
	JOIN TD_VARIABLE
		ON TM_DATA_CELL.DTC_VRB_ID = VRB_ID
	JOIN TD_CLASSIFICATION
		ON VRB_CLS_ID = CLS_ID
	WHERE td_data.TDT_MTR_ID = @MatrixIdInt
		AND (
			VRB_CLS_ID IN (
				SELECT cls_id
				FROM @ClsList
				)
			OR dtc_vrb_id IN (
				SELECT vrb_id
				FROM @VrbList
				)
			)
	GROUP BY tdt_id
	HAVING count(tdt_id) > @cCount - 1

	--Fills the table with some redundant data but correctly ordered
	INSERT INTO @otable
	SELECT q.*
		,spc = row_number() OVER (
			PARTITION BY 1 ORDER BY q.TDT_STT_ID
				,q.TDT_PRD_ID
				,q.cls_id
				,q.vrb_id
			)
	FROM (
		SELECT TDT_ID
			,TDT_STT_ID
			,TDT_PRD_ID
			,VRB_ID
			,CLS_ID
		FROM td_data
		JOIN TM_DATA_CELL
			ON tdt_ix = dtc_tdt_ix
				AND TDT_MTR_ID = @MatrixIdInt
		JOIN @PeriodList
			ON TDT_PRD_ID = prd_id
		JOIN @StatList
			ON TDT_STT_ID = stt_id
		JOIN TD_VARIABLE
			ON TM_DATA_CELL.DTC_VRB_ID = VRB_ID
		JOIN TD_CLASSIFICATION
			ON VRB_CLS_ID = CLS_ID
		WHERE td_data.TDT_MTR_ID = @MatrixIdInt
			AND (
				VRB_CLS_ID IN (
					SELECT cls_id
					FROM @ClsList
					)
				OR dtc_vrb_id IN (
					SELECT vrb_id
					FROM @VrbList
					)
				)
		GROUP BY tdt_id
			,TDT_VALUE
			,TDT_STT_ID
			,TDT_PRD_ID
			,VRB_ID
			,CLS_ID
		) q

	--finally we join the correct data list with the ordered list and order by the ordered list (Ordered SPC (Statistic, Period, Classification)
	SELECT d.tdt_id
		,tdt.TDT_VALUE AS TdtValue
		,min(o.spc) AS ospc
	FROM @otable o
	INNER JOIN @dtable d
		ON d.tdt_id = o.tdt_id
	INNER JOIN TD_DATA tdt
		ON d.tdt_id = tdt.TDT_ID
	GROUP BY d.tdt_id
		,tdt.TDT_VALUE
	ORDER BY ospc
END
