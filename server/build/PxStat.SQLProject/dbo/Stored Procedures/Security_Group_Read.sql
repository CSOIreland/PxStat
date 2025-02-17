
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 28/09/2018
-- Description:	Read Groups including Acount and release counts
-- exec Security_Group_Read 'RPTEST'
-- =============================================
CREATE
	

 PROCEDURE Security_Group_Read @GrpCode NVARCHAR(32) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT grp.GRP_CODE AS GrpCode
		,grp.GRP_NAME AS GrpName
		,grp.GRP_CONTACT_NAME AS GrpContactName
		,grp.GRP_CONTACT_PHONE AS GrpContactPhone
		,grp.GRP_CONTACT_EMAIL AS GrpContactEmail
		,GroupUserCount.CcnCount
		,GroupReleaseCount.RlsLiveCount
		,coalesce(mtrCount,0) AS MtrCount
	FROM TD_GROUP grp
	INNER JOIN (
		SELECT grp.GRP_ID
			,count(gcc.GCC_ID) AS CcnCount
		FROM TD_GROUP grp
		LEFT JOIN TM_GROUP_ACCOUNT gcc
			ON grp.GRP_ID = gcc.GCC_GRP_ID
				AND grp.GRP_DELETE_FLAG = 0
				AND gcc.GCC_DELETE_FLAG = 0
		GROUP BY grp.GRP_ID
		) GroupUserCount
		ON grp.GRP_ID = GroupUserCount.GRP_ID
	INNER JOIN (
		SELECT grp.GRP_ID
			,count(rls.RLS_ID) AS RlsLiveCount
		FROM TD_GROUP grp
		LEFT JOIN TD_RELEASE rls
			ON grp.GRP_ID = rls.RLS_GRP_ID
				AND grp.GRP_DELETE_FLAG = 0
				AND rls.RLS_DELETE_FLAG = 0
				AND RLS_id IN (
					SELECT VRN_RLS_ID
					FROM VW_RELEASE_LIVE_NOW
					)
		GROUP BY grp.GRP_ID
		) GroupReleaseCount
		ON grp.GRP_ID = GroupReleaseCount.GRP_ID

		LEFT JOIN (
			SELECT GRP_CODE
				,COUNT(*) AS mtrCount
			FROM (
				SELECT DISTINCT GRP_CODE, MTR_CODE
				FROM TD_GROUP
				INNER JOIN TD_RELEASE
				ON GRP_ID=RLS_GRP_ID 
				AND GRP_DELETE_FLAG=0
				AND RLS_DELETE_FLAG=0
				INNER JOIN TD_MATRIX
				ON RLS_ID=MTR_RLS_ID
				AND RLS_DELETE_FLAG=0 
				AND MTR_DELETE_FLAG=0
				) prcInner
			GROUP BY GRP_CODE
			) 
			 grpMtrCounter
			ON grp.GRP_CODE = grpMtrCounter.GRP_CODE


	WHERE (
			@GrpCode IS NULL
			OR (@GrpCode = grp.GRP_CODE)
			)
		AND grp.GRP_DELETE_FLAG = 0
	ORDER BY grp.GRP_CODE
END
