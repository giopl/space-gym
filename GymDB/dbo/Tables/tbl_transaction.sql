CREATE TABLE [dbo].[tbl_transaction] (
    [transaction_id]            INT             IDENTITY (100, 1) NOT NULL,
    [member_id]                 INT             NOT NULL,
    [membership_code]           NVARCHAR (20)   NULL,
    [transaction_date]          DATETIME        DEFAULT (getdate()) NOT NULL,
    [period_start_date]         DATE            NOT NULL,
    [period_end_date]           DATE            NOT NULL,
    [amount_paid]               FLOAT (53)      NULL,
    [amount_discounted]         FLOAT (53)      NULL,
    [amount_registration]       FLOAT (53)      NULL,
    [amount_unpaid]             FLOAT (53)      NULL,
    [last_transaction_id]       INT             NULL,
    [num_facilities_orig]       INT             NULL,
    [num_facilities_left]       INT             NULL,
    [comment]                   NVARCHAR (1000) NULL,
    [amount_writtenoff]         FLOAT (53)      NULL,
    [amount_first_installment]  FLOAT (53)      NULL,
    [amount_second_installment] FLOAT (53)      NULL
);











