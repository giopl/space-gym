CREATE TABLE [dbo].[tbl_visit] (
    [visit_id]  INT      IDENTITY (100, 1) NOT NULL,
    [member_id] INT      NOT NULL,
    [check_in]  DATETIME NULL,
    [check_out] DATETIME NULL,
    PRIMARY KEY CLUSTERED ([visit_id] ASC)
);

