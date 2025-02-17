
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 10/07/2019
-- Description:	Soft deletes all matrixes for a given Release Code
-- A separate sp will hard delete the data associated with this matrix
-- =============================================
CREATE
	

 PROCEDURE Data_Matrix_Delete @RlsCode INT
	,@CcnUsername NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @updateCount INT

	DECLARE @MatrixIds TABLE (
		MtrId INT
		,MtrDtgId INT
		,Rownum INT
		)
	DECLARE @DtgId INT

	UPDATE TD_MATRIX
	SET MTR_DELETE_FLAG = 1
	WHERE MTR_RLS_ID IN (
			SELECT [RLS_ID]
			FROM [TD_RELEASE]
			WHERE [RLS_DELETE_FLAG] = 0
				AND [RLS_CODE] = @RlsCode
			)
		AND MTR_DELETE_FLAG = 0

	SET @updateCount = @@ROWCOUNT

	INSERT INTO @MatrixIds (
		MtrId
		,MtrDtgId
		,Rownum
		) (
		SELECT MTR_ID
		,MTR_DTG_ID
		,(
			SELECT ROW_NUMBER() OVER (
					ORDER BY MTR_ID ASC
					)
			) AS RowNumber FROM TD_MATRIX WHERE MTR_RLS_ID IN (
			SELECT [RLS_ID]
			FROM [TD_RELEASE]
			WHERE [RLS_DELETE_FLAG] = 0
				AND [RLS_CODE] = @RlsCode
			)
		AND MTR_DELETE_FLAG = 0
		)

	IF @updateCount > 0
	BEGIN
		DECLARE @userId INT

		SELECT @userId = CCN_ID
		FROM TD_ACCOUNT
		WHERE CCN_USERNAME = @CcnUsername
			AND CCN_DELETE_FLAG = 0

		DECLARE @rnum INT

		SET @rnum = 1

		DECLARE @rcount INT = (
				SELECT count(*)
				FROM @MatrixIds
				)

		WHILE (@rnum <= @rcount)
		BEGIN
			SET @DtgId = (
					SELECT MtrDtgId
					FROM @MatrixIds
					WHERE Rownum = @rnum
					)

			EXEC Security_Auditing_Delete @DtgID
				,@CcnUsername

			SET @rnum = @rnum + 1
		END
	END
END
