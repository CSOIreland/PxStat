
-- =============================================
-- Author:		Neil O'Keeffe
-- Create date: 17/01/2019
-- Description:	Get a list of users who are approvers for a given Release
-- exec Security_Account_ReadReleaseApprovers NULL,6,1,'MODERATOR'
-- =============================================
CREATE
	

 PROCEDURE Security_Account_ReadReleaseApprovers @CcnUsername NVARCHAR(256) = NULL
	,@RlsCode INT
	,@GccApproveFlag bit=null
	,@PrvCode varchar(32)=null
AS
BEGIN
	SET NOCOUNT ON;



	DECLARE @PrvId int

	if @PrvCode is not null
	begin
		set @PrvId=(SELECT PRV_ID FROM TS_PRIVILEGE WHERE PRV_CODE=@PrvCode )
	end


	SELECT RLS_CODE as RlsCode
		,CCN_USERNAME as CcnUsername
		,GRP_NAME as GrpName
		,CCN_NOTIFICATION_FLAG as CcnNotificationFlag
	FROM TD_ACCOUNT
	INNER JOIN TM_GROUP_ACCOUNT
		ON CCN_ID = GCC_CCN_ID
			AND CCN_DELETE_FLAG = 0
			AND GCC_DELETE_FLAG = 0
	INNER JOIN TD_GROUP
		ON GCC_GRP_ID = GRP_ID
			AND GRP_DELETE_FLAG = 0
	INNER JOIN TD_RELEASE
		ON RLS_GRP_ID = GRP_ID
			AND RLS_DELETE_FLAG = 0
	
	inner join TS_PRIVILEGE 
		on CCN_PRV_ID=PRV_ID 
		and (@PrvCode is null or(PRV_ID<=@PrvId))
		WHERE  RLS_CODE = @RlsCode
		AND (
			@CcnUsername IS NULL
			OR @CcnUsername = CCN_USERNAME
			)
		AND (@GccApproveFlag is null and 
		GCC_APPROVE_FLAG = 1 or (@GccApproveFlag =GCC_APPROVE_FLAG))
END
