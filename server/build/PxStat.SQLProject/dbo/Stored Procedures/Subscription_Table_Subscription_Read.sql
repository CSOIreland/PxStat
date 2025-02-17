
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 24/09/2021
-- Description:	Read all user subscriptions for a table name, or if no table name, all subscriptions for all tables
-- exec Subscription_Table_Subscription_Read 'HPM08'
-- =============================================
CREATE
	

 PROCEDURE Subscription_Table_Subscription_Read @TsbTable NVARCHAR(20) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT TSB_TABLE AS TsbTable
		,MTR_TITLE AS MtrTitle
		,CCN_USERNAME AS CcnUsername
		,SBR_UID AS SbrUserId
		,CCN_EMAIL AS CcnEmail
		,LNG_ISO_CODE AS LngIsoCode
		,RLS_LIVE_DATETIME_FROM AS RlsLiveDatetimeFrom
		,RLS_EXCEPTIONAL_FLAG AS RlsExceptionalFlag
	FROM TM_TABLE_SUBSCRIPTION
	INNER JOIN TD_USER ON USR_ID = TSB_USR_ID
	INNER JOIN TS_LANGUAGE ON USR_LNG_ID = LNG_ID
	INNER JOIN TD_MATRIX ON TSB_TABLE = MTR_CODE
	INNER JOIN VW_RELEASE_LIVE_NOW ON VRN_MTR_ID = MTR_ID
	INNER JOIN TD_RELEASE ON VRN_RLS_ID = RLS_ID
	LEFT JOIN TD_ACCOUNT ON USR_ID = CCN_USR_ID
	LEFT JOIN TD_SUBSCRIBER ON SBR_USR_ID = USR_ID
	LEFT JOIN TD_LOGIN ON LGN_CCN_ID = CCN_ID
	WHERE TSB_DELETE_FLAG = 0
		AND (
			@TsbTable IS NULL
			OR TSB_TABLE = @TsbTable
			)
END
