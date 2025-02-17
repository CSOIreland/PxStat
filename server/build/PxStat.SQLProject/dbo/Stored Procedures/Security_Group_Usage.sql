
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 01/10/2018
-- Description:	To check whether or not the Group has associated entries in the TM_GROUP_ACCOUNT or TD_RELEASE tables
-- exec Security_Group_Usage '43535'
-- =============================================
CREATE
	

 PROCEDURE Security_Group_Usage @GrpCode NVARCHAR(32)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @GrpId INT
	DECLARE @IsInUse INT

	SET @GrpId = (
			SELECT GRP_ID
			FROM TD_GROUP
			WHERE GRP_CODE = @GrpCode
				AND GRP_DELETE_FLAG = 0
			)

	IF @GrpId IS NULL
		OR @GrpId = 0
	BEGIN
		-- No error to be returned, just the fact that there are no related records
		RETURN 0
	END

	SET @IsInUse = (
			SELECT (
					iif(EXISTS (
							SELECT NULL
							FROM TM_GROUP_ACCOUNT
							WHERE GCC_GRP_ID = @GrpID
								AND GCC_DELETE_FLAG = 0
							)
						OR EXISTS (
							SELECT NULL
							FROM TD_RELEASE
							WHERE RLS_GRP_ID = @GrpID
								AND RLS_DELETE_FLAG = 0
							), 1, 0)
					)
			)

	RETURN @IsInUse
END
