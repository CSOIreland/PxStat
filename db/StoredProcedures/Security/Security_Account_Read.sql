SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 02/10/2018
-- Description:	Read Account details
-- exec Security_Account_Read 'jmurphy@hse.ie'
-- =============================================
CREATE
	OR

ALTER PROCEDURE Security_Account_Read @CcnUsername NVARCHAR(256) = NULL
	,@PrvCode NVARCHAR(32) = NULL
	,@AdFlag BIT=NULL
	,@CcnEmail NVARCHAR(256) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT ccn.CCN_USERNAME AS CcnUsername
		,prv.PRV_CODE AS PrvCode
		,prv.PRV_VALUE AS PrvValue
		,ccn.CCN_NOTIFICATION_FLAG as CcnNotificationFlag
		,ccn.CCN_DISPLAYNAME as CcnDisplayName
		,ccn.CCN_EMAIL as CcnEmail
		,CCN_ID as CcnId
		,CCN_AD_FLAG as CcnAdFlag
		,CCN_LOCKED_FLAG as CcnLockedFlag
		,LGN_2FA as Lgn2Fa
	FROM TD_ACCOUNT ccn
	INNER JOIN TS_PRIVILEGE prv
		ON ccn.CCN_PRV_ID = prv.PRV_ID
			AND ccn.CCN_DELETE_FLAG = 0
	LEFT JOIN TD_LOGIN
	ON CCN_ID=LGN_CCN_ID 
	WHERE (
			@CcnUsername IS NULL
			OR ccn.CCN_USERNAME = @CcnUsername
			)
		AND (
			@PrvCode IS NULL
			OR prv.PRV_CODE = @PrvCode
			)
		AND
		(
			@AdFlag IS NULL
			OR CCN_AD_FLAG=@AdFlag 
		)
		AND
		(
			@CcnEmail IS NULL
			OR CCN_EMAIL=@CcnEmail 
		)
	ORDER BY ccn.CCN_USERNAME 
END
