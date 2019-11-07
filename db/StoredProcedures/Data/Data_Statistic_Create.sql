SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Paulo Patricio
-- Create date: 24 Sep 2018
-- Description:	Inserts a new record into the TD_STATISTIC table
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Statistic_Create @SttMtrId INT
	,@SttValue NVARCHAR(256)
	,@SttCode NVARCHAR(256)
	,@SttUnit NVARCHAR(256)
	,@SttDecimal TINYINT
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[TD_STATISTIC] (
		[STT_CODE]
		,[STT_VALUE]
		,[STT_UNIT]
		,[STT_MTR_ID]
		,[STT_DECIMAL]
		)
	VALUES (
		@SttCode
		,@SttValue
		,@SttUnit
		,@SttMtrId
		,@SttDecimal
		)

	RETURN @@identity
END
GO


