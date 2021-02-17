-- Drop Types
DECLARE @name VARCHAR(500)
DECLARE cur CURSOR 
FOR
SELECT [name]
FROM [sys].[types]
WHERE is_user_defined = 1

OPEN cur

FETCH NEXT
FROM cur
INTO @name

WHILE @@fetch_status = 0
BEGIN
	EXEC ('DROP TYPE [' + @name + ']')

	FETCH NEXT
	FROM cur
	INTO @name
END

CLOSE cur

DEALLOCATE cur
