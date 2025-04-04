﻿CREATE TABLE [dbo].[TD_KEYWORD_RELEASE] (
    [KRL_ID]                INT            IDENTITY (1, 1) NOT NULL,
    [KRL_CODE]              INT            CONSTRAINT [KRL_CODE_SEQUENCE] DEFAULT (NEXT VALUE FOR [dbo].[KRL_CODE_seq]) NOT NULL,
    [KRL_VALUE]             NVARCHAR (256) NOT NULL,
    [KRL_RLS_ID]            INT            NOT NULL,
    [KRL_MANDATORY_FLAG]    BIT            CONSTRAINT [DF_TD_KEYWORD_RELEASE_KRL_MANDATORY_FLAG] DEFAULT ((0)) NOT NULL,
    [KRL_SINGULARISED_FLAG] BIT            CONSTRAINT [DF_TD_KEYWORD_RELEASE_KRL_SINGULARISED_FLAG] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_TD_KEYWORD_RELEASE] PRIMARY KEY CLUSTERED ([KRL_ID] ASC),
    CONSTRAINT [FK_TD_KEYWORD_RELEASE_TD_RELEASE] FOREIGN KEY ([KRL_RLS_ID]) REFERENCES [dbo].[TD_RELEASE] ([RLS_ID])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_KRL_CODE]
    ON [dbo].[TD_KEYWORD_RELEASE]([KRL_CODE] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_KRL_MANDATORY_FLAG]
    ON [dbo].[TD_KEYWORD_RELEASE]([KRL_MANDATORY_FLAG] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_KRL_RLS_ID]
    ON [dbo].[TD_KEYWORD_RELEASE]([KRL_RLS_ID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_KRL_SINGULARISED_FLAG]
    ON [dbo].[TD_KEYWORD_RELEASE]([KRL_SINGULARISED_FLAG] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_KRL_VALUE]
    ON [dbo].[TD_KEYWORD_RELEASE]([KRL_VALUE] ASC);

