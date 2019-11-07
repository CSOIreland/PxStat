SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Paulo Patricio
-- Create date: 25 Sep 2018
-- Description:	Inserts a new record into the TD_STATISTIC table
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Classification_Create @ClsMtrId INT
	,@ClsValue NVARCHAR(256)
	,@ClsCode NVARCHAR(256)
	,@ClsGeoFlag BIT
	,@ClsGeoUrl NVARCHAR(2048) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[TD_CLASSIFICATION] (
		[CLS_CODE]
		,[CLS_VALUE]
		,[CLS_MTR_ID]
		,CLS_GEO_FLAG
		,CLS_GEO_URL
		)
	VALUES (
		@ClsCode
		,@ClsValue
		,@ClsMtrId
		,@ClsGeoFlag
		,@ClsGeoUrl
		)

	RETURN @@identity
END
GO


