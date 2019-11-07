SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Paulo Patricio
-- Create date: 25 Sep 2018
-- Description:	Inserts a new record into the TD_PERIOD table
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Period_Create @PrdFrqId INT
	,@PrdValue NVARCHAR(256)
	,@PrdCode NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[TD_PERIOD] (
		[PRD_CODE]
		,[PRD_VALUE]
		,[PRD_FRQ_ID]
		)
	VALUES (
		@PrdCode
		,@PrdValue
		,@PrdFrqId
		)

	RETURN @@identity
END
GO


