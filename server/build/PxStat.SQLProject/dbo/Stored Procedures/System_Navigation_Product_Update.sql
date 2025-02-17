
-- =============================================
-- Author:		Paulo Patricio
-- Update date: 15 Oct 2018
-- Description:	Updates record(s) from the TD_Product table
-- =============================================
CREATE
	

 PROCEDURE System_Navigation_Product_Update @PrcCode NVARCHAR(32)
	,@PrcCodeNew NVARCHAR(32)
	,@SbjCode INT
	,@PrcValue NVARCHAR(256)
	,@IsDefault BIT = 1
	,@userName NVARCHAR(256)
	,@ProductID INT OUT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @eMessage VARCHAR(256)
	DECLARE @updateCount INT
	DECLARE @PrcID INT = NULL
	DECLARE @DtgID INT = NULL
	DECLARE @SbjId INT = NULL

	--check if product record exists 
	SELECT @PrcID = PRC_ID
		,@DtgID = PRC_DTG_ID
	FROM TD_Product
	WHERE PRC_CODE = @PrcCode
		AND PRC_Delete_FLAG = 0

	IF @PrcID IS NULL
	BEGIN
		RETURN 0
	END

	--check if subject record exists 
	SELECT @SbjId = SBJ_ID
	FROM TD_SUBJECT
	WHERE SBJ_CODE = @SbjCode
		AND SBJ_DELETE_FLAG = 0

	IF @SbjId IS NULL
	BEGIN
		RETURN 0
	END

	UPDATE TD_Product
	SET PRC_SBJ_ID = @SbjId
		,PRC_CODE = @PrcCodeNew
		,PRC_VALUE = (
			CASE 
				WHEN (@IsDefault = 1)
					THEN @PrcValue
				ELSE PRC_VALUE
				END
			)
	WHERE PRC_ID = @PrcID
		AND PRC_DELETE_FLAG = 0

	SET @updateCount = @@ROWCOUNT

	IF @updateCount > 0
	BEGIN
		-- do the auditing 
		-- Create the entry in the TD_AUDITING table
		DECLARE @AuditUpdateCount INT

		EXEC @AuditUpdateCount = Security_Auditing_Update @DtgID
			,@userName

		-- check the previous stored procedure for error
		IF @AuditUpdateCount = 0
		BEGIN
			SET @eMessage = 'Error creating entry in TD_AUDITING for Product Update:' + cast(isnull(@PrcCode, 0) AS VARCHAR)

			RAISERROR (
					@eMessage
					,16
					,1
					)

			RETURN
		END
	END

	SET @ProductID = @PrcID

	-- Return the number of rows Updated
	RETURN @updateCount
END
