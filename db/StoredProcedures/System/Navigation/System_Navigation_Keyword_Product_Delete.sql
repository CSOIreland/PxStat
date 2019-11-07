SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Paulo Patricio
-- Create date: 16 Oct 2018
-- Description:	Delete a record from the TD_Keyword_Product table
-- exec System_Navigation_Keyword_Product_Delete null,'test1'

-- =============================================
CREATE
	OR

ALTER PROCEDURE System_Navigation_Keyword_Product_Delete @KprCode INT = NULL
	,@PrcCode NVARCHAR(32) = NULL
	,@KprMandatoryFlag BIT = NULL
AS
BEGIN
	--SET NOCOUNT ON;
	IF @KprCode IS NULL
		AND @PrcCode IS NULL
	BEGIN
		RETURN 0
	END

	DECLARE @DeleteCount INT
	DECLARE @PrcID INT = NULL
	DECLARE @errorMessage VARCHAR(256)

	IF @PrcCode IS NOT NULL
	BEGIN
		SET @PrcID = (
				SELECT PRC_ID
				FROM TD_PRODUCT
				WHERE PRC_CODE = @PrcCode
					AND PRC_DELETE_FLAG = 0
				)

		IF @PrcID IS NULL
		BEGIN
			BEGIN
				BEGIN
					SET @errorMessage = 'SP: ' + OBJECT_NAME(@@PROCID) + ' - product not found: ' + cast(isnull(@PrcCode, 0) AS VARCHAR)

					RAISERROR (
							@errorMessage
							,16
							,1
							);

					RETURN 0
				END
			END
		END
	END

	DELETE
	FROM [TD_KEYWORD_PRODUCT]
	WHERE (
			KPR_CODE = @KprCode
			OR @KprCode IS NULL
			)
		AND (
			KPR_PRC_ID = @PrcID
			OR @PrcID IS NULL
			)
		AND (
			KPR_MANDATORY_FLAG = @KprMandatoryFlag
			OR @KprMandatoryFlag IS NULL
			)

	SET @DeleteCount = @@ROWCOUNT

	-- Return the number of rows Deleted
	RETURN @DeleteCount
END
GO


