SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 21/06/2019
-- Description:	Gets data to indicate if a user has approval access for a release via group membership
-- exec Workflow_WorkflowResponse_ReadApprovalAccess 'okeeffene',6,1
-- =============================================
CREATE
	OR

ALTER PROCEDURE Workflow_WorkflowResponse_ReadApprovalAccess @CcnUsername NVARCHAR(256)
	,@RlsCode INT = NULL
	,@GccApproveFlag BIT = NULL
AS
BEGIN
	SELECT GRP_NAME AS GrpName
		,GRP_CODE AS GrpCode
		,CCN_USERNAME AS CcnUsername
		,RLS_CODE AS RlsCode
		,GCC_APPROVE_FLAG AS GccApproveFlag
	FROM TD_RELEASE
	INNER JOIN TM_GROUP_ACCOUNT
		ON GCC_GRP_ID = RLS_GRP_ID
			AND GCC_DELETE_FLAG = 0
			AND RLS_DELETE_FLAG = 0
	INNER JOIN TD_GROUP
		ON GCC_GRP_ID = GRP_ID
			AND GRP_DELETE_FLAG = 0
	INNER JOIN TD_ACCOUNT
		ON GCC_CCN_ID = CCN_ID
			AND CCN_DELETE_FLAG = 0
	WHERE CCN_USERNAME = @CcnUsername
		AND (
			@RlsCode IS NULL
			OR RLS_CODE = @RlsCode
			)
		AND (
			@GccApproveFlag IS NULL
			OR GCC_APPROVE_FLAG = @GccApproveFlag
			)
END
GO


