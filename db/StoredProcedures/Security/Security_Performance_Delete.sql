SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Liam Millar
-- Create date: 29/10/2020
-- Description:	Delete performance entries
-- =============================================
CREATE
	OR

ALTER PROCEDURE [dbo].[Security_Performance_Delete]
AS
BEGIN
	SET NOCOUNT ON;

	TRUNCATE TABLE TD_PERFORMANCE

	RETURN 1
END
GO


