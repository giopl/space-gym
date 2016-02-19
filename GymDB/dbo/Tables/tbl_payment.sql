CREATE TABLE [dbo].[tbl_payment] (
    [transaction_id] INT        NOT NULL,
    [year_month]     INT        NOT NULL,
    [fees]           FLOAT (53) NULL,
    [paid]           FLOAT (53) NULL,
    [written_off]    FLOAT (53) NULL,
    [discounted]     FLOAT (53) NULL,
    [due]            FLOAT (53) NULL,
    PRIMARY KEY CLUSTERED ([transaction_id] ASC, [year_month] ASC)
);

