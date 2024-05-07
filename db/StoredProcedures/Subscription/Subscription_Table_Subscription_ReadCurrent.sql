SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 24/09/2021
-- Description:	Read all Table subscriptions for a user
-- exec Subscription_Table_Subscription_ReadCurrent null, 'okeeffene','ga','en'
-- =============================================
CREATE
	OR

ALTER PROCEDURE Subscription_Table_Subscription_ReadCurrent @SubscriberUserId NVARCHAR(256) = NULL
	,@CcnUsername NVARCHAR(256) = NULL
	,@LngIsoCode CHAR(2)
	,@LngIsoCodeDefault CHAR(2)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF @SubscriberUserId IS NULL
		AND @CcnUsername IS NULL
	BEGIN
		RETURN
	END

	DECLARE @UserId INT

	IF @SubscriberUserId IS NOT NULL
	BEGIN
		SET @UserId = (
				SELECT USR_ID
				FROM TD_USER
				INNER JOIN TD_SUBSCRIBER ON SBR_USR_ID = USR_ID
					AND SBR_UID = @SubscriberUserId
					AND SBR_DELETE_FLAG = 0
				)
	END
	ELSE
	BEGIN
		SET @UserId = (
				SELECT USR_ID
				FROM TD_USER
				INNER JOIN TD_ACCOUNT ON CCN_USR_ID = USR_ID
					AND CCN_USERNAME = @CcnUsername
					AND CCN_DELETE_FLAG = 0
				)
	END

	IF @UserId IS NULL
	BEGIN
		RETURN
	END

	--To ensure this sp will return data in the preferred langauge (where available)...
	IF @LngIsoCode=@LngIsoCodeDefault 
	BEGIN
	-- no difference between default and preferred languages

		SELECT TSB_TABLE AS RsbTable
			,MTR_TITLE AS MtrTitle
			,RLS_LIVE_DATETIME_FROM AS RlsDatetimeFrom
			,RLS_EXCEPTIONAL_FLAG AS RlsExceptionalFlag
		FROM TM_TABLE_SUBSCRIPTION
		INNER JOIN TD_MATRIX ON TSB_TABLE = MTR_CODE
		INNER JOIN VW_RELEASE_LIVE_NOW ON VRN_MTR_ID = MTR_ID
		INNER JOIN TD_RELEASE ON VRN_RLS_ID = RLS_ID
		INNER JOIN TS_LANGUAGE 
		ON MTR_LNG_ID=LNG_ID
		AND LNG_DELETE_FLAG=0
		WHERE TSB_USR_ID = @UserId
			AND TSB_DELETE_FLAG = 0
			AND LNG_ISO_CODE=@LngIsoCode 
	END
	ELSE
	BEGIN

	--Default and preferred languages are different
	-- If the data exists in the preferred language, return that, otherwise return the version in the default language
	-- we use a subquery for each language

	SELECT  
	qDefault.RsbTable,
	COALESCE(qPreferred.MtrTitle,qDefault.MtrTitle) as MtrTitle,
	qDefault.RlsDatetimeFrom,
	qDefault.RlsExceptionalFlag 
	FROM
	(
	-- Default Language Read
		SELECT TSB_TABLE AS RsbTable
		,MTR_TITLE AS MtrTitle
		,LNG_ISO_CODE 
		,RLS_LIVE_DATETIME_FROM AS RlsDatetimeFrom
		,RLS_EXCEPTIONAL_FLAG AS RlsExceptionalFlag
		,DTT_ID
		FROM TM_TABLE_SUBSCRIPTION
		INNER JOIN TD_MATRIX ON TSB_TABLE = MTR_CODE
		INNER JOIN VW_RELEASE_LIVE_NOW ON VRN_MTR_ID = MTR_ID
		INNER JOIN TD_RELEASE ON VRN_RLS_ID = RLS_ID
		INNER JOIN TD_DATASET 
		ON MTR_CODE=DTT_MTR_CODE 
		INNER JOIN TS_LANGUAGE 
		ON MTR_LNG_ID=LNG_ID 
		AND LNG_DELETE_FLAG=0
		AND LNG_ISO_CODE=@LngIsoCodeDefault
				WHERE TSB_USR_ID =@UserId
			AND TSB_DELETE_FLAG = 0) qDefault
		
		LEFT OUTER JOIN
		(

	--Preferred Language Read
		SELECT TSB_TABLE AS RsbTable
		,MTR_TITLE AS MtrTitle
		,LNG_ISO_CODE 
		,RLS_LIVE_DATETIME_FROM AS RlsDatetimeFrom
		,RLS_EXCEPTIONAL_FLAG AS RlsExceptionalFlag
		,DTT_ID 
		FROM TM_TABLE_SUBSCRIPTION
		INNER JOIN TD_MATRIX ON TSB_TABLE = MTR_CODE
		INNER JOIN VW_RELEASE_LIVE_NOW ON VRN_MTR_ID = MTR_ID
		INNER JOIN TD_RELEASE ON VRN_RLS_ID = RLS_ID
		INNER JOIN TD_DATASET 
		ON MTR_CODE=DTT_MTR_CODE
		INNER JOIN TS_LANGUAGE 
		ON MTR_LNG_ID=LNG_ID 
		AND LNG_DELETE_FLAG=0
		AND LNG_ISO_CODE=@LngIsoCode
				WHERE TSB_USR_ID =@UserId
			AND TSB_DELETE_FLAG = 0) qPreferred

		on qPreferred.DTT_ID=qDefault.DTT_ID 

	END
END
GO


