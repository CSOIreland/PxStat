SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 28/09/2018
-- Description:	Read Groups including Acount and release counts
-- =============================================
CREATE
	OR

ALTER PROCEDURE Security_Group_Read @GrpCode NVARCHAR(32) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT grp.GRP_CODE AS GrpCode
		,grp.GRP_NAME AS GrpName
		,grp.GRP_CONTACT_NAME AS GrpContactName
		,grp.GRP_CONTACT_PHONE AS GrpContactPhone
		,grp.GRP_CONTACT_EMAIL AS GrpContactEmail
		,GroupUserCount.GrpUserCount
		,GroupReleaseCount.GrpReleaseCount
	FROM TD_GROUP grp
	INNER JOIN (
		SELECT grp.GRP_ID
			,count(gcc.GCC_ID) AS GrpUserCount
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
			,count(rls.RLS_ID) AS GrpReleaseCount
		FROM TD_GROUP grp
		LEFT JOIN TD_RELEASE rls
			ON grp.GRP_ID = rls.RLS_GRP_ID
				AND grp.GRP_DELETE_FLAG = 0
				AND rls.RLS_DELETE_FLAG = 0
		GROUP BY grp.GRP_ID
		) GroupReleaseCount
		ON grp.GRP_ID = GroupReleaseCount.GRP_ID
	WHERE (
			@GrpCode IS NULL
			OR (@GrpCode = grp.GRP_CODE)
			)
		AND grp.GRP_DELETE_FLAG = 0
	ORDER BY GRP_CODE
END
GO


