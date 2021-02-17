CREATE TYPE [dbo].[KeyValueVarcharAttribute] AS TABLE (
	[Key] [nvarchar](256) NULL,
	[Value] [nvarchar](256) NULL,
	[Attribute] [int] NULL DEFAULT ((1))
	)
GO


