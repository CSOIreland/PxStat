
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 30/04/2021
-- Description:	Create a GeoLayer
-- =============================================
CREATE
	

 PROCEDURE Data_GeoLayer_Create @GlrName NVARCHAR(256)
	
	,@CcnUsername NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DtgID INT

	DECLARE @GlrCode NVARCHAR(256)
	SET @GlrCode=CONCAT('L',(SELECT MAX(GLR_ID) FROM TD_GEOLAYER)+1)

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

	INSERT INTO TD_GEOLAYER (
		GLR_NAME
		,GLR_CODE
		,GLR_DTG_ID
		,GLR_DELETE_FLAG
		)
	VALUES (
		@GlrName
		,@GlrCode
		,@DtgID
		,0
		)

	RETURN @@rowcount
END
