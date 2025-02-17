
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 14/12/2018
-- Description:	Create an Alert
-- exec System_Navigation_Alert_Create 'A big Warning','A warning message','2018-12-31',1,'OKeeffeNe'
-- =============================================
CREATE
	

 PROCEDURE System_Navigation_Alert_Create @LrtMessage NVARCHAR(1024)
	,@LrtDatetime DATETIME
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

	INSERT INTO TD_ALERT (
		[LRT_MESSAGE]
		,[LRT_DATETIME]
		,[LRT_DTG_ID]
		,[LRT_DELETE_FLAG]
		)
	VALUES (
		@LrtMessage
		,@LrtDatetime
		,@DtgID
		,0
		)

	RETURN @@identity
END
