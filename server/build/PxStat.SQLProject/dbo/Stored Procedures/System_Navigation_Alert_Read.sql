
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 14/12/2018
-- Description:	Read an Alert
-- exec System_Navigation_Alert_Read 0,2
-- =============================================
CREATE
	

 PROCEDURE System_Navigation_Alert_Read @ReadLiveOnly BIT
	,@LrtCode INT = NULL
	,@LngIsoCode CHAR(2)
AS
BEGIN
	SET NOCOUNT ON;

	declare @LngId int
	declare @eMessage nvarchar(256)

		SET @LngId = (
			SELECT LNG_ID
			FROM TS_LANGUAGE
			WHERE LNG_ISO_CODE = @LngIsoCode
				AND LNG_DELETE_FLAG = 0
			)

	IF @LngID IS NULL
		OR @LngID = 0
	BEGIN
		SET @eMessage = 'No ID found for Language Code ' + cast(isnull(@LngIsoCode, 0) AS VARCHAR)

		RAISERROR (
				@eMessage
				,16
				,1
				)

		RETURN 0
	END

	SELECT lrt.LRT_CODE AS LrtCode
		,lrt.LRT_DATETIME AS LrtDatetime
		,Coalesce(LLN_MESSAGE,LRT_MESSAGE) AS LrtMessage
	FROM TD_ALERT lrt
	left join TD_ALERT_LANGUAGE 
	on lrt.LRT_ID=LLN_LRT_ID 
	and LLN_LNG_ID=@LngId
	WHERE (
			(
				@ReadLiveOnly = 1
				AND getdate() >= lrt.LRT_DATETIME
				)
			OR @ReadLiveOnly = 0
			)
		AND (
			@LrtCode IS NULL
			OR lrt.LRT_CODE = @LrtCode
			)
		AND lrt.LRT_DELETE_FLAG = 0
	ORDER BY lrt.LRT_DATETIME DESC
END
