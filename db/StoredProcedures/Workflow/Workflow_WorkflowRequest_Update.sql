SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 01/11/2018
-- Description:	Updates a Workflow Request. Specifically this means changing the wrq_current_flag to 1 or 0
-- =============================================
CREATE
	OR

ALTER PROCEDURE Workflow_WorkflowRequest_Update @CcnUsername NVARCHAR(256)
	,@RlsCode INT
	,@WrqCurrentFlag BIT
	,@WrqArchiveFlag BIT=NULL
	,@WrqReservationFlag BIT=NULL
	,@WrqExperimentalFlag BIT=NULL
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DtgID INT
	DECLARE @updateCount INT
	DECLARE @eMessage VARCHAR(256)

	SET @DtgID = (
			SELECT WRQ_DTG_ID
			FROM TD_WORKFLOW_REQUEST wrq
			INNER JOIN TD_RELEASE rls
				ON wrq.WRQ_RLS_ID = rls.RLS_ID
					AND rls.RLS_CODE = @RlsCode
					AND wrq.WRQ_DELETE_FLAG = 0
					AND rls.RLS_DELETE_FLAG = 0
					AND wrq.WRQ_CURRENT_FLAG = 1
			)

	UPDATE TD_WORKFLOW_REQUEST
	SET WRQ_CURRENT_FLAG = @WrqCurrentFlag
	,WRQ_ARCHIVE_FLAG=COALESCE(@WrqArchiveFlag,WRQ_ARCHIVE_FLAG)
	,WRQ_RESERVATION_FLAG=COALESCE(@WrqReservationFlag,WRQ_RESERVATION_FLAG)
	,WRQ_EXPERIMENTAL_FLAG =COALESCE(@WrqExperimentalFlag,WRQ_EXPERIMENTAL_FLAG)
	FROM TD_WORKFLOW_REQUEST wrq
	INNER JOIN TD_RELEASE rls
		ON wrq.WRQ_RLS_ID = rls.RLS_ID
			AND rls.RLS_CODE = @RlsCode
			AND wrq.WRQ_DELETE_FLAG = 0
			AND rls.RLS_DELETE_FLAG = 0

	SET @updateCount = @@ROWCOUNT

	IF @updateCount > 0
	BEGIN
		-- Auditing:
		DECLARE @AuditUpdateCount INT

		EXEC @AuditUpdateCount = Security_Auditing_Update @DtgID
			,@CcnUsername

		-- check the previous stored procedure for error
		IF @AuditUpdateCount = 0
		BEGIN
			SET @eMessage = 'Error creating entry in TD_AUDITING for Workflow_Request update - RlsCode:' + cast(isnull(@RlsCode, 0) AS VARCHAR)

			RAISERROR (
					@eMessage
					,16
					,1
					)

			RETURN
		END
	END

	RETURN @updateCount
END
GO


