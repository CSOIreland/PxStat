SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 17/04/2019
-- Description:	Reads variables based on classification ID
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Variable_Read @ClsID INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT VRB_ID AS VrbID
		,VRB_CODE AS VrbCode
		,VRB_VALUE AS VrbValue
	FROM TD_VARIABLE
	WHERE VRB_CLS_ID = @ClsID
END
GO


