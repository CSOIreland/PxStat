SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 30/04/2021
-- Description:	Update a GeoLayer record
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_GeoLayer_Update @GlrName NVARCHAR(256)
	,@GlrCode NVARCHAR(256)
	,@CcnUsername NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DtgID INT

	SET @DtgID = (
			SELECT GLR_DTG_ID
			FROM TD_GEOLAYER
			WHERE GLR_CODE = @GlrCode
				AND GLR_DELETE_FLAG = 0
			)

	IF @DtgID IS NULL
	BEGIN
		RETURN 0
	END

	-- Do the create Audit and get the new DtgID from the stored procedure
	EXEC @DtgID = Security_Auditing_Update @DtgID
		,@CcnUsername

	-- Check for problems with the audit stored procedure
	IF @DtgID = 0
		OR @DtgID IS NULL
	BEGIN
		RAISERROR (
				'Error in calling Security_Auditing_Update'
				,16
				,1
				)

		RETURN 0
	END

	UPDATE TD_GEOLAYER
	SET GLR_NAME = @GlrName
	WHERE GLR_CODE = @GlrCode

	RETURN @@rowcount
END
GO


