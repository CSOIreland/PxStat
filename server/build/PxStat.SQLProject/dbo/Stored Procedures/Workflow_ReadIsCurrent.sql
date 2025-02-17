
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 01/11/2018
-- Description:	Checks if there is a current workflow for a ReleaseCode
-- =============================================
CREATE
	

 PROCEDURE Workflow_ReadIsCurrent @RlsCode INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @result INT

	SET @result = (
			iif(EXISTS (
					SELECT rls.RLS_ID
					FROM TD_RELEASE rls
					INNER JOIN TD_WORKFLOW_REQUEST wrq
						ON wrq.WRQ_RLS_ID = rls.RLS_ID
					WHERE rls.RLS_CODE = @RlsCode
						AND wrq.WRQ_CURRENT_FLAG = 1
						AND rls.RLS_DELETE_FLAG = 0
						AND wrq.WRQ_DELETE_FLAG = 0
					), 1, 0)
			)

	RETURN @result
END
