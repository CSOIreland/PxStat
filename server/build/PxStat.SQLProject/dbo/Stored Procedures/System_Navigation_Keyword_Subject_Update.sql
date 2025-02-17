
-- =============================================
-- Author:		Paulo Patricio
-- Create date: 16 Oct 2018
-- Description:	Update a record on the TD_Keyword_Subject table
-- =============================================
CREATE
	

 PROCEDURE System_Navigation_Keyword_Subject_Update @KsbCode INT
	,@KsbValue NVARCHAR(256)
	,@KsbSingularisedFlag BIT
	,@KsbMandatoryFlag BIT = 0
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @updateCount INT
	--Prevent duplicate values in the Keyword Subject table
	DECLARE @KsbValueCount INT
	DECLARE @KsbSbjId INT

	SET @KsbSbjId = (
			SELECT KSB_SBJ_ID
			FROM TD_KEYWORD_SUBJECT
			WHERE KSB_CODE = @KsbCode
			)
	SET @KsbValueCount = (
			SELECT COUNT(*)
			FROM TD_KEYWORD_SUBJECT
			WHERE KSB_SBJ_ID = @KsbSbjId
				AND KSB_VALUE = @KsbValue
				AND KSB_CODE <> @KsbCode
			)

	IF @KsbValueCount > 0
	BEGIN
		RETURN - 1
	END

	UPDATE [TD_KEYWORD_SUBJECT]
	SET [KSB_VALUE] = @KsbValue
		,[KSB_MANDATORY_FLAG] = @KsbMandatoryFlag
		,KSB_SINGULARISED_FLAG=@KsbSingularisedFlag 
	WHERE [KSB_Code] = @KsbCode

	SET @updateCount = @@ROWCOUNT

	-- Return the number of rows Updated
	RETURN @updateCount
END
