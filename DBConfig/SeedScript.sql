SET NOCOUNT ON;

--------------------------
-- Reset & Seed FinBank --
--------------------------
USE FinBank;
GO

-- Reset data (maintaining FK order)
DELETE FROM dbo.IdempotencyKeys;
DELETE FROM dbo.Transfers;
DELETE FROM dbo.Accounts;
DELETE FROM dbo.Users;
GO

-- Users
DECLARE @custAlice UNIQUEIDENTIFIER = NEWID();
DECLARE @custBob   UNIQUEIDENTIFIER = NEWID();
DECLARE @custCarol UNIQUEIDENTIFIER = NEWID();
DECLARE @empReviewer UNIQUEIDENTIFIER = NEWID();

INSERT INTO dbo.Users (UserId, Email, Password, Role, Cnp, CreatedAt, Name, PhoneNumber, Country, Birthday, Address)
VALUES
(@custAlice, N'alice@finbank.test',  N'hash_pwd_alice',  N'Customer', '1760226055524', SYSUTCDATETIME(), N'Alice Martin',  N'+40-700-000-001', N'RO', '1990-03-05', N'Str. Lalelelor 10, București'),
(@custBob,   N'bob@finbank.test',    N'hash_pwd_bob',    N'Customer', '1991013418016', SYSUTCDATETIME(), N'Bob Ionescu',   N'+40-700-000-002', N'RO', '1987-11-21', N'Str. Mărului 5, Cluj-Napoca'),
(@custCarol, N'carol@finbank.test',  N'hash_pwd_carol',  N'Customer', '1740506414234', SYSUTCDATETIME(), N'Carol Pop',     N'+40-700-000-003', N'RO', '1993-06-14', N'Timișoara, Bd. Revoluției 1'),
(@empReviewer, N'reviewer@finbank.test', N'hash_pwd_reviewer', N'Employee', '1890820410547', SYSUTCDATETIME(), N'Risk Reviewer', N'+40-700-000-004', N'RO', NULL, N'București HQ');

-- Accounts
DECLARE @aliceEur NVARCHAR(34) = N'RO71BANK000000000000000001';
DECLARE @aliceRon NVARCHAR(34) = N'RO71BANK000000000000000002';
DECLARE @bobEur   NVARCHAR(34) = N'RO71BANK000000000000000003';
DECLARE @carolUsd NVARCHAR(34) = N'RO71BANK000000000000000004';

INSERT INTO dbo.Accounts (IBAN, CustomerId, CreatedAt, Balance, Currency)
VALUES
(@aliceEur, @custAlice, SYSUTCDATETIME(), 1000.50, 'EUR'),
(@aliceRon, @custAlice, SYSUTCDATETIME(), 5345.00, 'RON'),
(@bobEur,   @custBob,   SYSUTCDATETIME(), 420.00,  'EUR'),
(@carolUsd, @custCarol, SYSUTCDATETIME(), 100.99,  'USD');

-- Transfers
DECLARE @t1 UNIQUEIDENTIFIER = NEWID();
DECLARE @t2 UNIQUEIDENTIFIER = NEWID();
DECLARE @t3 UNIQUEIDENTIFIER = NEWID();

INSERT INTO dbo.Transfers
(TransferId, FromIban, ToIban, ReviewedBy, CreatedAt, Status, Amount, Currency, Reason, CompletedAt, PolicyVersion)
VALUES
(@t1, @aliceEur, @bobEur,   NULL,         SYSUTCDATETIME(), 'Completed', 550.00, 'EUR', NULL,           SYSUTCDATETIME(), 'v1'),
(@t2, @carolUsd, @aliceEur, @empReviewer, SYSUTCDATETIME(), 'Rejected',  999.99, 'USD', N'KYC Blocked', SYSUTCDATETIME(), 'v1'),
(@t3, @aliceRon, @bobEur,   NULL,         SYSUTCDATETIME(), 'Pending',    75.25, 'RON', NULL,           NULL,             'v1');


----------------------
-- Reset & Seed KYC --
----------------------
USE KYC;

DELETE FROM dbo.CustomerRisk;

INSERT INTO dbo.CustomerRisk (Cnp, RiskStatus, UpdatedAt)
VALUES
('1760226055524', 'Low',     SYSUTCDATETIME()),
('1991013418016',   'Medium',  SYSUTCDATETIME()),
('1740506414234', 'High',    SYSUTCDATETIME());

PRINT 'Seeding completed successfully.';
GO
