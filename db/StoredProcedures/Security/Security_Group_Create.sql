SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 01/10/2018
-- Description:	To create a new Group entry
-- =============================================
CREATE
	OR

ALTER PROCEDURE Security_Group_Create @GrpCode NVARCHAR(32)
	,@GrpName NVARCHAR(256)
	,@GrpContactName NVARCHAR(256) = NULL
	,@GrpContactPhone NVARCHAR(256) = NULL
	,@GrpContactEmail NVARCHAR(256) = NULL
	,@CcnUsername NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DtgID INT = NULL

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

	INSERT INTO TD_GROUP (
		GRP_NAME
		,GRP_CONTACT_NAME
		,GRP_CONTACT_PHONE
		,GRP_CONTACT_EMAIL
		,GRP_CODE
		,GRP_DTG_ID
		,GRP_DELETE_FLAG
		)
	VALUES (
		@GrpName
		,@GrpContactName
		,@GrpContactPhone
		,@GrpContactEmail
		,@GrpCode
		,@DtgID
		,0
		)

	RETURN @@IDENTITY
END
GO


