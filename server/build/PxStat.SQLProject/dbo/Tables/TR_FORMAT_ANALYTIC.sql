﻿CREATE TABLE [dbo].[TR_FORMAT_ANALYTIC] (
    [FNL_NLT_DATE] DATE           NOT NULL,
    [FNL_FORMAT]   NVARCHAR (128) NOT NULL,
    [FNL_TOTAL]    INT            NOT NULL
);


GO
CREATE CLUSTERED INDEX [IX_TR_FORMAT_ANALYTIC]
    ON [dbo].[TR_FORMAT_ANALYTIC]([FNL_NLT_DATE] ASC);

