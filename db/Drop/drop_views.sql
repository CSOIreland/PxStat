-- Drop Views
DECLARE @name VARCHAR(500)
DECLARE cur CURSOR
FOR
SELECT [name]
FROM [sys].[objects]
WHERE type = 'v'

OPEN cur

FETCH NEXT
FROM cur
INTO @name

WHILE @@fetch_status = 0
BEGIN
	EXEC ('DROP VIEW [' + @name + ']')

	FETCH NEXT
	FROM cur
	INTO @name
END

CLOSE cur

DEALLOCATE cur