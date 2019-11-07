SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 16/11/2018
-- Description:	Updates the Release
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Release_Update @CcnUsername NVARCHAR(256)
	,@RlsCode INT
	,@RlsVersion INT
	,@RlsRevision INT
	,@RlsLiveFlag BIT
	,@RlsLiveDatetimeFrom DATETIME = NULL
	,@RlsLiveDatetimeTo DATETIME = NULL
	,@RlsDependencyFlag BIT
	,@RlsEmergencyFlag BIT
	,@RlsReservationFlag BIT
	,@RlsArchiveFlag BIT
	,@RlsAnalyticalFlag BIT
	,@PrcCode NVARCHAR(32) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DtgID INT
	DECLARE @eMessage VARCHAR(256)
	DECLARE @updateCount INT
	DECLARE @PrcID INT

	SET @PrcID = (
			SELECT PRC_ID
			FROM TD_PRODUCT
			WHERE PRC_CODE = @PrcCode
				AND PRC_DELETE_FLAG = 0
			)
	SET @DtgID = (
			SELECT RLS_DTG_ID
			FROM TD_RELEASE
			WHERE RLS_CODE = @RlsCode
				AND RLS_DELETE_FLAG = 0
			)

	IF @DtgID = 0
		OR @DtgID IS NULL
	BEGIN
		-- the requested record doesn't exist
		RETURN 0
	END

	UPDATE TD_RELEASE
	SET RLS_VERSION = @RlsVersion
		,RLS_REVISION = @RlsRevision
		,RLS_LIVE_FLAG = @RlsLiveFlag
		,RLS_LIVE_DATETIME_FROM = @RlsLiveDatetimeFrom
		,RLS_LIVE_DATETIME_TO = @RlsLiveDatetimeTo
		,RLS_DEPENDENCY_FLAG = @RlsDependencyFlag
		,RLS_EMERGENCY_FLAG = @RlsEmergencyFlag
		,RLS_RESERVATION_FLAG = @RlsReservationFlag
		,RLS_ARCHIVE_FLAG = @RlsArchiveFlag
		,RLS_ANALYTICAL_FLAG = @RlsAnalyticalFlag
		,RLS_PRC_ID = CASE 
			WHEN @PrcID > 0
				THEN @PrcID
			ELSE NULL
			END
	WHERE RLS_CODE = @RlsCode
		AND RLS_DELETE_FLAG = 0

	SET @updateCount = @@ROWCOUNT

	IF @updateCount > 0
	BEGIN
		-- Auditing:
		-- update record on auditing table
		DECLARE @AuditUpdateCount INT

		EXEC @AuditUpdateCount = Security_Auditing_Update @DtgID
			,@CcnUsername

		-- check the previous stored procedure for error
		IF @AuditUpdateCount = 0
		BEGIN
			SET @eMessage = 'Error creating entry in TD_AUDITING for Release update:' + cast(isnull(@RlsCode, 0) AS VARCHAR)

			RAISERROR (
					@eMessage
					,16
					,1
					)

			RETURN
		END
	END

	--Return the number of rows updated
	RETURN @updateCount
END
GO


