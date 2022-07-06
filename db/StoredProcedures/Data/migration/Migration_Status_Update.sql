SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 31/03/2022
-- Description: Change the migration status of a matrix
-- =============================================
CREATE
	OR

ALTER PROCEDURE Migration_Status_Update @MtrId INT
	,@MigrationFlag BIT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE TD_MATRIX
	SET MTR_MIGRATION_FLAG = @MigrationFlag
	WHERE MTR_ID = @MtrId
END
GO


