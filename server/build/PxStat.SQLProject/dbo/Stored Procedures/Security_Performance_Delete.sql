
-- =============================================
-- Author:		Liam Millar
-- Create date: 29/10/2020
-- Amended 17/04/2024 Neil O'Keeffe - pxstat user does not have truncate permission, change to delete
-- Description:	Delete performance entries
--
-- =============================================
CREATE
	

 PROCEDURE [dbo].[Security_Performance_Delete]
AS
BEGIN
	SET NOCOUNT ON;

	DELETE FROM TD_PERFORMANCE

	RETURN 1
END
