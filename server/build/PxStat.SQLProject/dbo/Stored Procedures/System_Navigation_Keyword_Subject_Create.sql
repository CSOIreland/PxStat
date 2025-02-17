
-- =============================================
-- Author:		Paulo Patricio
-- Create date: 16 Oct 2018
-- Description:	Inserts a new record into the TD_Keyword_Subject table
-- =============================================
CREATE
	

 PROCEDURE System_Navigation_Keyword_Subject_Create @KsbValue NVARCHAR(256)
	,@SbjCode INT
	,@KsbSingularisedFlag BIT
	,@KsbMandatoryFlag BIT = 0
	
AS
BEGIN
	SET NOCOUNT ON;

	-- Error Message
	DECLARE @errorMessage VARCHAR(256)
	DECLARE @spName VARCHAR(100)

	SET @spName = 'System_Navigation_Keyword_Subject_Create'

	DECLARE @subjectId INT = NULL

	SELECT @subjectId = s.SBJ_ID
	FROM TD_SUBJECT s
	WHERE s.SBJ_CODE = @SbjCode

	IF @subjectId IS NULL
	BEGIN
		SET @errorMessage = 'SP: ' + @spName + ' - subject not found: ' + cast(isnull(@SbjCode, 0) AS VARCHAR)

		RAISERROR (
				@errorMessage
				,16
				,1
				);

		RETURN 0
	END

	--Prevent duplicate values in the Keyword Subject table
	DECLARE @KsbValueCount INT

	SET @KsbValueCount = (
			SELECT COUNT(*)
			FROM TD_KEYWORD_SUBJECT
			WHERE KSB_SBJ_ID = @subjectId
				AND KSB_VALUE = @KsbValue
			)

	IF @KsbValueCount > 0
	BEGIN
		RETURN - 1
	END

	INSERT INTO [TD_KEYWORD_SUBJECT] (
		[KSB_CODE]
		,[KSB_VALUE]
		,[KSB_SBJ_ID]
		,[KSB_MANDATORY_FLAG]
		,[KSB_SINGULARISED_FLAG]
		)
	VALUES (
		DEFAULT
		,@KsbValue
		,@subjectId
		,@KsbMandatoryFlag
		,@KsbSingularisedFlag 
		)

	RETURN @@identity
END
