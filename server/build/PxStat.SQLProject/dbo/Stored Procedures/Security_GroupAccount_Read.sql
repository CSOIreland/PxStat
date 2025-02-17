
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 04/10/2018
-- Description:	To read a Group Account entry. The output will be restricted depending on what parameters are or are not passed in
--exec Security_GroupAccount_Read 'dissemination@cso.ie'
-- =============================================
CREATE
	

 PROCEDURE Security_GroupAccount_Read @CcnUsername NVARCHAR(256) = NULL
	,@GrpCode NVARCHAR(32) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @CcnID INT = NULL
	DECLARE @GrpID INT = NULL

	IF @CcnUsername IS NOT NULL
	BEGIN
		SET @CcnID = (
				SELECT CCN_ID
				FROM TD_ACCOUNT
				WHERE CCN_USERNAME = @CcnUsername
					AND TD_ACCOUNT.CCN_DELETE_FLAG = 0
				)

		IF @CcnID IS NULL
		BEGIN
			-- we have sent in a @CcnUsername parameter but there is no corresponding entry in the TD_ACCOUNT table, so we return nothing
			RETURN 0
		END
	END

	IF @GrpCode IS NOT NULL
	BEGIN
		SET @GrpID = (
				SELECT GRP_ID
				FROM TD_GROUP
				WHERE GRP_CODE = @GrpCode
					AND TD_GROUP.GRP_DELETE_FLAG = 0
				)

		IF @GrpID IS NULL
		BEGIN
			-- we have sent in a @GrpCode parameter but there is no corresponding entry int the TD_GRP table, so we return nothing
			RETURN 0
		END
	END

	SELECT acc_inner.CCN_USERNAME AS CcnUsername
		,grp_inner.GRP_CODE AS GrpCode
		,grp_inner.GRP_NAME AS GrpName
		,gra_inner.GCC_APPROVE_FLAG AS GccApproveFlag
		,acc_inner.CCN_DISPLAYNAME AS CcnDisplayName
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
		) grp_inner
		ON gra_inner.GCC_GRP_ID = grp_inner.GRP_ID
	WHERE (
			@GrpID IS NULL
			OR gra_inner.GCC_GRP_ID = @GrpID
			)
		AND (
			@CcnID IS NULL
			OR gra_inner.GCC_CCN_ID = @CcnID
			)
END
