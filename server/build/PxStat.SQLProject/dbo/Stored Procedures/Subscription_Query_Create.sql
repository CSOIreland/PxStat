
-- =============================================
-- Author:		Damian Chapman
-- Create date: 27/09/2021
-- Description:	Create a Query
-- =============================================
CREATE    
	

 PROCEDURE [dbo].[Subscription_Query_Create] @TagName NVARCHAR(256)
	,@Matrix NVARCHAR(256)
	,@SnippetType NVARCHAR(256)
	,@SnippetQuery NVARCHAR(MAX)
	,@FluidTime BIT = 0
	,@SnippetIsogram NVARCHAR(256)
	,@SubscriberUserId NVARCHAR(256) = NULL
	,@CcnUsername NVARCHAR(256) = NULL
	,@QueryThreshold INT
AS
BEGIN
	SET NOCOUNT ON;

	IF @SubscriberUserId IS NULL
		AND @CcnUsername IS NULL
	BEGIN
		RETURN 0
	END

	DECLARE @UserId INT

	IF @SubscriberUserId IS NOT NULL
	BEGIN
		SET @UserId = (
				SELECT USR_ID
				FROM TD_USER
				INNER JOIN TD_SUBSCRIBER ON SBR_USR_ID = USR_ID
					AND SBR_UID = @SubscriberUserId
					AND SBR_DELETE_FLAG = 0
				)
	END
	ELSE
	BEGIN
		SET @UserId = (
				SELECT USR_ID
				FROM TD_USER
				INNER JOIN TD_ACCOUNT ON CCN_USR_ID = USR_ID
					AND CCN_USERNAME = @CcnUsername
					AND CCN_DELETE_FLAG = 0
				)
	END

	IF @UserId IS NULL
	BEGIN
		RETURN 0;
	END

	-- Check if user has reached the query threshold
	DECLARE @CurrentThreshold INT

	SET @CurrentThreshold = (
			SELECT count(*)
			FROM TD_USER_QUERY
			WHERE SQR_USR_ID = @UserId
			)

	IF @CurrentThreshold >= @QueryThreshold
	BEGIN
		RETURN - 2
	END

	IF EXISTS (
			SELECT 1
			FROM TD_USER_QUERY
			WHERE SQR_USR_ID = @UserId
				AND SQR_SNIPPET_TAGNAME = @TagName
			)
	BEGIN
		RETURN - 1
	END

	DECLARE @SnippetQueryCompressed VARBINARY(MAX)

	SET @SnippetQueryCompressed = compress(@SnippetQuery)

	INSERT INTO TD_USER_QUERY (
		SQR_SNIPPET_TAGNAME
		,SQR_SNIPPET_MATRIX
		,SQR_USR_ID
		,SQR_SNIPPET_TYPE
		,SQR_SNIPPET_QUERY
		,SQR_FLUID_TIME
		,SQR_SNIPPET_ISOGRAM
		)
	VALUES (
		@TagName
		,@Matrix
		,@UserId
		,@SnippetType
		,@SnippetQueryCompressed
		,@FluidTime
		,@SnippetIsogram
		)

	RETURN @@IDENTITY
END