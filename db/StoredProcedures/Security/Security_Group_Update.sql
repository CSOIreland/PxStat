SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 01/10/2018
-- Description:	To update a group entity. 
-- =============================================
CREATE
	OR

ALTER PROCEDURE Security_Group_Update @GrpCodeOld NVARCHAR(32)
	,@GrpCodeNew NVARCHAR(32)
	,@GrpName NVARCHAR(256)
	,@GrpContactName NVARCHAR(256)=NULL 
	,@GrpContactPhone NVARCHAR(256)=NULL
	,@GrpContactEmail NVARCHAR(256) 
	,@UpdateCcnUsername NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DtgID INT
	DECLARE @eMessage NVARCHAR(256)
	DECLARE @updateCount INT

	SET @DtgID = (
			SELECT GRP_DTG_ID
			FROM TD_GROUP
			WHERE GRP_CODE = @GrpCodeOld
				AND GRP_DELETE_FLAG = 0
			)

	IF @DtgID = 0
		OR @DtgID IS NULL
	BEGIN
		-- the requested record doesn't exist
		RETURN 0
	END

	-- update Table
	UPDATE TD_GROUP
	SET GRP_CODE = @GrpCodeNew
		,GRP_NAME = @GrpName
		,GRP_CONTACT_NAME = @GrpContactName
		,GRP_CONTACT_PHONE = @GrpContactPhone
		,GRP_CONTACT_EMAIL = @GrpContactEmail
	WHERE GRP_CODE = @GrpCodeOld
		AND GRP_DELETE_FLAG = 0

	SET @updateCount = @@ROWCOUNT

	IF @updateCount > 0
	BEGIN
		-- Auditing:
		-- update record on auditing table
		DECLARE @AuditUpdateCount INT

		EXEC @AuditUpdateCount = Security_Auditing_Update @DtgID
			,@UpdateCcnUsername

		-- check the previous stored procedure for error
		IF @AuditUpdateCount = 0
		BEGIN
			SET @eMessage = 'Error creating entry in TD_AUDITING for Group update:' + cast(isnull(@GrpCodeOld, 0) AS NVARCHAR)

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


