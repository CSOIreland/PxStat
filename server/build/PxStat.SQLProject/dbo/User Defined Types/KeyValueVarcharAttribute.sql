CREATE TYPE [dbo].[KeyValueVarcharAttribute] AS TABLE (
    [Key]       NVARCHAR (256) NULL,
    [Value]     NVARCHAR (256) NULL,
    [Attribute] INT            DEFAULT ((1)) NULL);

