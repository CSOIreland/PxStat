SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 12/10/2018
-- Description:	Reads one or all Reasons. If the @RlsCode Parameter is null then all reasons will be returned for that release
-- If the @RsnCode is supplied, the reason for that code will be returned
-- otherwise all Reasons will be returned
-- exec System_Settings_Reason_Read null,null,'ga'
-- =============================================
CREATE
	OR

ALTER PROCEDURE System_Settings_Reason_Read @RlsCode INT = NULL
	,@RsnCode NVARCHAR(32) = NULL
	,@LngIsoCode char(2)
AS
BEGIN
	SET NOCOUNT ON;

	Declare @LngIsoID int
	DECLARE @eMessage VARCHAR(256)

		SET @LngIsoID = (
			SELECT lng.LNG_ID
			FROM TS_LANGUAGE lng
			WHERE lng.LNG_ISO_CODE = @LngIsoCode
				AND lng.LNG_DELETE_FLAG = 0
			)

	IF @LngIsoID IS NULL
		OR @LngIsoID = 0
	BEGIN
		SET @eMessage = 'No ID found for Language Code code ' + cast(isnull(@LngIsoCode, 0) AS VARCHAR)

		RAISERROR (
				@eMessage
				,16
				,1
				)

		RETURN 0
	END


	SELECT DISTINCT rsn.RSN_CODE AS RsnCode
		,coalesce(rlg.RLG_VALUE_EXTERNAL, rsn.RSN_VALUE_EXTERNAL) AS RsnValueExternal
		,coalesce(rlg.RLG_VALUE_INTERNAL,rsn.RSN_VALUE_INTERNAL) AS RsnValueInternal
	FROM TS_REASON rsn
	LEFT JOIN TM_REASON_RELEASE rsr
		ON rsn.RSN_ID = rsr.RSR_RSN_ID
			AND rsr.RSR_DELETE_FLAG = 0
			AND rsn.RSN_DELETE_FLAG = 0
	LEFT JOIN TD_RELEASE rls
		ON rsr.RSR_RLS_ID = rls.RLS_ID
			AND rls.RLS_DELETE_FLAG = 0
	left join TS_REASON_LANGUAGE rlg
		on rsn.RSN_ID=rlg.RLG_RSN_ID 
		and rlg.RLG_LNG_ID=@LngIsoID 
	WHERE (
			@RlsCode IS NULL
			OR rls.RLS_CODE = @RlsCode
			)
		AND (
			@RsnCode IS NULL
			OR rsn.RSN_CODE = @RsnCode
			)
		AND rsn.RSN_DELETE_FLAG = 0
END
GO


