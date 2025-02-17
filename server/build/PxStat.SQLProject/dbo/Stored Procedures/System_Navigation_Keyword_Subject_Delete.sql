
-- =============================================
-- Author:		Paulo Patricio
-- Create date: 16 Oct 2018
-- Description:	Delete a record on the TD_Keyword_Subject table
-- =============================================
CREATE
	

 PROCEDURE System_Navigation_Keyword_Subject_Delete @KsbCode INT = NULL
	,@SbjCode INT = NULL
	,@KsbMandatoryFlag BIT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF @KsbCode IS NULL
		AND @SbjCode IS NULL
	BEGIN
		RETURN 0
	END

	DECLARE @DeleteCount INT
	DECLARE @sbjID INT = NULL
	DECLARE @errorMessage VARCHAR(256)

	IF @SbjCode IS NOT NULL
	BEGIN
		SET @sbjID = (
				SELECT SBJ_ID
				FROM TD_SUBJECT
				WHERE SBJ_CODE = @SbjCode
					AND SBJ_DELETE_FLAG = 0
				)

		IF @sbjId IS NULL
		BEGIN
			BEGIN
				SET @errorMessage = 'SP: ' + OBJECT_NAME(@@PROCID) + ' - subject not found: ' + cast(isnull(@SbjCode, 0) AS VARCHAR)

				RAISERROR (
						@errorMessage
						,16
						,1
						);

				RETURN 0
			END
		END
	END

	DELETE
	FROM [TD_KEYWORD_SUBJECT]
	WHERE (
			[KSB_Code] = @KsbCode
			OR @KsbCode IS NULL
			)
		AND (
			KSB_SBJ_ID = @sbjID
			OR @sbjID IS NULL
			)
		AND (
			@KsbMandatoryFlag IS NULL
			OR @KsbMandatoryFlag = KSB_MANDATORY_FLAG
			)

	SET @DeleteCount = @@ROWCOUNT

	-- Return the number of rows Deleted
	RETURN @DeleteCount
END
