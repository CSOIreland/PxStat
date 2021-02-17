SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 14/10/2020
-- Description:	Create Login
-- exec Security_Login_Create 'mdiggums@gov.ie','okeeffene','12345','2020-10-23'
-- =============================================
CREATE
	OR

ALTER PROCEDURE Security_Login_Create @CcnUsername NVARCHAR(256)
	,@CcnUsernameCreator NVARCHAR(256)
	,@LgnToken1FA VARCHAR(64) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @CcnId INT
	DECLARE @errorMessage VARCHAR(256)
	DECLARE @DtgID INT

	-- Do the create Audit and get the new DtgID from the stored procedure
	EXEC @DtgID = Security_Auditing_Create @CcnUsernameCreator

	-- Check for problems with the audit stored procedure
	IF @DtgID = 0
		OR @DtgID IS NULL
	BEGIN
		RAISERROR (
				'Error in calling Security_Auditing_Create'
				,16
				,1
				)

		RETURN 0
	END

	SET @CcnId = (
			SELECT CCN_ID
			FROM TD_ACCOUNT
			WHERE CCN_USERNAME = @CcnUsername
				AND CCN_DELETE_FLAG = 0
			)

	IF @CcnId = 0
		OR @CcnId IS NULL
	BEGIN
		RAISERROR (
				'User not found'
				,16
				,1
				)

		RETURN 0
	END

	IF (
			SELECT COUNT(*)
			FROM TD_LOGIN
			WHERE LGN_CCN_ID = @CcnId
			) > 0
	BEGIN
		DELETE
		FROM TD_LOGIN
		WHERE LGN_CCN_ID = @CcnId
	END

	INSERT INTO TD_LOGIN (
		LGN_TOKEN_1FA
		,LGN_CCN_ID
		)
	VALUES (
		@LgnToken1FA
		,@CcnId
		)

	RETURN @@IDENTITY
END
GO


