/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SBR_UID' AND object_id = OBJECT_ID('TD_SUBSCRIBER'))
BEGIN
	DROP INDEX IX_SBR_UID ON TD_SUBSCRIBER;
	CREATE UNIQUE NONCLUSTERED INDEX [IX_SBR_UID]
	ON [dbo].[TD_SUBSCRIBER]([SBR_UID]) WHERE (SBR_DELETE_FLAG = 0);
END