CREATE TABLE [dbo].[tbl_pass] (
    [pass_id]        INT             IDENTITY (100000, 1) NOT NULL,
    [title]          NVARCHAR (20)   NOT NULL,
    [fullname]       NVARCHAR (100)  NULL,
    [gender]         NCHAR (1)       NULL,
    [dob]            DATE            NULL,
    [age]            INT             NULL,
    [address]        NVARCHAR (200)  NULL,
    [email]          NVARCHAR (100)  NULL,
    [contact_no]     NVARCHAR (100)  NULL,
    [club]           NVARCHAR (20)   NULL,
    [visits_allowed] INT             NOT NULL,
    [visits_left]    INT             NULL,
    [comments]       NVARCHAR (1000) NULL,
    [created_on]     DATETIME        DEFAULT (getdate()) NULL,
    [last_visit]     DATETIME        DEFAULT (getdate()) NULL,
    [created_by]     NVARCHAR (30)   NULL,
    PRIMARY KEY CLUSTERED ([pass_id] ASC)
);

