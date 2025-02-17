-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 04/10/2024
-- Description:	Clones a release to a specific version
-- =============================================

CREATE PROCEDURE [dbo].[Data_Release_Clone_ToVersion] @RlsCode INT
	,@Version INT
	,@Revision INT
	,@UserName NVARCHAR(256)
AS
	DECLARE @RlsId INT = NULL
	DECLARE @errorMessage VARCHAR(256)
	DECLARE @NewRlsId INT = NULL

	SELECT @RlsId = RLS_ID
	FROM TD_RELEASE
	WHERE RLS_DELETE_FLAG = 0
		AND RLS_CODE = @RlsCode

	IF @RlsId IS NULL
	BEGIN
		SET @errorMessage = 'SP: ' + OBJECT_NAME(@@PROCID) + ' - release code not found: ' + cast(isnull(@RlsCode, '') AS VARCHAR)

		RAISERROR (
				@errorMessage
				,16
				,1
				);

		RETURN - 1
	END

	DECLARE @DtgId INT = NULL

	EXEC @DtgId = Security_Auditing_Create @UserName;

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

		INSERT INTO [TD_RELEASE] (
		[RLS_VERSION]
		,[RLS_REVISION]
		,[RLS_LIVE_FLAG]
		,[RLS_RESERVATION_FLAG]
		,[RLS_ARCHIVE_FLAG]
		,[RLS_ANALYTICAL_FLAG]
		,[RLS_EXPERIMENTAL_FLAG]
		,[RLS_GRP_ID]
		,[RLS_PRC_ID]
		,[RLS_DTG_ID]
		)
	SELECT @Version
		,@Revision
		,0
		,[RLS_RESERVATION_FLAG]
		,[RLS_ARCHIVE_FLAG]
		,[RLS_ANALYTICAL_FLAG]
		,[RLS_EXPERIMENTAL_FLAG]
		,[RLS_GRP_ID]
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

RETURN 0
