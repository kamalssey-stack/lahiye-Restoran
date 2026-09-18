-- =============================================================
-- RESTORAN SIFARISLERININ IDARE EDILMESI SISTEMI
-- SQL Server Database Script (DDL, DML, Indexes, Constraints)
-- =============================================================

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'RestaurantDb')
BEGIN
    CREATE DATABASE [RestaurantDb];
END
GO

USE [RestaurantDb];
GO

-- 1. Table: Categories
IF OBJECT_ID(N'[dbo].[OrderItems]', 'U') IS NOT NULL DROP TABLE [dbo].[OrderItems];
IF OBJECT_ID(N'[dbo].[MenuItems]', 'U') IS NOT NULL DROP TABLE [dbo].[MenuItems];
IF OBJECT_ID(N'[dbo].[Categories]', 'U') IS NOT NULL DROP TABLE [dbo].[Categories];
IF OBJECT_ID(N'[dbo].[Orders]', 'U') IS NOT NULL DROP TABLE [dbo].[Orders];
GO

CREATE TABLE [dbo].[Categories] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

-- 2. Table: MenuItems
CREATE TABLE [dbo].[MenuItems] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(150) NOT NULL,
    [Price] DECIMAL(18,2) NOT NULL,
    [CategoryId] INT NOT NULL,
    CONSTRAINT [PK_MenuItems] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MenuItems_Categories] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories] ([Id]),
    CONSTRAINT [UQ_MenuItems_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);
GO

-- 3. Table: Orders
CREATE TABLE [dbo].[Orders] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [TotalAmount] DECIMAL(18,2) NOT NULL,
    [Date] DATETIME2(7) NOT NULL DEFAULT (GETDATE()),
    CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

-- 4. Table: OrderItems
CREATE TABLE [dbo].[OrderItems] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [OrderId] INT NOT NULL,
    [MenuItemId] INT NOT NULL,
    [Count] INT NOT NULL,
    [UnitPrice] DECIMAL(18,2) NOT NULL,
    CONSTRAINT [PK_OrderItems] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_OrderItems_Orders] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Orders] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrderItems_MenuItems] FOREIGN KEY ([MenuItemId]) REFERENCES [dbo].[MenuItems] ([Id])
);
GO

-- =============================================================
-- SEED DATA
-- =============================================================
SET IDENTITY_INSERT [dbo].[Categories] ON;
INSERT INTO [dbo].[Categories] ([Id], [Name]) VALUES
(1, N'Şorbalar (Sup)'),
(2, N'Əsas Yeməklər'),
(3, N'İçkilər'),
(4, N'Desertlər'),
(5, N'Qəlyanaltılar');
SET IDENTITY_INSERT [dbo].[Categories] OFF;
GO

SET IDENTITY_INSERT [dbo].[MenuItems] ON;
INSERT INTO [dbo].[MenuItems] ([Id], [Name], [Price], [CategoryId]) VALUES
(1, N'Mərci Şorbası', 4.50, 1),
(2, N'Düşbərə', 6.00, 1),
(3, N'Lülə Kabab', 12.00, 2),
(4, N'Tikə Kabab', 14.00, 2),
(5, N'Toyuq Şnitsel', 9.50, 2),
(6, N'Coca-Cola 0.5L', 2.50, 3),
(7, N'Ayran', 1.50, 3),
(8, N'Təbii Şirə', 3.00, 3),
(9, N'Çizkeyk', 7.00, 4),
(10, N'Paxlava', 5.00, 4);
SET IDENTITY_INSERT [dbo].[MenuItems] OFF;
GO

-- =============================================================
-- STORED PROCEDURES / QUERIES FOR OPERATIONS
-- =============================================================

-- 1. Bütün menyu məhsulları (Kateqoriya adı ilə)
CREATE OR ALTER PROCEDURE sp_GetAllMenuItems
AS
BEGIN
    SELECT m.Id, m.Name AS MenuItemName, m.Price, c.Name AS CategoryName
    FROM MenuItems m
    INNER JOIN Categories c ON m.CategoryId = c.Id;
END;
GO

-- 2. Kateqoriyaya görə menyu məhsulları
CREATE OR ALTER PROCEDURE sp_GetMenuItemsByCategory
    @CategoryId INT
