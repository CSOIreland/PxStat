-- Output script to reorganise all indexes
SELECT 'ALTER INDEX ALL ON ' + TABLE_SCHEMA + '.' + TABLE_NAME + '  REORGANIZE;'
FROM Information_Schema.tables
WHERE TABLE_TYPE = 'BASE TABLE';
