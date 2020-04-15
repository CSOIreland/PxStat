SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Paulo Patricio
-- Create date: 24 Oct 2018
-- Description:	Inserts a new record into the TD_RELEASE table
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Release_Create @RlsVersion INT
	,@RlsRevision INT
	,@RlsLiveFlag BIT = 0
	,@RlsLiveDateTimeFrom DATETIME = NULL
	,@RlsLiveDateTimeTo DATETIME = NULL
	,@RlsDependencyFlag BIT = 0
	,@RlsExceptionalFlag BIT = 0
	,@RlsReservationFlag BIT = 0
	,@RlsArchiveFlag BIT = 0
	,@RlsAnalyticalFlag BIT = 0
	,@GrpCode NVARCHAR(32)
	,@PrcCode NVARCHAR(32) = NULL
	,@CmmCode INT = NULL
	,@userName NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	-- Error Message
	DECLARE @errorMessage VARCHAR(256)
	DECLARE @RlsCmmID INT

	SET @RlsCmmID = (
			SELECT CMM_ID
			FROM TD_COMMENT
			WHERE CMM_CODE = @CmmCode
				AND CMM_DELETE_FLAG = 0
			)

	-- Group lookup fk
	DECLARE @RlsGrpId INT = NULL

	SELECT @RlsGrpId = [GRP_ID]
	FROM [TD_GROUP]
	WHERE GRP_DELETE_FLAG = 0
		AND [GRP_CODE] = @GrpCode

	IF @RlsGrpId IS NULL
	BEGIN
		SET @errorMessage = 'SP: ' + OBJECT_NAME(@@PROCID) + ' - group not found: ' + cast(isnull(@GrpCode, '') AS VARCHAR)

		RAISERROR (
				@errorMessage
				,16
				,1
				);

		RETURN 0
	END

	-- Product lookup fk
	DECLARE @RlsPrcId INT = NULL

	IF @PrcCode IS NOT NULL
	BEGIN
		SELECT @RlsPrcId = [PRC_ID]
		FROM [TD_PRODUCT]
		WHERE [PRC_DELETE_FLAG] = 0
			AND [PRC_CODE] = @PrcCode

		IF @RlsPrcId IS NULL
		BEGIN
			SET @errorMessage = 'SP: ' + OBJECT_NAME(@@PROCID) + ' - Product not found: ' + cast(isnull(@PrcCode, 0) AS VARCHAR)

			RAISERROR (
					@errorMessage
					,16
					,1
					);

			RETURN 0
		END
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

	INSERT INTO [TD_RELEASE] (
		[RLS_CODE]
		,[RLS_VERSION]
		,[RLS_REVISION]
		,[RLS_LIVE_FLAG]
		,[RLS_LIVE_DATETIME_FROM]
		,[RLS_LIVE_DATETIME_TO]
		,[RLS_DEPENDENCY_FLAG]
		,[RLS_EXCEPTIONAL_FLAG]
		,[RLS_RESERVATION_FLAG]
		,[RLS_ARCHIVE_FLAG]
		,[RLS_ANALYTICAL_FLAG]
		,[RLS_GRP_ID]
		,[RLS_PRC_ID]
		,[RLS_CMM_ID]
		,[RLS_DTG_ID]
		,[RLS_DELETE_FLAG]
		)
	VALUES (
		DEFAULT
		,@RlsVersion
		,@RlsRevision
		,@RlsLiveFlag
		,@RlsLiveDateTimeFrom
		,@RlsLiveDateTimeTo
		,@RlsDependencyFlag
		,@RlsExceptionalFlag
		,@RlsReservationFlag
		,@RlsArchiveFlag
		,@RlsAnalyticalFlag
		,@RlsGrpId
		,@RlsPrcId
		,@RlsCmmId
		,@DtgId
		,0
		)

	RETURN @@identity
END
GO


