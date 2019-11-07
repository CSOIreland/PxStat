SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 05/11/2018
-- Description:	Read Groups for which the CcnUsername has access
-- exec Security_Group_ReadAccess 'okeeffene'
-- =============================================
CREATE
	OR

ALTER PROCEDURE Security_Group_ReadAccess @CcnUsername NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT grp.GRP_NAME AS GrpName
		,grp.GRP_CONTACT_NAME AS GrpContactName
		,grp.GRP_CONTACT_PHONE AS GrpContactPhone
		,grp.GRP_CONTACT_EMAIL AS GrpContactEmail
		,grp.GRP_CODE AS GrpCode
	FROM TD_GROUP grp
	INNER JOIN TM_GROUP_ACCOUNT gcc
		ON grp.GRP_ID = gcc.GCC_GRP_ID
			AND grp.GRP_DELETE_FLAG = 0
			AND gcc.GCC_DELETE_FLAG = 0
	INNER JOIN TD_ACCOUNT ccn
		ON gcc.GCC_CCN_ID = ccn.CCN_ID
			AND ccn.CCN_DELETE_FLAG = 0
	WHERE ccn.CCN_USERNAME = @CcnUsername
END
GO


