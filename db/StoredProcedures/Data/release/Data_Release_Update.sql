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
	,@RlsExceptionalFlag BIT
	,@RlsReservationFlag BIT
	,@RlsArchiveFlag BIT
	,@RlsExperimentalFlag BIT
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
		,RLS_EXCEPTIONAL_FLAG = @RlsExceptionalFlag
		,RLS_RESERVATION_FLAG = @RlsReservationFlag
		,RLS_ARCHIVE_FLAG = @RlsArchiveFlag
		,RLS_ANALYTICAL_FLAG = @RlsAnalyticalFlag
		,RLS_EXPERIMENTAL_FLAG = @RlsExperimentalFlag
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

		IF @PrcCode IS NOT NULL
    BEGIN
        DECLARE @ReleaseId AS INT;
        DECLARE @ProductId AS INT;
        DECLARE @Count AS INT;
        SELECT @ReleaseId = RLS_ID
        FROM   TD_RELEASE
        WHERE  RLS_CODE = @RlsCode;

        SELECT @ProductId = PRC_ID
        FROM   TD_PRODUCT
        WHERE  PRC_CODE = @PrcCode;

        SELECT @Count = COUNT(*)
        FROM   TM_RELEASE_PRODUCT
        WHERE  RPR_RLS_ID = @ReleaseId
                AND RPR_PRC_ID = @ProductId
                AND RPR_DELETE_FLAG = 0;
        IF @Count > 0
            BEGIN
                DECLARE @auditId AS INT = NULL;
                EXECUTE @auditId = Security_Auditing_Create @CcnUsername;
                IF @auditId IS NULL
                    OR @auditId = 0
                    BEGIN
                        RAISERROR ('SP: Security_Auditing_Create has failed!', 16, 1);
                        RETURN 0;
                    END
                UPDATE TM_RELEASE_PRODUCT
                SET    RPR_DELETE_FLAG = 1,
                        RPR_DTG_ID      = @auditId
                WHERE  RPR_RLS_ID = @ReleaseId
                        AND RPR_PRC_ID = @ProductId
                        AND RPR_DELETE_FLAG = 0;
            END
    END

	--Return the number of rows updated
	RETURN @updateCount
END
GO


