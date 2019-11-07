SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Paulo Patricio
-- Create date: 25 Sep 2018
-- Description:	Inserts a new record into the TD_FREQUENCY table
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Frequency_Create @FrqValue NVARCHAR(256)
	,@FrqCode NVARCHAR(256)
	,@FrqMtrId INT
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[TD_FREQUENCY] (
		[FRQ_CODE]
		,[FRQ_VALUE]
		,FRQ_MTR_ID
		)
	VALUES (
		@FrqCode
		,@FrqValue
		,@FrqMtrId
		)

	RETURN @@identity
END
GO


