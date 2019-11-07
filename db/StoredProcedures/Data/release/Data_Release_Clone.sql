SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Paulo Patricio
-- Create date: 27 Nov 2018
-- Description:	Clones a release record and related tables
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Release_Clone @RlsCode INT
	,@GrpCode NVARCHAR(32)
	,@userName NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	-- Error Message
	DECLARE @errorMessage VARCHAR(256)
	DECLARE @spName VARCHAR(100) = 'Data_Stat_Release_Clone'
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
		,[RLS_DEPENDENCY_FLAG]
		,[RLS_RESERVATION_FLAG]
		,[RLS_ARCHIVE_FLAG]
		,[RLS_ANALYTICAL_FLAG]
		,[RLS_GRP_ID]
		,[RLS_PRC_ID]
		,[RLS_DTG_ID]
		)
	SELECT [RLS_VERSION]
		,[RLS_REVISION] + 1
		,[RLS_DEPENDENCY_FLAG]
		,[RLS_RESERVATION_FLAG]
		,[RLS_ARCHIVE_FLAG]
		,[RLS_ANALYTICAL_FLAG]
		,@GrpId
		,[RLS_PRC_ID]
		,@DtgId
	FROM [TD_RELEASE]
	WHERE RLS_ID = @RlsId
		AND RLS_DELETE_FLAG = 0

	SET @NewRlsId = @@identity

	-- KEYWORD_RELEASE
	INSERT INTO [TD_KEYWORD_RELEASE] (
		[KRL_VALUE]
		,[KRL_RLS_ID]
		)
	SELECT [KRL_VALUE]
		,@NewRlsId
	FROM [TD_KEYWORD_RELEASE]
	WHERE KRL_MANDATORY_FLAG = 0
		AND KRL_RLS_ID = @RlsId

	RETURN @NewRlsId
END
GO


