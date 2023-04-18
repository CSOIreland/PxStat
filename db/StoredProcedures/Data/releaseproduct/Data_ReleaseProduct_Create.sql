GO
/****** Object:  StoredProcedure [dbo].[Data_ReleaseProduct_Create]    Script Date: 04/08/2022 10:30:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER   PROCEDURE [dbo].[Data_ReleaseProduct_Create]
@ReleaseCode NVARCHAR (256), @ProductCode NVARCHAR (256), @userName NVARCHAR (256)
AS
BEGIN
    SET NOCOUNT ON;
	DECLARE @ReleaseId int
	DECLARE @ProductId int
	DECLARE @ReleaseProductId int
	DECLARE @Count int
	DECLARE @auditId AS INT = NULL;

	SELECT @ReleaseId = RLS_ID, @ReleaseProductId = RLS_PRC_ID from TD_RELEASE
	WHERE RLS_CODE = @ReleaseCode
	AND RLS_DELETE_FLAG = 0

	SELECT @ProductId = PRC_ID
    FROM   TD_PRODUCT
    WHERE  PRC_CODE = @ProductCode
	AND PRC_DELETE_FLAG = 0

	--If you are trying to associate the Product with the core Product, this is a duplicate, return -1
	IF @ProductId = @ReleaseProductId
	BEGIN
		RETURN -1
	END
	SELECT @Count = COUNT(*) FROM TM_RELEASE_PRODUCT
	WHERE RPR_RLS_ID = @ReleaseId
	AND RPR_PRC_ID = @ProductId
	AND RPR_DELETE_FLAG = 0

	-- If the Product is associated with the Release already i.e. a duplicate, return -1
	IF @Count > 0
	BEGIN
		RETURN -1
	END

	EXECUTE @auditId = Security_Auditing_Create @userName;
    IF @auditId IS NULL
       OR @auditId = 0
    BEGIN
        RAISERROR ('SP: Security_Auditing_Create has failed!', 16, 1);
        RETURN 0;
    END
    INSERT INTO TM_RELEASE_PRODUCT (RPR_RLS_ID, RPR_PRC_ID, RPR_DTG_ID, RPR_DELETE_FLAG)
    VALUES(@ReleaseID, @ProductID, @auditId, 0);
    RETURN @@IDENTITY;
END