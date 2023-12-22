
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 08/29/2023 11:33:32
-- Generated from EDMX file: D:\Git\ShengLin\VJINC\nissan\nissan\AdminLTE-ASP.NET-MVC\Models\CarShop.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [CarShopBK];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_dbo_AspNetUserClaims_dbo_AspNetUsers_UserId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AspNetUserClaims] DROP CONSTRAINT [FK_dbo_AspNetUserClaims_dbo_AspNetUsers_UserId];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_AspNetUserLogins_dbo_AspNetUsers_UserId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AspNetUserLogins] DROP CONSTRAINT [FK_dbo_AspNetUserLogins_dbo_AspNetUsers_UserId];
GO
IF OBJECT_ID(N'[dbo].[FK_AspNetUserRoles_AspNetRoles]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AspNetUserRoles] DROP CONSTRAINT [FK_AspNetUserRoles_AspNetRoles];
GO
IF OBJECT_ID(N'[dbo].[FK_AspNetUserRoles_AspNetUsers]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AspNetUserRoles] DROP CONSTRAINT [FK_AspNetUserRoles_AspNetUsers];
GO
IF OBJECT_ID(N'[dbo].[FK_AspNetUserRoles1_AspNetRoles]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AspNetUserRoles1] DROP CONSTRAINT [FK_AspNetUserRoles1_AspNetRoles];
GO
IF OBJECT_ID(N'[dbo].[FK_AspNetUserRoles1_AspNetUsers]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AspNetUserRoles1] DROP CONSTRAINT [FK_AspNetUserRoles1_AspNetUsers];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[AspNetRoles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetRoles];
GO
IF OBJECT_ID(N'[dbo].[AspNetUserClaims]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetUserClaims];
GO
IF OBJECT_ID(N'[dbo].[AspNetUserLogins]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetUserLogins];
GO
IF OBJECT_ID(N'[dbo].[AspNetUsers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetUsers];
GO
IF OBJECT_ID(N'[dbo].[Banner]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Banner];
GO
IF OBJECT_ID(N'[dbo].[Members]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Members];
GO
IF OBJECT_ID(N'[dbo].[Roles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Roles];
GO
IF OBJECT_ID(N'[dbo].[UserRoles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserRoles];
GO
IF OBJECT_ID(N'[dbo].[PasswordHistory]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PasswordHistory];
GO
IF OBJECT_ID(N'[dbo].[Menus]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Menus];
GO
IF OBJECT_ID(N'[dbo].[MyFavourites]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MyFavourites];
GO
IF OBJECT_ID(N'[dbo].[RoleMenus]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RoleMenus];
GO
IF OBJECT_ID(N'[dbo].[Shops]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Shops];
GO
IF OBJECT_ID(N'[dbo].[Subscriptions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Subscriptions];
GO
IF OBJECT_ID(N'[dbo].[News]', 'U') IS NOT NULL
    DROP TABLE [dbo].[News];
GO
IF OBJECT_ID(N'[dbo].[VisitOrders]', 'U') IS NOT NULL
    DROP TABLE [dbo].[VisitOrders];
GO
IF OBJECT_ID(N'[dbo].[Profiles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Profiles];
GO
IF OBJECT_ID(N'[dbo].[Dealers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Dealers];
GO
IF OBJECT_ID(N'[dbo].[DealerPersons]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DealerPersons];
GO
IF OBJECT_ID(N'[dbo].[OTPs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OTPs];
GO
IF OBJECT_ID(N'[dbo].[Log]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Log];
GO
IF OBJECT_ID(N'[dbo].[Sells]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Sells];
GO
IF OBJECT_ID(N'[dbo].[Banners]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Banners];
GO
IF OBJECT_ID(N'[dbo].[NoticeRecords]', 'U') IS NOT NULL
    DROP TABLE [dbo].[NoticeRecords];
GO
IF OBJECT_ID(N'[dbo].[Prepaid]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Prepaid];
GO
IF OBJECT_ID(N'[dbo].[SellsHistory]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SellsHistory];
GO
IF OBJECT_ID(N'[dbo].[BuyHistory]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BuyHistory];
GO
IF OBJECT_ID(N'[dbo].[CarCompareHistory]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CarCompareHistory];
GO
IF OBJECT_ID(N'[dbo].[ResetPwLinks]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ResetPwLinks];
GO
IF OBJECT_ID(N'[dbo].[Code]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Code];
GO
IF OBJECT_ID(N'[dbo].[Banner1Set]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Banner1Set];
GO
IF OBJECT_ID(N'[dbo].[Members1Set]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Members1Set];
GO
IF OBJECT_ID(N'[dbo].[OTPs1Set]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OTPs1Set];
GO
IF OBJECT_ID(N'[dbo].[AspNetUserRoles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetUserRoles];
GO
IF OBJECT_ID(N'[dbo].[AspNetUserRoles1]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetUserRoles1];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'AspNetRoles'
CREATE TABLE [dbo].[AspNetRoles] (
    [Id] nvarchar(128)  NOT NULL,
    [Name] nvarchar(256)  NOT NULL
);
GO

-- Creating table 'AspNetUserClaims'
CREATE TABLE [dbo].[AspNetUserClaims] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] nvarchar(128)  NOT NULL,
    [ClaimType] nvarchar(max)  NULL,
    [ClaimValue] nvarchar(max)  NULL
);
GO

-- Creating table 'AspNetUserLogins'
CREATE TABLE [dbo].[AspNetUserLogins] (
    [LoginProvider] nvarchar(128)  NOT NULL,
    [ProviderKey] nvarchar(128)  NOT NULL,
    [UserId] nvarchar(128)  NOT NULL
);
GO

-- Creating table 'AspNetUsers'
CREATE TABLE [dbo].[AspNetUsers] (
    [Id] nvarchar(128)  NOT NULL,
    [NameIdentifier] nvarchar(max)  NULL,
    [Email] nvarchar(256)  NULL,
    [EmailConfirmed] bit  NOT NULL,
    [PasswordHash] nvarchar(max)  NULL,
    [SecurityStamp] nvarchar(max)  NULL,
    [PhoneNumber] nvarchar(max)  NULL,
    [PhoneNumberConfirmed] bit  NOT NULL,
    [TwoFactorEnabled] bit  NOT NULL,
    [LockoutEndDateUtc] datetime  NULL,
    [LockoutEnabled] bit  NOT NULL,
    [AccessFailedCount] int  NOT NULL,
    [UserName] nvarchar(256)  NOT NULL
);
GO

-- Creating table 'Banner'
CREATE TABLE [dbo].[Banner] (
    [title] nvarchar(50)  NOT NULL,
    [bodyContent] nvarchar(max)  NULL,
    [picture] nvarchar(100)  NULL,
    [createTime] datetime  NULL,
    [publishTime] datetime  NULL,
    [seq] int  NOT NULL,
    [status] bit  NULL,
    [hitCount] int  NULL,
    [viewCount] int  NULL,
    [publishRange] nvarchar(50)  NULL,
    [url] nvarchar(100)  NULL,
    [publishStatus] nvarchar(10)  NULL,
    [brand] nvarchar(20)  NULL,
    [contentType] nvarchar(20)  NULL
);
GO

-- Creating table 'Members'
CREATE TABLE [dbo].[Members] (
    [seq] bigint IDENTITY(1,1) NOT NULL,
    [name] nvarchar(50)  NULL,
    [email] nvarchar(50)  NULL,
    [password] nvarchar(50)  NULL,
    [createTime] datetime  NULL,
    [updateTime] datetime  NULL,
    [mobile] nvarchar(10)  NULL,
    [title] nvarchar(50)  NULL,
    [birthday] nvarchar(20)  NULL,
    [address] nvarchar(100)  NULL,
    [interestedCar] nvarchar(50)  NULL,
    [isMailVerify] bit  NULL,
    [isMobileVerify] bit  NULL,
    [id] nvarchar(10)  NULL,
    [needToChangeFirst] bit  NULL,
    [status] bit  NULL,
    [brand] nvarchar(10)  NULL,
    [area] nvarchar(10)  NULL
);
GO

-- Creating table 'Roles'
CREATE TABLE [dbo].[Roles] (
    [seq] int IDENTITY(1,1) NOT NULL,
    [RoleName] nvarchar(50)  NULL,
    [Description] nvarchar(200)  NULL,
    [status] bit  NULL,
    [createTime] datetime  NULL
);
GO

-- Creating table 'UserRoles'
CREATE TABLE [dbo].[UserRoles] (
    [seq] bigint IDENTITY(1,1) NOT NULL,
    [userName] nvarchar(100)  NULL,
    [userRole] int  NULL,
    [createTime] datetime  NULL,
    [updateTime] datetime  NULL
);
GO

-- Creating table 'PasswordHistory'
CREATE TABLE [dbo].[PasswordHistory] (
    [seq] bigint  NOT NULL,
    [userEmail] nvarchar(50)  NULL,
    [oldPassword1] nvarchar(500)  NULL,
    [oldPassword2] nvarchar(500)  NULL,
    [oldPassword3] nvarchar(500)  NULL,
    [LastUpdateTime] datetime  NULL,
    [round] int  NULL
);
GO

-- Creating table 'Menus'
CREATE TABLE [dbo].[Menus] (
    [seq] int IDENTITY(1,1) NOT NULL,
    [menuName] nvarchar(50)  NULL,
    [isRoot] bit  NULL,
    [parent] int  NULL,
    [isLink] bit  NULL,
    [controller] nvarchar(10)  NULL,
    [action] nvarchar(30)  NULL,
    [orderSeq] int  NULL,
    [icon] nvarchar(20)  NULL
);
GO

-- Creating table 'MyFavourites'
CREATE TABLE [dbo].[MyFavourites] (
    [seq] bigint IDENTITY(1,1) NOT NULL,
    [carShop_id] int  NULL,
    [user_id] int  NULL,
    [createTime] datetime  NULL
);
GO

-- Creating table 'RoleMenus'
CREATE TABLE [dbo].[RoleMenus] (
    [seq] int IDENTITY(1,1) NOT NULL,
    [roleId] int  NULL,
    [menuId] int  NULL
);
GO

-- Creating table 'Shops'
CREATE TABLE [dbo].[Shops] (
    [seq] bigint IDENTITY(1,1) NOT NULL,
    [price] int  NOT NULL,
    [licensePlate] nvarchar(20)  NULL,
    [licensePicture] nvarchar(100)  NULL,
    [milage] int  NOT NULL,
    [yearOfManufacture] int  NOT NULL,
    [carModel] nvarchar(50)  NULL,
    [carType] nvarchar(50)  NULL,
    [driveMode] nvarchar(50)  NULL,
    [transmissionType] nvarchar(50)  NULL,
    [fuelType] nvarchar(50)  NULL,
    [displacement] nvarchar(50)  NULL,
    [horsepower] nvarchar(50)  NULL,
    [outerColor] nvarchar(50)  NULL,
    [innerColor] nvarchar(50)  NULL,
    [feature] nvarchar(500)  NULL,
    [dealer] nvarchar(50)  NULL,
    [stronghold] nvarchar(50)  NULL,
    [salesRep] nvarchar(50)  NOT NULL,
    [carCondition1] nvarchar(200)  NULL,
    [carCondition2] nvarchar(200)  NULL,
    [carCondition3] nvarchar(200)  NULL,
    [carCondition4] nvarchar(200)  NULL,
    [otherCondition1] nvarchar(200)  NULL,
    [otherCondition2] nvarchar(200)  NULL,
    [contact] nvarchar(50)  NULL,
    [createDate] nvarchar(50)  NULL,
    [creator] nvarchar(50)  NULL,
    [status] nvarchar(10)  NULL,
    [ListingDate] datetime  NULL,
    [description] nvarchar(500)  NULL,
    [inspectionTable] nvarchar(200)  NULL,
    [brand] nvarchar(10)  NULL,
    [createTime] datetime  NULL,
    [updateTime] datetime  NULL,
    [ShopNo] nvarchar(50)  NULL,
    [area] nvarchar(10)  NULL,
    [outEquip] nvarchar(500)  NULL,
    [innerEquip] nvarchar(100)  NULL,
    [member] nvarchar(10)  NULL,
    [consultant] nvarchar(10)  NULL,
    [process] nvarchar(50)  NULL,
    [cashStatus] nvarchar(10)  NULL,
    [finalPrice] int  NOT NULL,
    [contract] nvarchar(100)  NULL,
    [action] nvarchar(10)  NULL,
    [sellingPrice] int  NOT NULL,
    [sellTime] datetime  NULL,
    [NoticeCount] int  NOT NULL,
    [carCondition5] nvarchar(200)  NULL,
    [carCondition6] nvarchar(200)  NULL,
    [carCondition7] nvarchar(200)  NULL,
    [carCondition8] nvarchar(200)  NULL,
    [carCondition9] nvarchar(200)  NULL,
    [carCondition10] nvarchar(200)  NULL,
    [formalShopNo] nvarchar(50)  NULL,
    [isHit] nvarchar(1)  NULL,
    [isFrame] nvarchar(1)  NULL,
    [hitOrder] int  NULL,
    [monthOfManufacture] int  NOT NULL
);
GO

-- Creating table 'Subscriptions'
CREATE TABLE [dbo].[Subscriptions] (
    [seq] bigint IDENTITY(1,1) NOT NULL,
    [carShop_id] int  NULL,
    [user_id] int  NULL,
    [createTime] datetime  NULL,
    [driveMode] nvarchar(100)  NULL,
    [carType] nvarchar(100)  NULL,
    [carModel] nvarchar(100)  NULL,
    [outerColor] nvarchar(100)  NULL,
    [area] nvarchar(100)  NULL,
    [dealer] nvarchar(100)  NULL,
    [price] int  NOT NULL,
    [milage] int  NOT NULL,
    [yearOfManufacture] int  NOT NULL,
    [brand] nvarchar(10)  NULL,
    [priceStr] nvarchar(20)  NULL,
    [milageStr] nvarchar(20)  NULL,
    [yearOfManufactureStr] nvarchar(20)  NULL,
    [email] nvarchar(50)  NULL
);
GO

-- Creating table 'News'
CREATE TABLE [dbo].[News] (
    [seq] bigint IDENTITY(1,1) NOT NULL,
    [title] nvarchar(100)  NULL,
    [bodyContent] nvarchar(max)  NULL,
    [picturePath] nvarchar(100)  NULL,
    [createTime] datetime  NULL,
    [status] nvarchar(10)  NULL,
    [publishRange] nvarchar(50)  NULL,
    [brand] nvarchar(20)  NULL,
    [showBody] nvarchar(200)  NULL
);
GO

-- Creating table 'VisitOrders'
CREATE TABLE [dbo].[VisitOrders] (
    [seq] bigint IDENTITY(1,1) NOT NULL,
    [visitCarType] nvarchar(20)  NULL,
    [visitCarYear] nvarchar(10)  NULL,
    [dealerName] nvarchar(50)  NULL,
    [dealerAddress] nvarchar(50)  NULL,
    [dealerTel] nvarchar(20)  NULL,
    [name] nvarchar(20)  NULL,
    [mobile] nvarchar(20)  NULL,
    [email] nvarchar(50)  NULL,
    [title] nvarchar(20)  NULL,
    [id] nvarchar(10)  NULL,
    [birthday] nvarchar(20)  NULL,
    [address] nvarchar(100)  NULL,
    [salesRep] nvarchar(50)  NULL,
    [others] nvarchar(500)  NULL,
    [period] nvarchar(50)  NULL,
    [isConfirmed] bit  NULL,
    [verifyCode] nvarchar(10)  NULL,
    [verifyCodeValidTime] datetime  NULL,
    [createTime] datetime  NULL,
    [assignedConsultant] nvarchar(50)  NULL,
    [consultant] nvarchar(50)  NULL,
    [status] nvarchar(10)  NULL,
    [shopNo] nvarchar(50)  NULL,
    [visitTime] datetime  NOT NULL,
    [brand] nvarchar(10)  NULL,
    [updateTime] datetime  NULL,
    [loseReason] nvarchar(20)  NULL,
    [loseReason2] nvarchar(20)  NULL,
    [action] nvarchar(20)  NULL,
    [formalNo] nvarchar(20)  NULL,
    [isClose] nvarchar(1)  NULL,
    [visitCarModel] nvarchar(20)  NULL
);
GO

-- Creating table 'Profiles'
CREATE TABLE [dbo].[Profiles] (
    [Id] nvarchar(128)  NOT NULL,
    [Company] nvarchar(50)  NULL,
    [Department] nvarchar(20)  NULL,
    [Mobile] nvarchar(10)  NULL,
    [status] bit  NULL,
    [createTime] datetime  NULL,
    [Name] nvarchar(50)  NULL,
    [brand] nvarchar(10)  NULL,
    [LastLoginTime] datetime  NULL
);
GO

-- Creating table 'Dealers'
CREATE TABLE [dbo].[Dealers] (
    [seq] bigint IDENTITY(1,1) NOT NULL,
    [area] nvarchar(10)  NULL,
    [dealerName] nvarchar(20)  NULL,
    [businessOffice] nvarchar(50)  NULL,
    [telAreaCode] nvarchar(10)  NULL,
    [tel] nvarchar(20)  NULL,
    [address] nvarchar(50)  NULL,
    [gmap] nvarchar(1000)  NULL,
    [businessStartHourDay] nvarchar(10)  NULL,
    [businessEndHourDay] nvarchar(10)  NULL,
    [businessStartHourNight] nvarchar(10)  NULL,
    [businessEndHourNight] nvarchar(10)  NULL,
    [createTime] datetime  NULL,
    [type] nvarchar(10)  NULL,
    [email] nvarchar(50)  NULL,
    [brand] nvarchar(10)  NULL,
    [businessOffice2] nvarchar(50)  NULL,
    [picture] nvarchar(200)  NULL,
    [busDay] nvarchar(50)  NULL,
    [dealerCode] nvarchar(20)  NULL
);
GO

-- Creating table 'DealerPersons'
CREATE TABLE [dbo].[DealerPersons] (
    [seq] bigint IDENTITY(1,1) NOT NULL,
    [name] nvarchar(20)  NULL,
    [mobile] nvarchar(10)  NULL,
    [email] nvarchar(50)  NULL,
    [dealer] nvarchar(20)  NULL,
    [businessOffice] nvarchar(50)  NULL,
    [createTime] datetime  NULL,
    [area] nvarchar(50)  NULL,
    [brand] nvarchar(20)  NULL
);
GO

-- Creating table 'OTPs'
CREATE TABLE [dbo].[OTPs] (
    [seq] bigint IDENTITY(1,1) NOT NULL,
    [OTP] nvarchar(10)  NULL,
    [OTPSendTime] datetime  NULL,
    [mobile] nvarchar(10)  NULL
);
GO

-- Creating table 'Log'
CREATE TABLE [dbo].[Log] (
    [seq] bigint IDENTITY(1,1) NOT NULL,
    [action] nvarchar(50)  NULL,
    [request] nvarchar(max)  NULL,
    [response] nvarchar(max)  NULL,
    [createTime] datetime  NULL,
    [eventLevel] nvarchar(50)  NULL,
    [creator] nvarchar(50)  NULL
);
GO

-- Creating table 'Sells'
CREATE TABLE [dbo].[Sells] (
    [seq] bigint IDENTITY(1,1) NOT NULL,
    [brand] nvarchar(10)  NULL,
    [licensePlate] nvarchar(20)  NULL,
    [licensePicture] nvarchar(200)  NULL,
    [milage] int  NOT NULL,
    [yearOfManufacture] int  NOT NULL,
    [carModel] nvarchar(50)  NULL,
    [displacement] nvarchar(50)  NULL,
    [area] nvarchar(20)  NULL,
    [owner] nvarchar(40)  NULL,
    [title] nvarchar(20)  NULL,
    [email] nvarchar(50)  NULL,
    [mobile] nvarchar(10)  NULL,
    [dealer] nvarchar(50)  NULL,
    [salesRep] nvarchar(50)  NOT NULL,
    [carCondition1] nvarchar(200)  NULL,
    [carCondition2] nvarchar(200)  NULL,
    [carCondition3] nvarchar(200)  NULL,
    [carCondition4] nvarchar(200)  NULL,
    [creator] nvarchar(50)  NULL,
    [createTime] datetime  NULL,
    [contactPeriod] nvarchar(50)  NULL,
    [yearMonthOfManufacture] nvarchar(20)  NULL,
    [updateTime] datetime  NULL,
    [sellNo] nvarchar(50)  NULL,
    [price] int  NOT NULL,
    [status] nvarchar(10)  NULL,
    [consultant] nvarchar(50)  NULL,
    [contactTime] nvarchar(50)  NULL,
    [process] nvarchar(20)  NULL,
    [note] nvarchar(50)  NULL,
    [formalSellNo] nvarchar(50)  NULL,
    [liveArea] nvarchar(10)  NULL,
    [address] nvarchar(50)  NULL,
    [isBuyNewCar] nvarchar(20)  NULL,
    [loseReason] nvarchar(20)  NULL,
    [needConsult] bit  NOT NULL,
    [stronghold] nvarchar(20)  NULL,
    [birthday] nvarchar(20)  NULL,
    [action] nvarchar(10)  NULL,
    [carBrand] nvarchar(20)  NULL,
    [otherBrand] nvarchar(20)  NULL
);
GO

-- Creating table 'Banners'
CREATE TABLE [dbo].[Banners] (
    [seq] bigint IDENTITY(1,1) NOT NULL,
    [title] nvarchar(50)  NULL,
    [bodyContent] nvarchar(max)  NULL,
    [createTime] datetime  NULL,
    [status] nvarchar(10)  NULL,
    [hitCount] int  NULL,
    [viewCount] int  NULL,
    [publishRange] nvarchar(50)  NULL,
    [url] nvarchar(100)  NULL,
    [brand] nvarchar(20)  NULL,
    [contentType] nvarchar(20)  NULL,
    [picture] nvarchar(200)  NULL,
    [action] nvarchar(10)  NULL,
    [subTitle] nvarchar(50)  NULL,
    [mobilePicture] nvarchar(200)  NULL,
    [banner_name] nvarchar(100)  NULL,
    [banner_sort] int  NULL
);
GO

-- Creating table 'NoticeRecords'
CREATE TABLE [dbo].[NoticeRecords] (
    [seq] bigint IDENTITY(1,1) NOT NULL,
    [shopSeq] bigint  NULL,
    [noticeTime] datetime  NOT NULL,
    [member] nvarchar(50)  NULL,
    [shopNo] nvarchar(50)  NULL,
    [brand] nvarchar(10)  NULL
);
GO

-- Creating table 'Prepaid'
CREATE TABLE [dbo].[Prepaid] (
    [seq] bigint IDENTITY(1,1) NOT NULL,
    [prepaidNo] nvarchar(50)  NULL,
    [shopNo] nvarchar(20)  NULL,
    [user_id] nvarchar(10)  NULL,
    [name] nvarchar(20)  NULL,
    [mobile] nvarchar(10)  NULL,
    [email] nvarchar(50)  NULL,
    [title] nvarchar(10)  NULL,
    [id] nvarchar(10)  NULL,
    [birthday] nvarchar(20)  NULL,
    [address] nvarchar(50)  NULL,
    [contactPeriod] nvarchar(50)  NULL,
    [others] nvarchar(100)  NULL,
    [createTime] datetime  NULL,
    [paidStatus] nvarchar(10)  NULL,
    [contactStatus] nvarchar(10)  NULL,
    [sales] nvarchar(10)  NULL,
    [note] nvarchar(200)  NULL,
    [contactTime] datetime  NULL,
    [serveSales] nvarchar(10)  NULL,
    [brand] nvarchar(10)  NULL,
    [updateTime] datetime  NULL,
    [returnReason] nvarchar(20)  NULL,
    [action] nvarchar(20)  NULL,
    [formalNo] nvarchar(20)  NULL,
    [isClose] nvarchar(1)  NULL,
    [status] nvarchar(20)  NULL
);
GO

-- Creating table 'SellsHistory'
CREATE TABLE [dbo].[SellsHistory] (
    [seq] bigint IDENTITY(1,1) NOT NULL,
    [sellNo] nvarchar(50)  NULL,
    [recordDate] datetime  NULL,
    [status] nvarchar(10)  NULL,
    [consultant] nvarchar(50)  NULL,
    [salesRep] nvarchar(50)  NULL,
    [admin] nchar(50)  NULL,
    [note] nvarchar(100)  NULL
);
GO

-- Creating table 'BuyHistory'
CREATE TABLE [dbo].[BuyHistory] (
    [seq] bigint IDENTITY(1,1) NOT NULL,
    [type] nvarchar(5)  NULL,
    [shopNo] nvarchar(50)  NULL,
    [recordDate] datetime  NULL,
    [status] nvarchar(10)  NULL,
    [consultant] nvarchar(50)  NULL,
    [salesRep] nvarchar(50)  NULL,
    [admin] nchar(50)  NULL,
    [note] nvarchar(100)  NULL
);
GO

-- Creating table 'CarCompareHistory'
CREATE TABLE [dbo].[CarCompareHistory] (
    [seq] bigint  NOT NULL,
    [user_id] int  NULL,
    [carCompare] nvarchar(max)  NULL,
    [createTime] datetime  NULL,
    [brand] nvarchar(10)  NULL
);
GO

-- Creating table 'ResetPwLinks'
CREATE TABLE [dbo].[ResetPwLinks] (
    [seq] bigint IDENTITY(1,1) NOT NULL,
    [link] nvarchar(1000)  NULL,
    [createTime] datetime  NULL
);
GO

-- Creating table 'Code'
CREATE TABLE [dbo].[Code] (
    [Code_sn] int  NOT NULL,
    [Class_sn] int  NOT NULL,
    [Code_name] nvarchar(max)  NOT NULL,
    [Note1] nvarchar(max)  NULL,
    [Note2] nvarchar(max)  NULL,
    [Note3] nvarchar(max)  NULL,
    [Note4] nvarchar(max)  NULL,
    [Note5] nvarchar(max)  NULL,
    [Refer_sn] int  NOT NULL,
    [Code_sort] int  NOT NULL,
    [Code_valid] bit  NOT NULL,
    [Note_int1] int  NOT NULL,
    [Note_int2] int  NOT NULL,
    [Note_int3] int  NOT NULL,
    [Note_int4] int  NOT NULL,
    [Note_int5] int  NOT NULL,
    [Note_int6] int  NOT NULL,
    [Note_int7] int  NOT NULL,
    [Note_int8] int  NOT NULL,
    [Note_int9] int  NOT NULL,
    [Note_int10] int  NOT NULL
);
GO

-- Creating table 'Banner1Set'
CREATE TABLE [dbo].[Banner1Set] (
    [seq] bigint IDENTITY(1,1) NOT NULL,
    [title] nvarchar(50)  NOT NULL,
    [bodyContent] nvarchar(max)  NULL,
    [picture] nvarchar(100)  NULL,
    [createTime] datetime  NULL,
    [publishTime] datetime  NULL,
    [status] bit  NULL,
    [hitCount] int  NULL,
    [viewCount] int  NULL,
    [publishRange] nvarchar(50)  NULL,
    [url] nvarchar(100)  NULL,
    [publishStatus] nvarchar(10)  NULL,
    [brand] nvarchar(20)  NULL,
    [contentType] nvarchar(20)  NULL
);
GO

-- Creating table 'Members1Set'
CREATE TABLE [dbo].[Members1Set] (
    [seq] bigint IDENTITY(1,1) NOT NULL,
    [name] nvarchar(50)  NULL,
    [id] nvarchar(10)  NULL,
    [mobile] nvarchar(10)  NULL,
    [email] nvarchar(50)  NULL,
    [title] nvarchar(50)  NULL,
    [birthday] nvarchar(20)  NULL,
    [address] nvarchar(100)  NULL,
    [interestedCar] nvarchar(50)  NULL,
    [isMailVerify] bit  NULL,
    [isMobileVerify] bit  NULL,
    [password] nvarchar(50)  NULL,
    [createTime] datetime  NULL,
    [updateTime] datetime  NULL,
    [needToChangeFirst] bit  NULL,
    [status] bit  NULL,
    [brand] nvarchar(10)  NULL,
    [area] nvarchar(10)  NULL
);
GO

-- Creating table 'OTPs1Set'
CREATE TABLE [dbo].[OTPs1Set] (
    [seq] bigint IDENTITY(1,1) NOT NULL,
    [OTP] nvarchar(10)  NULL,
    [OTPSendTime] datetime  NULL,
    [mobile] nvarchar(10)  NULL
);
GO

-- Creating table 'AspNetUserRoles'
CREATE TABLE [dbo].[AspNetUserRoles] (
    [AspNetRoles_Id] nvarchar(128)  NOT NULL,
    [AspNetUsers_Id] nvarchar(128)  NOT NULL
);
GO

-- Creating table 'AspNetUserRoles1'
CREATE TABLE [dbo].[AspNetUserRoles1] (
    [AspNetUserRoles1_AspNetUsers_Id] nvarchar(128)  NOT NULL,
    [AspNetUserRoles1_AspNetRoles_Id] nvarchar(128)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'AspNetRoles'
ALTER TABLE [dbo].[AspNetRoles]
ADD CONSTRAINT [PK_AspNetRoles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AspNetUserClaims'
ALTER TABLE [dbo].[AspNetUserClaims]
ADD CONSTRAINT [PK_AspNetUserClaims]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [LoginProvider], [ProviderKey], [UserId] in table 'AspNetUserLogins'
ALTER TABLE [dbo].[AspNetUserLogins]
ADD CONSTRAINT [PK_AspNetUserLogins]
    PRIMARY KEY CLUSTERED ([LoginProvider], [ProviderKey], [UserId] ASC);
GO

-- Creating primary key on [Id] in table 'AspNetUsers'
ALTER TABLE [dbo].[AspNetUsers]
ADD CONSTRAINT [PK_AspNetUsers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [seq] in table 'Banner'
ALTER TABLE [dbo].[Banner]
ADD CONSTRAINT [PK_Banner]
    PRIMARY KEY CLUSTERED ([seq] ASC);
GO

-- Creating primary key on [seq] in table 'Members'
ALTER TABLE [dbo].[Members]
ADD CONSTRAINT [PK_Members]
    PRIMARY KEY CLUSTERED ([seq] ASC);
GO

-- Creating primary key on [seq] in table 'Roles'
ALTER TABLE [dbo].[Roles]
ADD CONSTRAINT [PK_Roles]
    PRIMARY KEY CLUSTERED ([seq] ASC);
GO

-- Creating primary key on [seq] in table 'UserRoles'
ALTER TABLE [dbo].[UserRoles]
ADD CONSTRAINT [PK_UserRoles]
    PRIMARY KEY CLUSTERED ([seq] ASC);
GO

-- Creating primary key on [seq] in table 'PasswordHistory'
ALTER TABLE [dbo].[PasswordHistory]
ADD CONSTRAINT [PK_PasswordHistory]
    PRIMARY KEY CLUSTERED ([seq] ASC);
GO

-- Creating primary key on [seq] in table 'Menus'
ALTER TABLE [dbo].[Menus]
ADD CONSTRAINT [PK_Menus]
    PRIMARY KEY CLUSTERED ([seq] ASC);
GO

-- Creating primary key on [seq] in table 'MyFavourites'
ALTER TABLE [dbo].[MyFavourites]
ADD CONSTRAINT [PK_MyFavourites]
    PRIMARY KEY CLUSTERED ([seq] ASC);
GO

-- Creating primary key on [seq] in table 'RoleMenus'
ALTER TABLE [dbo].[RoleMenus]
ADD CONSTRAINT [PK_RoleMenus]
    PRIMARY KEY CLUSTERED ([seq] ASC);
GO

-- Creating primary key on [seq] in table 'Shops'
ALTER TABLE [dbo].[Shops]
ADD CONSTRAINT [PK_Shops]
    PRIMARY KEY CLUSTERED ([seq] ASC);
GO

-- Creating primary key on [seq] in table 'Subscriptions'
ALTER TABLE [dbo].[Subscriptions]
ADD CONSTRAINT [PK_Subscriptions]
    PRIMARY KEY CLUSTERED ([seq] ASC);
GO

-- Creating primary key on [seq] in table 'News'
ALTER TABLE [dbo].[News]
ADD CONSTRAINT [PK_News]
    PRIMARY KEY CLUSTERED ([seq] ASC);
GO

-- Creating primary key on [seq] in table 'VisitOrders'
ALTER TABLE [dbo].[VisitOrders]
ADD CONSTRAINT [PK_VisitOrders]
    PRIMARY KEY CLUSTERED ([seq] ASC);
GO

-- Creating primary key on [Id] in table 'Profiles'
ALTER TABLE [dbo].[Profiles]
ADD CONSTRAINT [PK_Profiles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [seq] in table 'Dealers'
ALTER TABLE [dbo].[Dealers]
ADD CONSTRAINT [PK_Dealers]
    PRIMARY KEY CLUSTERED ([seq] ASC);
GO

-- Creating primary key on [seq] in table 'DealerPersons'
ALTER TABLE [dbo].[DealerPersons]
ADD CONSTRAINT [PK_DealerPersons]
    PRIMARY KEY CLUSTERED ([seq] ASC);
GO

-- Creating primary key on [seq] in table 'OTPs'
ALTER TABLE [dbo].[OTPs]
ADD CONSTRAINT [PK_OTPs]
    PRIMARY KEY CLUSTERED ([seq] ASC);
GO

-- Creating primary key on [seq] in table 'Log'
ALTER TABLE [dbo].[Log]
ADD CONSTRAINT [PK_Log]
    PRIMARY KEY CLUSTERED ([seq] ASC);
GO

-- Creating primary key on [seq] in table 'Sells'
ALTER TABLE [dbo].[Sells]
ADD CONSTRAINT [PK_Sells]
    PRIMARY KEY CLUSTERED ([seq] ASC);
GO

-- Creating primary key on [seq] in table 'Banners'
ALTER TABLE [dbo].[Banners]
ADD CONSTRAINT [PK_Banners]
    PRIMARY KEY CLUSTERED ([seq] ASC);
GO

-- Creating primary key on [seq] in table 'NoticeRecords'
ALTER TABLE [dbo].[NoticeRecords]
ADD CONSTRAINT [PK_NoticeRecords]
    PRIMARY KEY CLUSTERED ([seq] ASC);
GO

-- Creating primary key on [seq] in table 'Prepaid'
ALTER TABLE [dbo].[Prepaid]
ADD CONSTRAINT [PK_Prepaid]
    PRIMARY KEY CLUSTERED ([seq] ASC);
GO

-- Creating primary key on [seq] in table 'SellsHistory'
ALTER TABLE [dbo].[SellsHistory]
ADD CONSTRAINT [PK_SellsHistory]
    PRIMARY KEY CLUSTERED ([seq] ASC);
GO

-- Creating primary key on [seq] in table 'BuyHistory'
ALTER TABLE [dbo].[BuyHistory]
ADD CONSTRAINT [PK_BuyHistory]
    PRIMARY KEY CLUSTERED ([seq] ASC);
GO

-- Creating primary key on [seq] in table 'CarCompareHistory'
ALTER TABLE [dbo].[CarCompareHistory]
ADD CONSTRAINT [PK_CarCompareHistory]
    PRIMARY KEY CLUSTERED ([seq] ASC);
GO

-- Creating primary key on [seq] in table 'ResetPwLinks'
ALTER TABLE [dbo].[ResetPwLinks]
ADD CONSTRAINT [PK_ResetPwLinks]
    PRIMARY KEY CLUSTERED ([seq] ASC);
GO

-- Creating primary key on [Code_sn] in table 'Code'
ALTER TABLE [dbo].[Code]
ADD CONSTRAINT [PK_Code]
    PRIMARY KEY CLUSTERED ([Code_sn] ASC);
GO

-- Creating primary key on [seq] in table 'Banner1Set'
ALTER TABLE [dbo].[Banner1Set]
ADD CONSTRAINT [PK_Banner1Set]
    PRIMARY KEY CLUSTERED ([seq] ASC);
GO

-- Creating primary key on [seq] in table 'Members1Set'
ALTER TABLE [dbo].[Members1Set]
ADD CONSTRAINT [PK_Members1Set]
    PRIMARY KEY CLUSTERED ([seq] ASC);
GO

-- Creating primary key on [seq] in table 'OTPs1Set'
ALTER TABLE [dbo].[OTPs1Set]
ADD CONSTRAINT [PK_OTPs1Set]
    PRIMARY KEY CLUSTERED ([seq] ASC);
GO

-- Creating primary key on [AspNetRoles_Id], [AspNetUsers_Id] in table 'AspNetUserRoles'
ALTER TABLE [dbo].[AspNetUserRoles]
ADD CONSTRAINT [PK_AspNetUserRoles]
    PRIMARY KEY CLUSTERED ([AspNetRoles_Id], [AspNetUsers_Id] ASC);
GO

-- Creating primary key on [AspNetUserRoles1_AspNetUsers_Id], [AspNetUserRoles1_AspNetRoles_Id] in table 'AspNetUserRoles1'
ALTER TABLE [dbo].[AspNetUserRoles1]
ADD CONSTRAINT [PK_AspNetUserRoles1]
    PRIMARY KEY CLUSTERED ([AspNetUserRoles1_AspNetUsers_Id], [AspNetUserRoles1_AspNetRoles_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [UserId] in table 'AspNetUserClaims'
ALTER TABLE [dbo].[AspNetUserClaims]
ADD CONSTRAINT [FK_dbo_AspNetUserClaims_dbo_AspNetUsers_UserId]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_dbo_AspNetUserClaims_dbo_AspNetUsers_UserId'
CREATE INDEX [IX_FK_dbo_AspNetUserClaims_dbo_AspNetUsers_UserId]
ON [dbo].[AspNetUserClaims]
    ([UserId]);
GO

-- Creating foreign key on [UserId] in table 'AspNetUserLogins'
ALTER TABLE [dbo].[AspNetUserLogins]
ADD CONSTRAINT [FK_dbo_AspNetUserLogins_dbo_AspNetUsers_UserId]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_dbo_AspNetUserLogins_dbo_AspNetUsers_UserId'
CREATE INDEX [IX_FK_dbo_AspNetUserLogins_dbo_AspNetUsers_UserId]
ON [dbo].[AspNetUserLogins]
    ([UserId]);
GO

-- Creating foreign key on [AspNetRoles_Id] in table 'AspNetUserRoles'
ALTER TABLE [dbo].[AspNetUserRoles]
ADD CONSTRAINT [FK_AspNetUserRoles_AspNetRoles]
    FOREIGN KEY ([AspNetRoles_Id])
    REFERENCES [dbo].[AspNetRoles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [AspNetUsers_Id] in table 'AspNetUserRoles'
ALTER TABLE [dbo].[AspNetUserRoles]
ADD CONSTRAINT [FK_AspNetUserRoles_AspNetUsers]
    FOREIGN KEY ([AspNetUsers_Id])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AspNetUserRoles_AspNetUsers'
CREATE INDEX [IX_FK_AspNetUserRoles_AspNetUsers]
ON [dbo].[AspNetUserRoles]
    ([AspNetUsers_Id]);
GO

-- Creating foreign key on [AspNetUserRoles1_AspNetUsers_Id] in table 'AspNetUserRoles1'
ALTER TABLE [dbo].[AspNetUserRoles1]
ADD CONSTRAINT [FK_AspNetUserRoles1_AspNetRoles]
    FOREIGN KEY ([AspNetUserRoles1_AspNetUsers_Id])
    REFERENCES [dbo].[AspNetRoles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [AspNetUserRoles1_AspNetRoles_Id] in table 'AspNetUserRoles1'
ALTER TABLE [dbo].[AspNetUserRoles1]
ADD CONSTRAINT [FK_AspNetUserRoles1_AspNetUsers]
    FOREIGN KEY ([AspNetUserRoles1_AspNetRoles_Id])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AspNetUserRoles1_AspNetUsers'
CREATE INDEX [IX_FK_AspNetUserRoles1_AspNetUsers]
ON [dbo].[AspNetUserRoles1]
    ([AspNetUserRoles1_AspNetRoles_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------