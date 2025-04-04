﻿CREATE TABLE [dbo].[TD_VARIABLE] (
    [VRB_ID]               INT            IDENTITY (1, 1) NOT NULL,
    [VRB_CODE]             NVARCHAR (256) NOT NULL,
    [VRB_VALUE]            NVARCHAR (256) NOT NULL,
    [VRB_CLS_ID]           INT            NOT NULL,
    [VRB_ELIMINATION_FLAG] BIT            DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_TD_VARIABLE] PRIMARY KEY CLUSTERED ([VRB_ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_VRB_CLS_ID]
    ON [dbo].[TD_VARIABLE]([VRB_CLS_ID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_VRB_CODE]
    ON [dbo].[TD_VARIABLE]([VRB_CODE] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_VRB_ELIMINATION_FLAG]
    ON [dbo].[TD_VARIABLE]([VRB_ELIMINATION_FLAG] ASC);

