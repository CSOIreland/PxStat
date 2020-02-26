SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 13/12/2018
-- Reads ReasonRelease entities based on RlsCode
-- exec Data_Reason_Release_Read 6,null,'ga'
-- =============================================
CREATE
	OR

ALTER PROCEDURE Data_Reason_Release_Read @RlsCode INT
	,@RsnCode NVARCHAR(32) = NULL
	,@LngIsoCode char(2)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @LngIsoId INT

	SET @LngIsoId = (
			SELECT LNG_ID
			FROM TS_LANGUAGE
			WHERE LNG_ISO_CODE = @LngIsoCode
				AND LNG_DELETE_FLAG = 0
			)

	SELECT rls.RLS_CODE AS RlsCode
		,rsn.RSN_CODE AS RsnCode
		,coalesce(rlg.RLG_VALUE_EXTERNAL , rsn.RSN_VALUE_EXTERNAL) AS RsnValueExternal
		,coalesce(rlg.RLG_VALUE_INTERNAL , rsn.RSN_VALUE_INTERNAL) AS RsnValueInternal
		,cmm.CMM_Value AS CmmValue
	FROM TM_REASON_RELEASE rsr
	INNER JOIN TS_REASON rsn
		ON rsr.RSR_RSN_ID = rsn.RSN_ID
			AND rsr.RSR_DELETE_FLAG = 0
	LEFT JOIN TD_COMMENT cmm
		ON cmm.CMM_ID = rsr.RSR_CMM_ID
			AND cmm.CMM_DELETE_FLAG = 0
	INNER JOIN TD_RELEASE rls
		ON rsr.RSR_RLS_ID = rls.RLS_ID
			AND rls.RLS_DELETE_FLAG = 0
	left join TS_REASON_LANGUAGE rlg
	on rsn.RSN_ID=rlg.RLG_RSN_ID 
	and rlg.RLG_LNG_ID=@LngIsoId 
	WHERE rls.RLS_CODE = @RlsCode
		AND (
			@RsnCode IS NULL
			OR @RsnCode = rsn.RSN_CODE
			)
END
GO


