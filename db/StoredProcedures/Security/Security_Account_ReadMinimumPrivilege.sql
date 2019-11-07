SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 19/08/2019
-- Description:	Read Account details for the privilege code and for higher privileges
-- Get users of the specified privilege and higher
-- exec Security_Account_ReadMinimumPrivilege 'MODERATOR',1
-- =============================================
CREATE
	OR

ALTER PROCEDURE Security_Account_ReadMinimumPrivilege @PrvCode NVARCHAR(32)
	,@ReadHigherPrivileges BIT = 0
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @PrvId INT

	SET @PrvID = (
			SELECT PRV_ID
			FROM TS_PRIVILEGE
			WHERE PRV_CODE = @PrvCode
			)

	SELECT ccn.CCN_USERNAME AS CcnUsername
		,prv.PRV_CODE AS PrvCode
		,prv.PRV_VALUE AS PrvValue
		,ccn.CCN_NOTIFICATION_FLAG as CcnNotificationFlag
	FROM TD_ACCOUNT ccn
	INNER JOIN TS_PRIVILEGE prv
		ON ccn.CCN_PRV_ID = prv.PRV_ID
			AND ccn.CCN_DELETE_FLAG = 0
	WHERE (
			(
				@ReadHigherPrivileges = 1
				AND prv.PRV_ID <= @PrvId
				)
			OR (
				@ReadHigherPrivileges = 0
				AND prv.PRV_ID = @PrvId
				)
			)
END
