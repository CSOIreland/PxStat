SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 05/05/2021
-- Description:	Soft delete a GeoMap
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_GeoMap_Delete @GmpCode NVARCHAR(32)
	,@CcnUsername NVARCHAR(256)
	,@UrlStub NVARCHAR(2048)
AS
BEGIN
	DECLARE @DtgId INT

	SET @DtgId = (
			SELECT GMP_DTG_ID
			FROM TD_GEOMAP
			WHERE GMP_CODE = @GmpCode
			)

	IF @DtgID = 0
		OR @DtgID IS NULL
	BEGIN
		RETURN 0
	END

	-- Do the create Audit and get the new DtgID from the stored procedure
	EXEC @DtgID = Security_Auditing_Delete @DtgID
		,@CcnUsername

	-- Check for problems with the audit stored procedure
	IF @DtgID = 0
		OR @DtgID IS NULL
	BEGIN
		RAISERROR (
				'Error in calling Security_Auditing_Delete'
				,16
				,1
				)

		RETURN 0
	END

	DECLARE @Url NVARCHAR(2048)

	--We can't delete where there are data dimensions (classifications) associated with the map
	SET @Url = @UrlStub  + @GmpCode

	IF (
			SELECT COUNT(*)
			FROM TD_MATRIX_DIMENSION 
			INNER JOIN TD_MATRIX ON MDM_MTR_ID = MTR_ID
				AND MTR_DELETE_FLAG = 0
				AND MDM_GEO_URL = @Url
			) > 0
	BEGIN
		RETURN 0
	END

	UPDATE TD_GEOMAP
	SET GMP_DELETE_FLAG = 1
	WHERE GMP_CODE = @GmpCode
		AND GMP_DELETE_FLAG = 0

	RETURN @@ROWCOUNT
END
GO