AS
BEGIN
    SELECT m.Id, m.Name AS MenuItemName, m.Price, c.Name AS CategoryName
    FROM MenuItems m
    INNER JOIN Categories c ON m.CategoryId = c.Id
    WHERE m.CategoryId = @CategoryId;
END;
GO

-- 3. Qiymət aralığına görə menyu məhsulları
CREATE OR ALTER PROCEDURE sp_GetMenuItemsByPriceRange
    @MinPrice DECIMAL(18,2),
    @MaxPrice DECIMAL(18,2)
AS
BEGIN
    SELECT m.Id, m.Name AS MenuItemName, m.Price, c.Name AS CategoryName
    FROM MenuItems m
    INNER JOIN Categories c ON m.CategoryId = c.Id
    WHERE m.Price BETWEEN @MinPrice AND @MaxPrice;
END;
GO

-- 4. Ada görə axtarış (Search)
CREATE OR ALTER PROCEDURE sp_SearchMenuItemsByName
    @SearchText NVARCHAR(150)
AS
BEGIN
    SELECT m.Id, m.Name AS MenuItemName, m.Price, c.Name AS CategoryName
    FROM MenuItems m
    INNER JOIN Categories c ON m.CategoryId = c.Id
    WHERE m.Name LIKE N'%' + @SearchText + N'%';
END;
GO

-- 5. Bütün Sifarişlər (Ümumi say və məbləğlə)
CREATE OR ALTER PROCEDURE sp_GetAllOrders
AS
BEGIN
    SELECT 
        o.Id AS OrderId,
        o.TotalAmount,
        ISNULL(SUM(oi.Count), 0) AS TotalItemCount,
        o.Date
    FROM Orders o
    LEFT JOIN OrderItems oi ON o.Id = oi.OrderId
    GROUP BY o.Id, o.TotalAmount, o.Date
    ORDER BY o.Date DESC;
END;
GO

-- 6. Tarix aralığına görə sifarişlər
CREATE OR ALTER PROCEDURE sp_GetOrdersByDatesInterval
    @StartDate DATETIME2,
    @EndDate DATETIME2
AS
BEGIN
    SELECT 
        o.Id AS OrderId,
        o.TotalAmount,
        ISNULL(SUM(oi.Count), 0) AS TotalItemCount,
        o.Date
    FROM Orders o
    LEFT JOIN OrderItems oi ON o.Id = oi.OrderId
    WHERE o.Date BETWEEN @StartDate AND @EndDate
    GROUP BY o.Id, o.TotalAmount, o.Date
    ORDER BY o.Date DESC;
END;
GO

-- 7. Məbləğ aralığına görə sifarişlər
CREATE OR ALTER PROCEDURE sp_GetOrdersByPriceInterval
    @MinAmount DECIMAL(18,2),
    @MaxAmount DECIMAL(18,2)
AS
BEGIN
    SELECT 
        o.Id AS OrderId,
        o.TotalAmount,
        ISNULL(SUM(oi.Count), 0) AS TotalItemCount,
        o.Date
    FROM Orders o
    LEFT JOIN OrderItems oi ON o.Id = oi.OrderId
    WHERE o.TotalAmount BETWEEN @MinAmount AND @MaxAmount
    GROUP BY o.Id, o.TotalAmount, o.Date
    ORDER BY o.Date DESC;
END;
GO

-- 8. Nömrəyə görə sifariş detalları
CREATE OR ALTER PROCEDURE sp_GetOrderDetailsByNo
    @OrderId INT
AS
BEGIN
    -- Sifariş haqqında ümumi məlumat
    SELECT 
        o.Id AS OrderId,
        o.TotalAmount,
        ISNULL(SUM(oi.Count), 0) AS TotalItemCount,
        o.Date
    FROM Orders o
    LEFT JOIN OrderItems oi ON o.Id = oi.OrderId
    WHERE o.Id = @OrderId
    GROUP BY o.Id, o.TotalAmount, o.Date;

    -- Sifarişə daxil olan hər bir məhsul
    SELECT 
        oi.Id AS OrderItemId,
        m.Name AS MenuItemName,
        oi.Count,
        oi.UnitPrice,
        (oi.Count * oi.UnitPrice) AS SubTotal
    FROM OrderItems oi
    INNER JOIN MenuItems m ON oi.MenuItemId = m.Id
    WHERE oi.OrderId = @OrderId;
END;
GO