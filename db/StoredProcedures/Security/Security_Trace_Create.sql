SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 10/10/2018
-- Description:	Create a Trace entry
-- =============================================
CREATE
	OR

ALTER PROCEDURE Security_Trace_Create @TrcMethod NVARCHAR(256)
	,@TrcParams NVARCHAR(2048)
	,@TrcIp VARCHAR(15)
	,@TrcUseragent VARCHAR(2048)
	,@CcnUsername NVARCHAR(256) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO TD_TRACE (
		TRC_METHOD
		,TRC_PARAMS
		,TRC_IP
		,TRC_USERAGENT
		,TRC_USERNAME
		,TRC_DATETIME
		)
	VALUES (
		@TrcMethod
		,@TrcParams
		,@TrcIp
		,@Trcuseragent
		,@CcnUsername
		,getdate()
		)

	RETURN @@Identity
END
GO


