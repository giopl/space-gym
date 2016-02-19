CREATE TABLE [dbo].[tbl_comment] (
    [comment_id]     INT             IDENTITY (100, 1) NOT NULL,
    [member_id]      INT             NOT NULL,
    [comment]        NVARCHAR (2000) NULL,
    [comment_date]   DATETIME        NULL,
    [followup_date]  DATE            NULL,
    [comment_type]   NVARCHAR (50)   NULL,
    [Inputter]       NVARCHAR (30)   NULL,
    [comment_status] NVARCHAR (20)   NULL,
    PRIMARY KEY CLUSTERED ([comment_id] ASC)
);

