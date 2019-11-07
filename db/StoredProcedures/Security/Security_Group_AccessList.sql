SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Paulo Patricio
-- Read date:	07 Dec 2018
-- Description:	Lists groups user has access to
-- =============================================
CREATE
	OR

ALTER PROCEDURE Security_Group_AccessList @CcnUsername NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM TD_ACCOUNT
			INNER JOIN TS_PRIVILEGE
				ON CCN_PRV_ID = PRV_ID
					AND (
						PRV_CODE = 'ADMINISTRATOR'
						OR PRV_CODE = 'POWER_USER'
						)
					AND CCN_DELETE_FLAG = 0
			WHERE CCN_USERNAME = @CcnUsername
			)
	BEGIN
		SELECT GRP_ID
		FROM TD_GROUP
		WHERE GRP_DELETE_FLAG = 0
	END
	ELSE
	BEGIN
		SELECT GCC_GRP_ID
		FROM TM_GROUP_ACCOUNT
		INNER JOIN TD_ACCOUNT
			ON GCC_CCN_ID = CCN_ID
		WHERE CCN_USERNAME = @CcnUsername
			AND GCC_DELETE_FLAG = 0
			AND CCN_DELETE_FLAG = 0
		GROUP BY GCC_GRP_ID
	END
END
GO


