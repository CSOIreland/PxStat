SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Paulo Patricio
-- Create date: 25 Sep 2018
-- Description:	Inserts a new record into the TD_VARIABLE table
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Variable_Create @VrbClsId INT
	,@VrbValue NVARCHAR(256)
	,@VrbCode NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[TD_VARIABLE] (
		[VRB_CODE]
		,[VRB_VALUE]
		,[VRB_CLS_ID]
		)
	VALUES (
		@VrbCode
		,@VrbValue
		,@VrbClsId
		)

	RETURN @@identity
END
GO


