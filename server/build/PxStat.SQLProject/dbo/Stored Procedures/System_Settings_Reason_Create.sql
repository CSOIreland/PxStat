
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 12/10/2018
-- Description:	Creates a new Reason entity
-- =============================================
CREATE
	

 PROCEDURE System_Settings_Reason_Create @CcnUsername NVARCHAR(256)
	,@RsnCode NVARCHAR(32)
	,@RsnValueInternal NVARCHAR(256)
	,@RsnValueExternal NVARCHAR(256)
	,@LngIsoCode char(2)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DtgID INT = NULL
	DECLARE @lngID INT = NULL
	DECLARE @errorMessage VARCHAR(256)

		SET @lngID = (
			SELECT LNG_ID
			FROM TS_LANGUAGE
			WHERE LNG_ISO_CODE = @LngIsoCode
				AND LNG_DELETE_FLAG = 0
			)

	IF @lngId IS NULL
	BEGIN
		SET @errorMessage = 'SP: ' + OBJECT_NAME(@@PROCID) + ' - language not found: ' + cast(isnull(@LngIsoCode, 0) AS VARCHAR)

		RAISERROR (
				@errorMessage
				,16
				,1
				);

		RETURN 0
	END

	-- Do the create Audit and get the new DtgID from the stored procedure
	EXEC @DtgID = Security_Auditing_Create @CcnUsername

	-- Check for problems with the audit stored procedure
	IF @DtgID = 0
		OR @DtgID IS NULL
	BEGIN
		RAISERROR (
				'Error in calling Security_Auditing_Create'
				,16
				,1
				)

		RETURN 0
	END

	INSERT INTO TS_REASON (
		RSN_CODE
		,RSN_VALUE_INTERNAL
		,RSN_VALUE_EXTERNAL
		,RSN_LNG_ID 
		,RSN_DTG_ID
		,RSN_DELETE_FLAG
		)
	VALUES (
		@RsnCode
		,@RsnValueInternal
		,@RsnValueExternal
		,@lngID 
		,@DtgID
		,0
		)

	RETURN @@IDENTITY
END
