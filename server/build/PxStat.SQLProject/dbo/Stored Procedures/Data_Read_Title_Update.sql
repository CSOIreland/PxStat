
CREATE     PROCEDURE [dbo].[Data_Read_Title_Update]
@Placeholder NVARCHAR (256), @MatrixMap KeyValueVarcharAttribute READONLY
AS
DECLARE @cursor CURSOR;
DECLARE @MtrId INT;
DECLARE @Title AS NVARCHAR (MAX);
DECLARE @Contents AS NVARCHAR (256);
DECLARE @Dimensions AS NVARCHAR (256);
DECLARE @Time AS NVARCHAR (256);
DECLARE @StartValue AS NVARCHAR (256);
DECLARE @FinishValue AS NVARCHAR (256);
CREATE TABLE #TITLES (MtrId INT, MtrTitle NVARCHAR (MAX)) 
BEGIN
	SET @Placeholder = ' ' + @Placeholder + ' ';
    SET @cursor = CURSOR FOR
    select [Key], [Value]
	from @MatrixMap   

    OPEN @cursor 
    FETCH NEXT FROM @cursor 
    INTO @MtrId, @Contents

    WHILE @@FETCH_STATUS = 0
		BEGIN
		SET @Time = (SELECT MDM_VALUE
					 FROM   TD_MATRIX_DIMENSION
					 WHERE  MDM_MTR_ID = @MtrId
							AND MDM_DMR_ID = 2);
        SET @Dimensions = ''
		SELECT @Dimensions = @Dimensions + MDM_VALUE + ', '
		FROM   TD_MATRIX_DIMENSION
		WHERE  MDM_MTR_ID = @MtrId
			   AND MDM_DMR_ID = 3;
		SET @StartValue = (SELECT   TOP (1) DMT_VALUE
						   FROM     TD_DIMENSION_ITEM
						   WHERE    DMT_MDM_ID = (SELECT MDM_ID
												  FROM   TD_MATRIX_DIMENSION
												  WHERE  MDM_MTR_ID = @MtrId
														 AND MDM_DMR_ID = 2)
						   ORDER BY DMT_VALUE DESC);
		SET @FinishValue = (SELECT   TOP (1) DMT_VALUE
							FROM     TD_DIMENSION_ITEM
							WHERE    DMT_MDM_ID = (SELECT MDM_ID
												   FROM   TD_MATRIX_DIMENSION
												   WHERE  MDM_MTR_ID = @MtrId
														  AND MDM_DMR_ID = 2)
							ORDER BY DMT_VALUE ASC);
		SET @Dimensions = SUBSTRING(@Dimensions, 1, len(@Dimensions)-1)
		IF @StartValue = @FinishValue
			BEGIN
				SET @Title = @Contents + @Placeholder + @Dimensions + ' and ' + @Time + ' ' + @StartValue;
			END
		ELSE
			BEGIN
				SET @Title = @Contents + @Placeholder + @Dimensions + ' and ' + @Time + ' ' + @StartValue + @Placeholder + @FinishValue;
			END 
		INSERT INTO #TITLES VALUES(@MtrId, @Title)
		FETCH NEXT FROM @cursor INTO @MtrId, @Contents
    END; 
    CLOSE @cursor;
    DEALLOCATE @cursor;
	SELECT * from #TITLES
END