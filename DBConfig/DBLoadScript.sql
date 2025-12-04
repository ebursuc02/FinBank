---------------------------------------
-- Create databases (if not created) --
---------------------------------------
IF DB_ID('FinBank') IS NULL
BEGIN
  CREATE DATABASE FinBank;
END;
GO

IF DB_ID('KYC') IS NULL
BEGIN
  CREATE DATABASE KYC;
END;
GO


---------------------------------------
------- FinBank schema & tables -------
---------------------------------------
USE FinBank;
GO
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO


-- Users --
IF OBJECT_ID('dbo.Users','U') IS NULL
CREATE TABLE dbo.Users
(
    UserId        UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_User PRIMARY KEY,
    Email         NVARCHAR(100)    NOT NULL CONSTRAINT UN_User_Email UNIQUE,
    Password      NVARCHAR(128)    NOT NULL,
    Role          NVARCHAR(50)     NOT NULL,
    CreatedAt     DATETIME2(3)     NOT NULL CONSTRAINT DF_User_CreatedAt DEFAULT SYSUTCDATETIME(),
    Name          NVARCHAR(200)    NOT NULL,
    PhoneNumber   NVARCHAR(50)     NOT NULL,
    Country       NVARCHAR(100)    NULL,
    Birthday      DATE             NULL,
    Address       NVARCHAR(300)    NULL 
);
GO


-- Accounts --
IF OBJECT_ID('dbo.Accounts','U') IS NULL
BEGIN
    CREATE TABLE dbo.Accounts
    (
        IBAN       VARCHAR(34)      NOT NULL CONSTRAINT PK_Accounts PRIMARY KEY,
        CustomerId UNIQUEIDENTIFIER NOT NULL,
        IsClosed   BIT              NOT NULL DEFAULT 0,
        CreatedAt  DATETIME2(3)     NOT NULL CONSTRAINT DF_Accounts_CreatedAt DEFAULT SYSUTCDATETIME(),
        Balance    DECIMAL(18,2)    NOT NULL CONSTRAINT DF_Accounts_Balance DEFAULT 0,
        Currency   VARCHAR(3)       NOT NULL,

        CONSTRAINT CK_Currency
            CHECK (Currency IN ('RON','USD','EUR')),
        CONSTRAINT FK_Accounts_Customers
            FOREIGN KEY (CustomerId) REFERENCES dbo.Users(UserId)
            ON DELETE NO ACTION ON UPDATE NO ACTION
    );
    CREATE INDEX IX_Accounts_CustomerId ON dbo.Accounts(CustomerId);
END;
GO

-- Transfers --
IF OBJECT_ID('dbo.Transfers','U') IS NULL
BEGIN
    CREATE TABLE dbo.Transfers
    (
        TransferId     UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Transfers PRIMARY KEY,
        FromIban       VARCHAR(34)      NOT NULL,
        ToIban         VARCHAR(34)      NOT NULL,
        ReviewedBy     UNIQUEIDENTIFIER NULL,
        CreatedAt      DATETIME2(3)     NOT NULL CONSTRAINT DF_Transfers_CreatedAt DEFAULT SYSUTCDATETIME(),
        Status         VARCHAR(30)      NOT NULL,   -- Pending, Completed, Rejected, UnderReview
        Amount         DECIMAL(18,2)    NOT NULL,
        Currency       CHAR(3)          NOT NULL,
        Reason         NVARCHAR(500)    NULL,
        CompletedAt    DATETIME2(3)     NULL,
        PolicyVersion  VARCHAR(20)      NULL,

        CONSTRAINT CK_Transfers_Status
            CHECK (Status IN ('Pending','Completed','Rejected','UnderReview', 'Failed')),
        CONSTRAINT CK_Transfers_FromToDifferent
            CHECK (FromIban <> ToIban),
        CONSTRAINT CK_Currency_Transfers
            CHECK (Currency IN ('RON','USD','EUR')),

        CONSTRAINT FK_Transfers_FromAccount
            FOREIGN KEY (FromIban) REFERENCES dbo.Accounts(IBAN)
            ON DELETE NO ACTION ON UPDATE NO ACTION,
        CONSTRAINT FK_Transfers_ReviewedBy
            FOREIGN KEY (ReviewedBy)    REFERENCES dbo.Users(UserId)
            ON DELETE NO ACTION ON UPDATE NO ACTION
    );
    CREATE INDEX IX_Transfers_FromIban ON dbo.Transfers(FromIban);
    CREATE INDEX IX_Transfers_ToIban   ON dbo.Transfers(ToIban);
    CREATE INDEX IX_Transfers_StatusCreated ON dbo.Transfers(Status, CreatedAt);
END;
GO


-- IdempotencyKeys --
IF OBJECT_ID('dbo.IdempotencyKeys','U') IS NULL
BEGIN
    CREATE TABLE dbo.IdempotencyKeys
    (
        [Key]             NVARCHAR(100)    NOT NULL CONSTRAINT PK_IdempotencyKeys PRIMARY KEY,
        RequestHash       NVARCHAR(200)    NULL,
        ResponseJson      NVARCHAR(MAX)    NULL,
        FirstProcessedAt  DATETIME2(3)     NULL,
    );
    CREATE UNIQUE INDEX UX_Idem_RequestHash ON dbo.IdempotencyKeys(RequestHash) WHERE RequestHash IS NOT NULL;
END;
GO


---------------------------------------
------- KYC database and schema -------
---------------------------------------
USE KYC;
GO
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- CustomerRisk --
IF OBJECT_ID('dbo.CustomerRisk','U') IS NULL
CREATE TABLE dbo.CustomerRisk
(
    CustomerId UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_CustomerRisk PRIMARY KEY,
    RiskStatus VARCHAR(20)      NOT NULL,  -- Low, Medium, High, Blocked, Unknown
    UpdatedAt  DATETIME2(3)     NOT NULL CONSTRAINT DF_CustomerRisk_UpdatedAt DEFAULT SYSUTCDATETIME(),

    CONSTRAINT CK_CustomerRisk_Status CHECK (RiskStatus IN ('Low','Medium','High','Blocked','Unknown'))
);
GO


PRINT 'Databases FinBank and KYC created with required tables and constraints.';
GO
