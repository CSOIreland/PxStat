
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 04/10/2018
-- Description:	To read a Group Account entry. A list of GroupCodes is passed in as a parameter.
-- If Group codes is empty then we read from all Groups
-- exec Security_GroupAccount_ReadMultiple
-- =============================================
CREATE
	

 PROCEDURE Security_GroupAccount_ReadMultiple @GroupList ValueVarchar READONLY
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @ReadAllGroups BIT

	IF (
			SELECT count(*)
			FROM @GroupList
			) = 0
	BEGIN
		SET @ReadAllGroups = 1
	END
	ELSE
	BEGIN
		SET @ReadAllGroups = 0
	END

	SELECT DISTINCT acc_inner.CCN_USERNAME AS CcnUsername
		,CCN_NOTIFICATION_FLAG AS CcnNotificationFlag
	FROM (
		SELECT *
		FROM TD_ACCOUNT
		WHERE CCN_DELETE_FLAG = 0
		) acc_inner
	INNER JOIN (
		SELECT *
		FROM TM_GROUP_ACCOUNT
		WHERE GCC_DELETE_FLAG = 0
		) gra_inner
		ON acc_inner.CCN_ID = gra_inner.GCC_CCN_ID
	INNER JOIN (
		SELECT *
		FROM td_group
		WHERE GRP_DELETE_FLAG = 0
			AND GRP_CODE IN (
				SELECT [value]
				FROM @GroupList
				)
			OR @ReadAllGroups = 1
		) grp_inner
		ON gra_inner.GCC_GRP_ID = grp_inner.GRP_ID
END
