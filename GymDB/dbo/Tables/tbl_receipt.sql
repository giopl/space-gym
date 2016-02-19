CREATE TABLE [dbo].[tbl_receipt] (
    [receipt_id]            INT           IDENTITY (10000, 1) NOT NULL,
    [transaction_id]        INT           NOT NULL,
    [received_on]           DATETIME      DEFAULT (getdate()) NULL,
    [received_by]           NVARCHAR (30) NULL,
    [payment_method]        NVARCHAR (50) NULL,
    [amount_received]       FLOAT (53)    NULL,
    [transaction_cancelled] CHAR (1)      DEFAULT ('N') NULL,
    [member_id]             INT           NULL
);



