SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Paulo Patricio
-- Create date: 16 Oct 2018
-- Description:	Update a record on the TD_Keyword_Product table
-- =============================================
CREATE
	OR

ALTER PROCEDURE System_Navigation_Keyword_Product_Update @KprCode INT
	,@KprValue NVARCHAR(256)
	,@KprSingularisedFlag BIT
	,@KprMandatoryFlag BIT = 0
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @updateCount INT
	--Prevent duplicate values in the Keyword Product table
	DECLARE @KprValueCount INT
	DECLARE @KprPrcID INT

	SET @KprPrcID = (
			SELECT KPR_PRC_ID
			FROM TD_KEYWORD_PRODUCT
			WHERE KPR_CODE = @KprCode
			)
	SET @KprValueCount = (
			SELECT COUNT(*)
			FROM TD_KEYWORD_PRODUCT
			WHERE KPR_PRC_ID = @KprPrcID
				AND KPR_VALUE = @KprValue
				AND KPR_CODE <> @KprCode
			)

	IF @KprValueCount > 0
	BEGIN
		RETURN - 1
	END

	UPDATE [TD_KEYWORD_PRODUCT]
	SET [KPR_VALUE] = @KprValue
		,[KPR_MANDATORY_FLAG] = @KprMandatoryFlag
		,KPR_SINGULARISED_FLAG=@KprSingularisedFlag 
	WHERE KPR_CODE = @KprCode

	SET @updateCount = @@ROWCOUNT

	-- Return the number of rows Updated
	RETURN @updateCount
END
GO


