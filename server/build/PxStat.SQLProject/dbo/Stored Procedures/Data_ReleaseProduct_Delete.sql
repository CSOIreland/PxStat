
CREATE
	

 PROCEDURE [dbo].[Data_ReleaseProduct_Delete] @ReleaseCode INT
	,@ProductCode NVARCHAR(256)
	,@userName NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @auditId AS INT = NULL;
	SELECT @auditId = RPR_DTG_ID FROM TM_RELEASE_PRODUCT
	WHERE RPR_RLS_ID = (
			SELECT RLS_ID
			FROM TD_RELEASE
			WHERE RLS_CODE = @ReleaseCode
			AND RLS_DELETE_FLAG=0
			)
		AND RPR_PRC_ID = (
			SELECT PRC_ID
			FROM TD_PRODUCT
			WHERE PRC_CODE = @ProductCode
			AND PRC_DELETE_FLAG =0 
			)
		AND RPR_DELETE_FLAG = 0

	EXECUTE @auditId = Security_Auditing_Delete @auditId, @userName;

	IF @auditId IS NULL
		OR @auditId = 0
	BEGIN
		RAISERROR (
				'SP: Security_Auditing_Delete has failed!'
				,16
				,1
				);

		RETURN 0;
	END

	UPDATE TM_RELEASE_PRODUCT
	SET RPR_DELETE_FLAG = 1
		,RPR_DTG_ID = @auditId
	WHERE RPR_RLS_ID = (
			SELECT RLS_ID
			FROM TD_RELEASE
			WHERE RLS_CODE = @ReleaseCode
			AND RLS_DELETE_FLAG=0
			)
		AND RPR_PRC_ID = (
			SELECT PRC_ID
			FROM TD_PRODUCT
			WHERE PRC_CODE = @ProductCode
			AND PRC_DELETE_FLAG =0 
			)
		AND RPR_DELETE_FLAG = 0

	RETURN @@ROWCOUNT;
END
