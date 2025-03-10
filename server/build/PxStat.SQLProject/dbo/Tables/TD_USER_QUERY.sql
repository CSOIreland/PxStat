﻿CREATE TABLE [dbo].[TD_USER_QUERY] (
    [SQR_ID]              INT             IDENTITY (1, 1) NOT NULL,
    [SQR_SNIPPET_TYPE]    NVARCHAR (256)  NOT NULL,
    [SQR_SNIPPET_TAGNAME] NVARCHAR (256)  NOT NULL,
    [SQR_SNIPPET_MATRIX]  NVARCHAR (256)  NOT NULL,
    [SQR_SNIPPET_QUERY]   VARBINARY (MAX) NOT NULL,
    [SQR_SNIPPET_ISOGRAM] NVARCHAR (256)  NOT NULL,
    [SQR_FLUID_TIME]      BIT             NOT NULL,
    [SQR_USR_ID]          INT             NOT NULL,
    CONSTRAINT [PK_TD_USER_QUERY] PRIMARY KEY CLUSTERED ([SQR_ID] ASC),
    CONSTRAINT [TD_USER_QUERY_TD_USER] FOREIGN KEY ([SQR_USR_ID]) REFERENCES [dbo].[TD_USER] ([USR_ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_SQR_USR_ID]
    ON [dbo].[TD_USER_QUERY]([SQR_USR_ID] ASC);

