SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 10/05/2021
-- Description:	Update a GeoMap
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Geomap_Update @GmpCode NVARCHAR(32)
	,@GmpName NVARCHAR(256)
	,@GmpDescription NVARCHAR(MAX)
	,@CcnUsername NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DtgID INT

	SET @DtgID = (
			SELECT GMP_DTG_ID
			FROM TD_GEOMAP
			WHERE GMP_CODE = @GmpCode
			)

	-- Check for problems with the audit stored procedure
	IF @DtgID = 0
		OR @DtgID IS NULL
	BEGIN
		RAISERROR (
				'Unknown GeoMap Code'
				,16
				,1
				)

		RETURN 0
	END

	-- Do the create Audit and get the new DtgID from the stored procedure
	EXEC @DtgID = Security_Auditing_Update @DtgID
		,@CcnUsername

	UPDATE TD_GEOMAP
	SET GMP_NAME = @GmpName
		,GMP_DESCRIPTION = @GmpDescription
	WHERE GMP_CODE = @GmpCode

	RETURN @@ROWCOUNT
END
GO


