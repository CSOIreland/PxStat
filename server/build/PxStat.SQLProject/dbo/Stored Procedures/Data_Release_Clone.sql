
-- =============================================
-- Author:		Paulo Patricio
-- Create date: 27 Nov 2018
-- Description:	Clones a release record and related tables
-- =============================================
CREATE
	

 PROCEDURE Data_Release_Clone @RlsCode INT
	,@GrpCode NVARCHAR(32)
	,@userName NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	-- Error Message
	DECLARE @errorMessage VARCHAR(256)
	DECLARE @spName VARCHAR(100)

	SET @spName = 'Data_Stat_Release_Clone'

	DECLARE @RlsGrpId INT = NULL
	DECLARE @RlsId INT = NULL
	DECLARE @NewRlsId INT = NULL
	DECLARE @GrpId INT = NULL
	DECLARE @PreviousGrpCode VARCHAR(32) = NULL

	SELECT @RlsId = RLS_ID
	FROM TD_RELEASE
	WHERE RLS_DELETE_FLAG = 0
		AND RLS_CODE = @RlsCode

	IF @RlsId IS NULL
	BEGIN
		SET @errorMessage = 'SP: ' + @spName + ' - release code not found: ' + cast(isnull(@RlsCode, '') AS VARCHAR)

		RAISERROR (
				@errorMessage
				,16
				,1
				);

		RETURN - 1
	END

	SELECT @GrpId = GRP_ID
	FROM TD_GROUP
	WHERE GRP_DELETE_FLAG = 0
		AND GRP_CODE = @GrpCode

	IF @GrpId IS NULL
	BEGIN
		SET @errorMessage = 'SP: ' + @spName + ' - group code not found: ' + cast(isnull(@GrpCode, '') AS VARCHAR)

		RAISERROR (
				@errorMessage
				,16
				,1
				);

		RETURN - 2
	END

	-- Start the cloning!
	-- 1 get a new auditing 
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

	-- release
	INSERT INTO [TD_RELEASE] (
		[RLS_VERSION]
		,[RLS_REVISION]
		,[RLS_RESERVATION_FLAG]
		,[RLS_ARCHIVE_FLAG]
		,[RLS_ANALYTICAL_FLAG]
		,[RLS_EXPERIMENTAL_FLAG]
		,[RLS_GRP_ID]
		,[RLS_PRC_ID]
		,[RLS_DTG_ID]
		)
	SELECT [RLS_VERSION]
		,[RLS_REVISION] + 1
		,[RLS_RESERVATION_FLAG]
		,[RLS_ARCHIVE_FLAG]
		,[RLS_ANALYTICAL_FLAG]
		,[RLS_EXPERIMENTAL_FLAG]
		,@GrpId
		,[RLS_PRC_ID]
		,@DtgId
	FROM [TD_RELEASE]
	WHERE RLS_ID = @RlsId
		AND RLS_DELETE_FLAG = 0

	SET @NewRlsId = @@identity

	-- Update TD_RELEASE_PRODUCT for cloned release
	DECLARE @auditId AS INT = NULL;
    EXECUTE @auditId = Security_Auditing_Create @userName;
    IF @auditId IS NULL
       OR @auditId = 0
        BEGIN
            RAISERROR ('SP: Security_Auditing_Create has failed!', 16, 1);
            RETURN 0;
        END
	INSERT INTO TM_RELEASE_PRODUCT(RPR_RLS_ID, RPR_PRC_ID, RPR_DTG_ID, RPR_DELETE_FLAG)
	SELECT @NewRlsId, RPR_PRC_ID, @auditId, 0 FROM TM_RELEASE_PRODUCT
	WHERE RPR_PRC_ID IN (SELECT RPR_PRC_ID from TM_RELEASE_PRODUCT WHERE RPR_RLS_ID = @RlsId AND RPR_DELETE_FLAG = 0)
	AND RPR_RLS_ID = @RlsId

	RETURN @NewRlsId
END
