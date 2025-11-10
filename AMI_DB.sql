-- ===========================================================
-- 1️⃣ Create Database
-- ===========================================================
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'AMI_DB')
BEGIN
    DROP DATABASE AMI_DB;
END
GO

CREATE DATABASE AMI_DB;
GO

USE AMI_DB;
GO

-- ===========================================================
-- 2️⃣ ORGUNIT TABLE
-- ===========================================================
-- Self-referencing hierarchy for Zone → Substation → Feeder → DTR
-- Using ON DELETE NO ACTION to avoid multiple cascade paths
CREATE TABLE OrgUnit (
    OrgUnitId INT IDENTITY PRIMARY KEY,
    Type VARCHAR(20) NOT NULL CHECK (Type IN ('Zone','Substation','Feeder','DTR')),
    Name NVARCHAR(100) NOT NULL,
    ParentId INT NULL,
    CONSTRAINT FK_OrgUnit_Parent FOREIGN KEY (ParentId) REFERENCES OrgUnit(OrgUnitId) ON DELETE NO ACTION
);
GO

-- ===========================================================
-- 3️⃣ TARIFF TABLE
-- ===========================================================
CREATE TABLE Tariff (
    TariffId INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    EffectiveFrom DATE NOT NULL,
    EffectiveTo DATE NULL,
    BaseRate DECIMAL(18,4) NOT NULL,
    TaxRate DECIMAL(18,4) NOT NULL DEFAULT 0,
    CreatedAt DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

-- ===========================================================
-- 4️⃣ TARIFF SLAB TABLE
-- ===========================================================
-- Each slab defines a range of kWh and rate
CREATE TABLE TariffSlab (
    TariffSlabId INT IDENTITY PRIMARY KEY,
    TariffId INT NOT NULL,
    FromKwh DECIMAL(18,6) NOT NULL,
    ToKwh DECIMAL(18,6) NOT NULL,
    RatePerKwh DECIMAL(18,6) NOT NULL,
    Sequence INT DEFAULT 1,
    CreatedAt DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT CK_TariffSlab_Range CHECK (FromKwh >= 0 AND ToKwh > FromKwh),
    CONSTRAINT FK_TariffSlab_Tariff FOREIGN KEY (TariffId) REFERENCES Tariff(TariffId) ON DELETE CASCADE
);
GO

-- ===========================================================
-- 5️⃣ CONSUMER TABLE
-- ===========================================================
-- Links consumer to OrgUnit (DTR) and Tariff
CREATE TABLE Consumer (
    ConsumerId BIGINT IDENTITY PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Address NVARCHAR(500) NULL,
    Phone NVARCHAR(30) NULL,
    Email NVARCHAR(200) NULL,
    OrgUnitId INT NOT NULL,
    TariffId INT NOT NULL,
    Lat DECIMAL(9,6) NULL,
    Lon DECIMAL(9,6) NULL,
    Status VARCHAR(20) NOT NULL DEFAULT 'Active' CHECK (Status IN ('Active','Inactive')),
    CreatedAt DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy NVARCHAR(100) NOT NULL DEFAULT 'system',
    UpdatedAt DATETIME2(3) NULL,
    UpdatedBy NVARCHAR(100) NULL,
    CONSTRAINT FK_Consumer_OrgUnit FOREIGN KEY (OrgUnitId) REFERENCES OrgUnit(OrgUnitId) ON DELETE NO ACTION,
    CONSTRAINT FK_Consumer_Tariff FOREIGN KEY (TariffId) REFERENCES Tariff(TariffId) ON DELETE NO ACTION
);
GO

-- ===========================================================
-- 6️⃣ METER TABLE
-- ===========================================================
-- Stores smart meter info and links to consumer
CREATE TABLE Meter (
    MeterSerialNo NVARCHAR(50) NOT NULL PRIMARY KEY,
    IpAddress NVARCHAR(45) NOT NULL,
    ICCID NVARCHAR(30) NOT NULL,
    IMSI NVARCHAR(30) NOT NULL,
    Manufacturer NVARCHAR(100) NOT NULL,
    Firmware NVARCHAR(50) NULL,
    Category NVARCHAR(50) NOT NULL,
    InstallTsUtc DATETIME2(3) NOT NULL,
    Status VARCHAR(20) NOT NULL DEFAULT 'Active' 
        CHECK (Status IN ('Active','Inactive','Decommissioned')),
    ConsumerId BIGINT NULL,
    CONSTRAINT FK_Meter_Consumer FOREIGN KEY (ConsumerId) REFERENCES Consumer(ConsumerId) ON DELETE SET NULL
);
GO

CREATE UNIQUE INDEX IX_Meter_Serial ON Meter(MeterSerialNo);
GO

-- ===========================================================
-- 7️⃣ METER READING TABLE
-- ===========================================================
-- Stores energy readings per meter
CREATE TABLE MeterReading (
    MeterReadingId BIGINT IDENTITY PRIMARY KEY,
    MeterSerialNo NVARCHAR(50) NOT NULL,
    ReadingDateTime DATETIME2(3) NOT NULL,
    ConsumptionKwh DECIMAL(18,4) NOT NULL,
    Voltage DECIMAL(10,2) NULL,
    Ampere DECIMAL(10,2) NULL,
    PowerFactor DECIMAL(10,4) NULL,
    Frequency DECIMAL(10,4) NULL,
    CreatedAt DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_MeterReading_Meter FOREIGN KEY (MeterSerialNo) REFERENCES Meter(MeterSerialNo) ON DELETE CASCADE
);
GO

-- ===========================================================
-- 8️⃣ BILL TABLE
-- ===========================================================
CREATE TABLE Bill (
    BillId BIGINT IDENTITY PRIMARY KEY,
    ConsumerId BIGINT NOT NULL,
    MeterSerialNo NVARCHAR(50) NULL,
    BillingPeriodStart DATE NOT NULL,
    BillingPeriodEnd DATE NOT NULL,
    UnitsConsumed DECIMAL(18,4) NOT NULL,
    TotalAmount DECIMAL(18,4) NOT NULL,
    TariffId INT NOT NULL,
    BillGeneratedAt DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Bill_Consumer FOREIGN KEY (ConsumerId) REFERENCES Consumer(ConsumerId) ON DELETE CASCADE,
    CONSTRAINT FK_Bill_Meter FOREIGN KEY (MeterSerialNo) REFERENCES Meter(MeterSerialNo) ON DELETE SET NULL,
    CONSTRAINT FK_Bill_Tariff FOREIGN KEY (TariffId) REFERENCES Tariff(TariffId) ON DELETE NO ACTION
);
GO

-- ===========================================================
-- 9️⃣ BILL DETAIL TABLE
-- ===========================================================
CREATE TABLE BillDetail (
    BillDetailId BIGINT IDENTITY PRIMARY KEY,
    BillId BIGINT NOT NULL,
    TariffSlabId INT NULL,
    Units DECIMAL(18,4) NOT NULL,
    Rate DECIMAL(18,4) NOT NULL,
    Amount DECIMAL(18,4) NOT NULL,
    CONSTRAINT FK_BillDetail_Bill FOREIGN KEY (BillId) REFERENCES Bill(BillId) ON DELETE CASCADE,
    CONSTRAINT FK_BillDetail_TariffSlab FOREIGN KEY (TariffSlabId) REFERENCES TariffSlab(TariffSlabId) ON DELETE SET NULL
);
GO

-- ===========================================================
-- 🔐 AUTHENTICATION TABLES
-- ===========================================================

-- Users
CREATE TABLE [User] (
    UserId BIGINT IDENTITY PRIMARY KEY,
    Email NVARCHAR(256) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    DisplayName NVARCHAR(200) NULL,
    Phone NVARCHAR(50) NULL,
    EmailConfirmed BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

-- Roles
CREATE TABLE [Role] (
    RoleId INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE
);
GO

-- UserRoles (Many-to-Many)
CREATE TABLE UserRole (
    UserId BIGINT NOT NULL,
    RoleId INT NOT NULL,
    PRIMARY KEY (UserId, RoleId),
    CONSTRAINT FK_UserRole_User FOREIGN KEY (UserId) REFERENCES [User](UserId) ON DELETE CASCADE,
    CONSTRAINT FK_UserRole_Role FOREIGN KEY (RoleId) REFERENCES [Role](RoleId) ON DELETE CASCADE
);
GO

-- RefreshTokens
CREATE TABLE RefreshToken (
    RefreshTokenId BIGINT IDENTITY PRIMARY KEY,
    UserId BIGINT NOT NULL,
    Token NVARCHAR(500) NOT NULL,
    ExpiresAt DATETIME2(3) NOT NULL,
    CreatedAt DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedByIp NVARCHAR(100) NULL,
    RevokedAt DATETIME2(3) NULL,
    ReplacedByToken NVARCHAR(500) NULL,
    CONSTRAINT FK_RefreshToken_User FOREIGN KEY (UserId) REFERENCES [User](UserId) ON DELETE CASCADE
);
GO

-- Optional: Connect system users to consumers
ALTER TABLE [User] ADD ConsumerId BIGINT NULL;
ALTER TABLE [User] ADD CONSTRAINT FK_User_Consumer FOREIGN KEY (ConsumerId) REFERENCES Consumer(ConsumerId) ON DELETE SET NULL;
GO


-- 1️⃣ OrgUnit hierarchy
SELECT OrgUnitId, Type, Name, ParentId
FROM OrgUnit
ORDER BY OrgUnitId;
GO

-- 2️⃣ Tariffs
SELECT TariffId, Name, EffectiveFrom, EffectiveTo, BaseRate, TaxRate, CreatedAt
FROM Tariff
ORDER BY TariffId;
GO

-- 3️⃣ Tariff Slabs
SELECT TariffSlabId, TariffId, FromKwh, ToKwh, RatePerKwh, Sequence, CreatedAt
FROM TariffSlab
ORDER BY TariffId, Sequence;
GO

-- 4️⃣ Consumers
SELECT ConsumerId, Name, Address, Phone, Email, OrgUnitId, TariffId, Lat, Lon, Status, CreatedAt
FROM Consumer
ORDER BY ConsumerId;
GO

-- 5️⃣ Meters
SELECT MeterSerialNo, IpAddress, ICCID, IMSI, Manufacturer, Firmware, Category, InstallTsUtc, Status, ConsumerId
FROM Meter
ORDER BY MeterSerialNo;
GO

-- 6️⃣ Meter Readings (with Ampere)
SELECT MeterReadingId, MeterSerialNo, ReadingDateTime, ConsumptionKwh, Voltage, Ampere, PowerFactor, Frequency, CreatedAt
FROM MeterReading
ORDER BY MeterSerialNo, ReadingDateTime;
GO

-- 7️⃣ Bills
SELECT BillId, ConsumerId, MeterSerialNo, BillingPeriodStart, BillingPeriodEnd, UnitsConsumed, TotalAmount, TariffId, BillGeneratedAt
FROM Bill
ORDER BY BillId;
GO

-- 8️⃣ Bill Details
SELECT BillDetailId, BillId, TariffSlabId, Units, Rate, Amount
FROM BillDetail
ORDER BY BillId, BillDetailId;
GO

-- 9️⃣ Users (Authentication)
SELECT UserId, Email, DisplayName, Phone, EmailConfirmed, CreatedAt, ConsumerId
FROM [User]
ORDER BY UserId;
GO

-- 🔟 Roles
SELECT RoleId, Name
FROM [Role]
ORDER BY RoleId;
GO

-- 1️⃣1️⃣ UserRoles
SELECT UserId, RoleId
FROM UserRole
ORDER BY UserId, RoleId;
GO

-- 1️⃣2️⃣ Refresh Tokens
SELECT RefreshTokenId, UserId, Token, ExpiresAt, CreatedAt, CreatedByIp, RevokedAt, ReplacedByToken
FROM RefreshToken
ORDER BY UserId, CreatedAt;
GO


WITH OrgHierarchy AS (
    SELECT OrgUnitId, Name, Type, ParentId, 0 AS Level
    FROM OrgUnit
    WHERE ParentId IS NULL
    UNION ALL
    SELECT o.OrgUnitId, o.Name, o.Type, o.ParentId, h.Level + 1
    FROM OrgUnit o
    INNER JOIN OrgHierarchy h ON o.ParentId = h.OrgUnitId
)
SELECT * FROM OrgHierarchy ORDER BY Level, OrgUnitId;


USE AMI_DB;
GO

-- ===========================================================
-- 1️⃣ ORGUNIT (Hierarchy: Zone → Substation → Feeder)
-- ===========================================================
INSERT INTO OrgUnit (Type, Name, ParentId) VALUES 
('Zone', 'North Zone', NULL),         -- 1
('Substation', 'Substation A', 1),    -- 2
('Feeder', 'Feeder A1', 2);           -- 3
GO

-- ===========================================================
-- 2️⃣ TARIFF
-- ===========================================================
INSERT INTO Tariff (Name, EffectiveFrom, EffectiveTo, BaseRate, TaxRate)
VALUES
('Domestic Tariff', '2024-01-01', NULL, 5.75, 0.10),
('Commercial Tariff', '2024-01-01', NULL, 7.25, 0.15),
('Industrial Tariff', '2024-01-01', NULL, 6.50, 0.12);
GO

-- ===========================================================
-- 3️⃣ TARIFF SLAB
-- ===========================================================
INSERT INTO TariffSlab (TariffId, FromKwh, ToKwh, RatePerKwh, Sequence)
VALUES
(1, 0, 100, 5.00, 1),
(1, 101, 300, 6.50, 2),
(1, 301, 99999, 8.00, 3),
(2, 0, 200, 7.00, 1),
(2, 201, 500, 8.50, 2),
(3, 0, 1000, 6.75, 1);
GO

-- ===========================================================
-- 4️⃣ CONSUMER
-- ===========================================================
INSERT INTO Consumer (Name, Address, Phone, Email, OrgUnitId, TariffId, Lat, Lon, Status, CreatedBy)
VALUES
('John Doe', '123 Main St, North City', '9876543210', 'john@example.com', 3, 1, 12.9716, 77.5946, 'Active', 'system'),
('ABC Stores', '45 Market Rd, Substation Area', '9123456780', 'abcstores@example.com', 3, 2, 13.0827, 80.2707, 'Active', 'system'),
('XYZ Industries', '88 Industrial Estate, Zone 1', '9988776655', 'xyz@example.com', 3, 3, 19.0760, 72.8777, 'Inactive', 'system');
GO

-- ===========================================================
-- 5️⃣ METER
-- ===========================================================
INSERT INTO Meter (MeterSerialNo, IpAddress, ICCID, IMSI, Manufacturer, Firmware, Category, InstallTsUtc, Status, ConsumerId)
VALUES
('MTR1001', '192.168.1.10', '8991101200001234567', '404101234567890', 'SecureMeters', '1.0.5', 'Single Phase', SYSUTCDATETIME(), 'Active', 1),
('MTR1002', '192.168.1.11', '8991101200009876543', '404101987654321', 'L&T', '2.1.0', 'Three Phase', SYSUTCDATETIME(), 'Active', 2),
('MTR1003', '192.168.1.12', '8991101200005556667', '404101555666777', 'Genus', '1.2.3', 'Single Phase', SYSUTCDATETIME(), 'Inactive', 3);
GO

-- ===========================================================
-- 6️⃣ METER READING
-- ===========================================================
INSERT INTO MeterReading (MeterSerialNo, ReadingDateTime, ConsumptionKwh, Voltage, Ampere, PowerFactor, Frequency)
VALUES
('MTR1001', DATEADD(DAY, -1, SYSUTCDATETIME()), 15.25, 230.5, 10.2, 0.98, 50.0),
('MTR1002', DATEADD(DAY, -1, SYSUTCDATETIME()), 42.70, 400.0, 25.6, 0.96, 50.1),
('MTR1003', DATEADD(DAY, -1, SYSUTCDATETIME()), 88.00, 230.0, 30.0, 0.90, 49.9);
GO

-- ===========================================================
-- 7️⃣ BILL
-- ===========================================================
INSERT INTO Bill (ConsumerId, MeterSerialNo, BillingPeriodStart, BillingPeriodEnd, UnitsConsumed, TotalAmount, TariffId)
VALUES
(1, 'MTR1001', '2024-10-01', '2024-10-31', 120.5, 720.00, 1),
(2, 'MTR1002', '2024-10-01', '2024-10-31', 315.7, 2450.00, 2),
(3, 'MTR1003', '2024-10-01', '2024-10-31', 600.0, 4100.00, 3);
GO

-- ===========================================================
-- 8️⃣ BILL DETAIL
-- ===========================================================
INSERT INTO BillDetail (BillId, TariffSlabId, Units, Rate, Amount)
VALUES
(1, 1, 100, 5.00, 500.00),
(1, 2, 20.5, 6.50, 133.25),
(2, 4, 200, 7.00, 1400.00),
(2, 5, 115.7, 8.50, 983.45),
(3, 6, 600, 6.75, 4050.00);
GO

-- ===========================================================
-- 9️⃣ USERS
-- ===========================================================
INSERT INTO [User] (Email, PasswordHash, DisplayName, Phone, EmailConfirmed, ConsumerId)
VALUES
('admin@ami.com', 'HASHEDPASSWORD1', 'Admin User', '9000000000', 1, NULL),
('john@ami.com', 'HASHEDPASSWORD2', 'John Doe', '9876543210', 1, 1),
('abc@ami.com', 'HASHEDPASSWORD3', 'ABC Manager', '9123456780', 1, 2);
GO

-- ===========================================================
-- 🔟 ROLES
-- ===========================================================
INSERT INTO [Role] (Name)
VALUES
('Admin'),
('Operator'),
('Consumer');
GO

-- ===========================================================
-- 1️⃣1️⃣ USER ROLE
-- ===========================================================
INSERT INTO UserRole (UserId, RoleId)
VALUES
(1, 1), -- Admin User → Admin
(2, 3), -- John Doe → Consumer
(3, 2); -- ABC Manager → Operator
GO

-- ===========================================================
-- 1️⃣2️⃣ REFRESH TOKENS
-- ===========================================================
INSERT INTO RefreshToken (UserId, Token, ExpiresAt, CreatedByIp)
VALUES
(1, 'token_admin_123', DATEADD(DAY, 7, SYSUTCDATETIME()), '127.0.0.1'),
(2, 'token_john_456', DATEADD(DAY, 7, SYSUTCDATETIME()), '127.0.0.1'),
(3, 'token_abc_789', DATEADD(DAY, 7, SYSUTCDATETIME()), '127.0.0.1');
GO

SELECT * FROM OrgUnit;
SELECT * FROM Tariff;
SELECT * FROM Consumer;
SELECT * FROM Meter;
SELECT * FROM MeterReading;
SELECT * FROM Bill;
SELECT * FROM [User];
SELECT * FROM Role;
SELECT * FROM UserRole;


USE AMI_DB;
GO

-- Clear existing users first (optional, if re-seeding)
DELETE FROM RefreshToken;
DELETE FROM UserRole;
DELETE FROM [User];
GO

-- ===========================================================
-- 👤 USERS (with real BCrypt hashes)
-- ===========================================================
INSERT INTO [User] (Email, PasswordHash, DisplayName, Phone, EmailConfirmed, ConsumerId)
VALUES
(
    'admin@ami.com',
    '$2a$11$9Lk4RnOOnDgS0y/hy1OYWuR.CUxvHBeCi5lEj3TQZ0E7CF3M/zPya',  -- Admin@123!
    'Admin User',
    '9000000000',
    1,
    NULL
),
(
    'john@ami.com',
    '$2a$11$9s5Qy46z2TVSkLRA7zXrRehsmT1dHwzu6WkTckF7N0nXyXxK3ENRu',  -- John@123!
    'John Doe',
    '9876543210',
    1,
    1
),
(
    'abc@ami.com',
    '$2a$11$h96h5YbEXq9F5H5ciV7HBu5M6tZpQYb4OiPj5jSG3zciQCFpD9rs2',  -- ABC@123!
    'ABC Manager',
    '9123456780',
    1,
    2
);
GO

-- ===========================================================
-- 🔐 ROLES
-- ===========================================================
DELETE FROM UserRole;
DELETE FROM [Role];
GO

INSERT INTO [Role] (Name)
VALUES
('Admin'),
('Operator'),
('Consumer');
GO

-- ===========================================================
-- 👥 USER ROLES
-- ===========================================================
DECLARE @AdminRoleId INT = (SELECT RoleId FROM [Role] WHERE Name = 'Admin');
DECLARE @OperatorRoleId INT = (SELECT RoleId FROM [Role] WHERE Name = 'Operator');
DECLARE @ConsumerRoleId INT = (SELECT RoleId FROM [Role] WHERE Name = 'Consumer');

INSERT INTO UserRole (UserId, RoleId)
VALUES
(1, @AdminRoleId),   -- Admin User → Admin
(2, @ConsumerRoleId),-- John Doe → Consumer
(3, @OperatorRoleId);-- ABC Manager → Operator
GO

-- ===========================================================
-- 🧾 TEST RESULT
-- ===========================================================
SELECT UserId, Email, DisplayName, Phone, EmailConfirmed FROM [User];
SELECT * FROM [Role];
SELECT * FROM UserRole;
GO


DROP TABLE IF EXISTS [UserRole];
DROP TABLE IF EXISTS [User];
DROP TABLE IF EXISTS [Role];

-- Disable FK temporarily
ALTER TABLE [UserRole] NOCHECK CONSTRAINT ALL;
ALTER TABLE [RefreshToken] NOCHECK CONSTRAINT ALL;

-- Delete dependent data first
DELETE FROM [RefreshToken];
DELETE FROM [UserRole];

-- Now clear users and roles
DELETE FROM [User];
DELETE FROM [Role];

-- Re-enable constraints
ALTER TABLE [UserRole] CHECK CONSTRAINT ALL;
ALTER TABLE [RefreshToken] CHECK CONSTRAINT ALL;



SELECT 
    fk.name AS FK_Name,
    tp.name AS ParentTable,
    cp.name AS ParentColumn,
    tr.name AS ReferencedTable,
    cr.name AS ReferencedColumn
FROM sys.foreign_keys AS fk
INNER JOIN sys.foreign_key_columns AS fkc ON fk.object_id = fkc.constraint_object_id
INNER JOIN sys.tables AS tp ON fkc.parent_object_id = tp.object_id
INNER JOIN sys.columns AS cp ON fkc.parent_object_id = cp.object_id AND fkc.parent_column_id = cp.column_id
INNER JOIN sys.tables AS tr ON fkc.referenced_object_id = tr.object_id
INNER JOIN sys.columns AS cr ON fkc.referenced_object_id = cr.object_id AND fkc.referenced_column_id = cr.column_id
WHERE tr.name = 'User';


-- ============================================
-- 🧹 Step 1: Drop FK from RefreshToken → User
-- ============================================
IF EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_RefreshToken_User'
)
BEGIN
    ALTER TABLE [RefreshToken] DROP CONSTRAINT [FK_RefreshToken_User];
END
GO

-- ============================================
-- 💣 Step 2: Drop old tables if they exist
-- ============================================
DROP TABLE IF EXISTS [UserRole];
DROP TABLE IF EXISTS [Role];
DROP TABLE IF EXISTS [User];
GO

-- ============================================
-- 🧱 Step 3: Recreate Role table
-- ============================================
CREATE TABLE [Role] (
    [RoleId] INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL UNIQUE
);
GO

-- ============================================
-- 🧱 Step 4: Recreate User table
-- ============================================
CREATE TABLE [User] (
    [UserId] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [Email] NVARCHAR(256) NOT NULL UNIQUE,
    [PasswordHash] NVARCHAR(MAX) NOT NULL,
    [DisplayName] NVARCHAR(200),
    [Phone] NVARCHAR(50),
    [EmailConfirmed] BIT DEFAULT 0,
    [CreatedAt] DATETIME2(3) DEFAULT SYSUTCDATETIME(),
    [ConsumerId] BIGINT NULL
);
GO

-- ============================================
-- 🧱 Step 5: Recreate UserRole junction table
-- ============================================
CREATE TABLE [UserRole] (
    [UserId] BIGINT NOT NULL,
    [RoleId] INT NOT NULL,
    CONSTRAINT PK_UserRole PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT FK_UserRole_User FOREIGN KEY ([UserId]) REFERENCES [User]([UserId]) ON DELETE CASCADE,
    CONSTRAINT FK_UserRole_Role FOREIGN KEY ([RoleId]) REFERENCES [Role]([RoleId]) ON DELETE CASCADE
);
GO

-- ============================================
-- 🔁 Step 6: Recreate FK from RefreshToken → User
-- ============================================
ALTER TABLE [RefreshToken]
ADD CONSTRAINT FK_RefreshToken_User
FOREIGN KEY ([UserId]) REFERENCES [User]([UserId]) ON DELETE CASCADE;
GO

-- ============================================
-- 🌱 Step 7: Seed initial roles
-- ============================================
INSERT INTO [Role] (Name)
VALUES ('Admin'), ('User');
GO

-- ============================================
-- 👤 Step 8: Seed default admin user
-- ============================================
INSERT INTO [User] (Email, PasswordHash, DisplayName, Phone, EmailConfirmed)
VALUES (
    'admin@ami.com',
    '$2a$11$9Lk4RnOOnDgS0y/hy1OYWuR.CUxvHBeCi5lEj3TQZ0E7CF3M/zPya', -- bcrypt hash for "admin123"
    'System Admin',
    '9999999999',
    1
);
GO

-- ============================================
-- 🔗 Step 9: Assign admin role to the user
-- ============================================
INSERT INTO [UserRole] (UserId, RoleId)
SELECT u.UserId, r.RoleId
FROM [User] u
JOIN [Role] r ON r.Name = 'Admin'
WHERE u.Email = 'admin@ami.com';
GO

PRINT '✅ User, Role, and UserRole tables successfully recreated and seeded with Admin and User roles.';
select * from [User]

-- ============================================
-- 🧹 Step 1: Clear existing OrgUnit data safely
-- ============================================

-- Disable FK temporarily (to avoid self-reference deletion error)
ALTER TABLE OrgUnit NOCHECK CONSTRAINT FK_OrgUnit_Parent;
GO

-- Delete all rows
DELETE FROM OrgUnit;
GO

-- Reset the identity counter
DBCC CHECKIDENT ('OrgUnit', RESEED, 0);
GO

-- Re-enable FK constraint
ALTER TABLE OrgUnit CHECK CONSTRAINT FK_OrgUnit_Parent;
GO


-- ============================================
-- 🌳 Step 2: Insert Hierarchical Org Units
-- ============================================

-- 1️⃣ ZONES
INSERT INTO OrgUnit (Type, Name, ParentId)
VALUES 
('Zone', 'Zone A', NULL),
('Zone', 'Zone B', NULL);
GO

-- 2️⃣ SUBSTATIONS
INSERT INTO OrgUnit (Type, Name, ParentId)
VALUES
('Substation', 'Substation A1', 1),
('Substation', 'Substation A2', 1),
('Substation', 'Substation B1', 2);
GO

-- 3️⃣ FEEDERS
INSERT INTO OrgUnit (Type, Name, ParentId)
VALUES
('Feeder', 'Feeder A1-F1', 3),
('Feeder', 'Feeder A1-F2', 3),
('Feeder', 'Feeder A2-F1', 4),
('Feeder', 'Feeder B1-F1', 5);
GO

-- 4️⃣ DTRs
INSERT INTO OrgUnit (Type, Name, ParentId)
VALUES
('DTR', 'DTR A1-F1-D1', 6),
('DTR', 'DTR A1-F1-D2', 6),
('DTR', 'DTR A1-F2-D1', 7),
('DTR', 'DTR A1-F2-D2', 7),
('DTR', 'DTR A2-F1-D1', 8),
('DTR', 'DTR A2-F1-D2', 8),
('DTR', 'DTR B1-F1-D1', 9),
('DTR', 'DTR B1-F1-D2', 9);
GO


-- ============================================
-- ✅ Step 3: Verify hierarchy
-- ============================================
SELECT OrgUnitId, Type, Name, ParentId
FROM OrgUnit
ORDER BY OrgUnitId;
GO


BEGIN TRY
    BEGIN TRAN;

    PRINT '--- Step 1: Ensure Unassigned OrgUnit exists ---';
    IF NOT EXISTS (SELECT 1 FROM OrgUnit WHERE Name = 'Unassigned')
    BEGIN
        INSERT INTO OrgUnit (Type, Name, ParentId)
        VALUES ('DTR', 'Unassigned', NULL);
        PRINT 'Created new Unassigned OrgUnit.';
    END
    ELSE
        PRINT 'Unassigned OrgUnit already exists.';

    DECLARE @UnassignedId INT = (
        SELECT TOP (1) OrgUnitId FROM OrgUnit WHERE Name = 'Unassigned' ORDER BY OrgUnitId
    );

    PRINT '--- Step 2: Move Consumers to Unassigned ---';
    UPDATE Consumer
    SET OrgUnitId = @UnassignedId
    WHERE OrgUnitId <> @UnassignedId;

    DECLARE @Moved INT = @@ROWCOUNT;
    PRINT CONCAT('Moved ', @Moved, ' consumers to Unassigned.');

    PRINT '--- Step 3: Delete all other OrgUnits ---';
    DELETE FROM OrgUnit
    WHERE OrgUnitId <> @UnassignedId;

    DECLARE @Deleted INT = @@ROWCOUNT;
    PRINT CONCAT('Deleted ', @Deleted, ' org units.');

    PRINT '--- Step 4: Reseed OrgUnit identity ---';
    DBCC CHECKIDENT ('OrgUnit', RESEED, @UnassignedId);

    COMMIT TRAN;
    PRINT '✅ OrgUnit cleanup completed successfully.';
END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    PRINT '❌ Error occurred:';
    PRINT ERROR_MESSAGE();
END CATCH;





-- Example hierarchy
INSERT INTO OrgUnit (Type, Name, ParentId)
VALUES
('Zone', 'Zone A', NULL),
('Substation', 'Substation 1', 1),
('Feeder', 'Feeder 1A', 2),
('DTR', 'DTR 001', 3);


SELECT OrgUnitId, Name, Type, ParentId FROM OrgUnit ORDER BY OrgUnitId;




-- ====================================================
-- 🧹 STEP 1: Disable foreign key temporarily
-- ====================================================
ALTER TABLE OrgUnit NOCHECK CONSTRAINT FK_OrgUnit_Parent;

-- If Consumers are linked to OrgUnit, disable that too:
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Consumer_OrgUnit')
    ALTER TABLE Consumer NOCHECK CONSTRAINT FK_Consumer_OrgUnit;

-- ====================================================
-- 🧽 STEP 2: Delete existing OrgUnit data safely
-- ====================================================
DELETE FROM OrgUnit;

-- Reset identity counter
DBCC CHECKIDENT ('OrgUnit', RESEED, 0);

-- ====================================================
-- 🧩 STEP 3: Insert new hierarchy data
-- ====================================================

-- ZONES
INSERT INTO OrgUnit (Type, Name, ParentId) VALUES
('Zone', 'Zone A', NULL),
('Zone', 'Zone B', NULL);

-- SUBSTATIONS
INSERT INTO OrgUnit (Type, Name, ParentId) VALUES
('Substation', 'Sub A1', 1),
('Substation', 'Sub A2', 1),
('Substation', 'Sub B1', 2),
('Substation', 'Sub B2', 2);

-- FEEDERS
INSERT INTO OrgUnit (Type, Name, ParentId) VALUES
('Feeder', 'Feeder A1-1', 3),
('Feeder', 'Feeder A1-2', 3),
('Feeder', 'Feeder A2-1', 4),
('Feeder', 'Feeder A2-2', 4),
('Feeder', 'Feeder B1-1', 5),
('Feeder', 'Feeder B1-2', 5),
('Feeder', 'Feeder B2-1', 6),
('Feeder', 'Feeder B2-2', 6);

-- DTRs
INSERT INTO OrgUnit (Type, Name, ParentId) VALUES
('DTR', 'DTR A1-1-1', 7),
('DTR', 'DTR A1-1-2', 7),
('DTR', 'DTR A1-2-1', 8),
('DTR', 'DTR A1-2-2', 8),
('DTR', 'DTR A2-1-1', 9),
('DTR', 'DTR A2-1-2', 9),
('DTR', 'DTR A2-2-1', 10),
('DTR', 'DTR A2-2-2', 10),
('DTR', 'DTR B1-1-1', 11),
('DTR', 'DTR B1-1-2', 11),
('DTR', 'DTR B1-2-1', 12),
('DTR', 'DTR B1-2-2', 12),
('DTR', 'DTR B2-1-1', 13),
('DTR', 'DTR B2-1-2', 13),
('DTR', 'DTR B2-2-1', 14),
('DTR', 'DTR B2-2-2', 14);

-- ====================================================
-- ✅ STEP 4: Re-enable foreign keys
-- ====================================================
ALTER TABLE OrgUnit CHECK CONSTRAINT FK_OrgUnit_Parent;

IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Consumer_OrgUnit')
    ALTER TABLE Consumer CHECK CONSTRAINT FK_Consumer_OrgUnit;

-- ====================================================
-- 🔍 STEP 5: Verify results
-- ====================================================
SELECT OrgUnitId, Type, Name, ParentId FROM OrgUnit ORDER BY OrgUnitId;


Select * from TariffSlab