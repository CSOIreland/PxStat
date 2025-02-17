
-- =============================================
-- Author:		Paulo Patricio
-- Create date: 21 Sep 2018
-- Description:	Inserts a new record into the TD_MATRIX table
-- =============================================
CREATE  
	

 PROCEDURE [dbo].[Data_Matrix_Create] @MtrCode NVARCHAR(256)
	,@MtrTitle NVARCHAR(256)
	,@LngIsoCode CHAR(2)
	,@CprValue NVARCHAR(256)
	,@FrmType NVARCHAR(32)
	,@FrmVersion NVARCHAR(20)
	,@MtrOfficialFlag BIT = 1
	,@MtrNote NVARCHAR(max) = NULL
	,@MtrInput NVARCHAR(max) = NULL
	,@MtrRlsId INT
	,@userName NVARCHAR(256)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Error Message
	DECLARE @errorMessage VARCHAR(256)
	DECLARE @spName VARCHAR(100)
	set  @spName = 'Data_Stat_Matrix_Create'
	-- language lookup fk
	DECLARE @MtrLngId INT = NULL

	SELECT @MtrLngId = LNG_ID
	FROM TS_LANGUAGE
	WHERE LNG_ISO_CODE = @LngIsoCode
		AND LNG_DELETE_FLAG = 0

	IF @MtrLngId IS NULL
	BEGIN
		SET @errorMessage = 'SP: ' + @spName + ' - language not found: ' + cast(isnull(@LngIsoCode, 0) AS VARCHAR)

		RAISERROR (
				@errorMessage
				,16
				,1
				);

		RETURN 0
	END

	-- copyright lookup fk
	DECLARE @MtrCprId INT = NULL

	SELECT @MtrCprId = CPR_ID
	FROM TS_COPYRIGHT
	WHERE CPR_VALUE = @CprValue
		AND CPR_DELETE_FLAG = 0

	IF @MtrCprId IS NULL
	BEGIN
		SET @errorMessage = 'SP: ' + @spName + ' - source not found: ' + cast(isnull(@CprValue, 0) AS VARCHAR)

		RAISERROR (
				@errorMessage
				,16
				,1
				);

		RETURN 0
	END

	-- format fk lookup
	DECLARE @MtrFrmId INT = NULL

	-- We assume that the format has already been validated against the max version, so we accept the max version
	SELECT @MtrFrmId = (
			SELECT MAX(FRM_ID)
			FROM TS_FORMAT
			WHERE FRM_TYPE = @FrmType
			)

	IF @MtrFrmId IS NULL
	BEGIN
		SET @errorMessage = 'SP: ' + @spName + ' - format not type: ' + cast(isnull(@FrmType, 0) AS VARCHAR) + ', version: ' + cast(isnull(@FrmVersion, 0) AS VARCHAR)

		RAISERROR (
				@errorMessage
				,16
				,1
				);

		RETURN 0
	END

	DECLARE @DtgId INT = NULL

	EXEC @DtgId = Security_Auditing_Create @userName;

	IF @DtgId IS NULL
		OR @DtgId = 0
	BEGIN
		RAISERROR (
				'SP: Security_Auditing_Create has failed!'
				,16
				,1
				)

		RETURN 0
	END

	INSERT INTO [TD_MATRIX] (
		[MTR_CODE]
		,[MTR_TITLE]
		,[MTR_OFFICIAL_FLAG]
		,[MTR_INPUT]
		,[MTR_NOTE]
		,[MTR_RLS_ID]
		,[MTR_LNG_ID]
		,[MTR_FRM_ID]
		,[MTR_CPR_ID]
		,[MTR_DTG_ID]
		,[MTR_DELETE_FLAG]
		)
	VALUES (
		@MtrCode
		,@MtrTitle
		,@MtrOfficialFlag
		,@MtrInput
		,-- input file
		@MtrNote
		,@MtrRlsId
		,-- release
		@MtrLngId
		,@MtrFrmId
		,@MtrCprId
		,@DtgId
		,0
		)

	RETURN @@identity
END
