
-- =============================================
-- Author:		Paulo Patricio
-- Create date: 16 Oct 2018
-- Description:	Inserts a new record into the TD_Keyword_Product table
-- =============================================
CREATE
	

 PROCEDURE System_Navigation_Keyword_Product_Create @KprValue NVARCHAR(256)
	,@PrcCode NVARCHAR(32)
	,@KprSingularisedFlag BIT
	,@KprMandatoryFlag BIT = 0
AS
BEGIN
	SET NOCOUNT ON;

	-- Error Message
	DECLARE @errorMessage VARCHAR(256)
	DECLARE @spName VARCHAR(100)

	SET @spName = 'System_Navigation_Keyword_Product_Create'

	DECLARE @ProductId INT = NULL

	SELECT @ProductId = p.PRC_ID
	FROM TD_Product p
	WHERE p.PRC_CODE = @PrcCode

	IF @ProductId IS NULL
	BEGIN
		SET @errorMessage = 'SP: ' + @spName + ' - Product not found: ' + cast(isnull(@PrcCode, 0) AS VARCHAR)

		RAISERROR (
				@errorMessage
				,16
				,1
				);

		RETURN 0
	END

	--Prevent duplicate values in the Keyword Product table
	DECLARE @KprValueCount INT

	SET @KprValueCount = (
			SELECT COUNT(*)
			FROM TD_KEYWORD_PRODUCT
			WHERE KPR_PRC_ID = @ProductId
				AND KPR_VALUE = @KprValue
			)

	IF @KprValueCount > 0
	BEGIN
		RETURN - 1
	END

	INSERT INTO [TD_KEYWORD_PRODUCT] (
		[KPR_CODE]
		,[KPR_VALUE]
		,[KPR_PRC_ID]
		,[KPR_MANDATORY_FLAG]
		,[KPR_SINGULARISED_FLAG] 
		)
	VALUES (
		DEFAULT
		,@KprValue
		,@ProductId
		,@KprMandatoryFlag
		,@KprSingularisedFlag
		)

	RETURN @@identity
END
