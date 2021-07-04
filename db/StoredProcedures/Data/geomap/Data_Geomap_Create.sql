SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 29/04/2021
-- Description:	Create a GeoMap
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Geomap_Create @GmpCode NVARCHAR(32)
	,@GmpName NVARCHAR(256)
	,@GmpDescription NVARCHAR(MAX)
	,@GmpGeoJson NVARCHAR(MAX)
	,@GlrCode NVARCHAR(256)
	,@GmpFeatureCount INT
	,@CcnUsername NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @GlrId INT
	DECLARE @DtgID INT

	SET @GlrId = (
			SELECT GLR_ID
			FROM TD_GEOLAYER
			WHERE GLR_CODE = @GlrCode
			)

	IF @GlrId IS NULL
		OR @GlrId = 0
	BEGIN
		RETURN 0
	END

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

	INSERT INTO TD_GEOMAP (
		GMP_CODE
		,GMP_NAME
		,GMP_DESCRIPTION
		,GMP_GEOJSON
		,GMP_FEATURE_COUNT 
		,GMP_GLR_ID
		,GMP_DTG_ID
		)
	VALUES (
		@GmpCode
		,@GmpName
		,@GmpDescription
		,@GmpGeoJson
		,@GmpFeatureCount 
		,@GlrId
		,@DtgID
		)

	RETURN @@ROWCOUNT
END
GO


