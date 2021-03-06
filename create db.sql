USE [master]
GO
/****** Object:  Database [CuttingMrp]    Script Date: 12/09/2016 14:08:21 ******/
CREATE DATABASE [CuttingMrp] ON  PRIMARY 
( NAME = N'CuttingMrp', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\CuttingMrp.mdf' , SIZE = 741376KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'CuttingMrp_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\CuttingMrp_log.ldf' , SIZE = 3164032KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [CuttingMrp] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [CuttingMrp].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [CuttingMrp] SET ANSI_NULL_DEFAULT OFF
GO
ALTER DATABASE [CuttingMrp] SET ANSI_NULLS OFF
GO
ALTER DATABASE [CuttingMrp] SET ANSI_PADDING OFF
GO
ALTER DATABASE [CuttingMrp] SET ANSI_WARNINGS OFF
GO
ALTER DATABASE [CuttingMrp] SET ARITHABORT OFF
GO
ALTER DATABASE [CuttingMrp] SET AUTO_CLOSE OFF
GO
ALTER DATABASE [CuttingMrp] SET AUTO_CREATE_STATISTICS ON
GO
ALTER DATABASE [CuttingMrp] SET AUTO_SHRINK OFF
GO
ALTER DATABASE [CuttingMrp] SET AUTO_UPDATE_STATISTICS ON
GO
ALTER DATABASE [CuttingMrp] SET CURSOR_CLOSE_ON_COMMIT OFF
GO
ALTER DATABASE [CuttingMrp] SET CURSOR_DEFAULT  GLOBAL
GO
ALTER DATABASE [CuttingMrp] SET CONCAT_NULL_YIELDS_NULL OFF
GO
ALTER DATABASE [CuttingMrp] SET NUMERIC_ROUNDABORT OFF
GO
ALTER DATABASE [CuttingMrp] SET QUOTED_IDENTIFIER OFF
GO
ALTER DATABASE [CuttingMrp] SET RECURSIVE_TRIGGERS OFF
GO
ALTER DATABASE [CuttingMrp] SET  DISABLE_BROKER
GO
ALTER DATABASE [CuttingMrp] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
GO
ALTER DATABASE [CuttingMrp] SET DATE_CORRELATION_OPTIMIZATION OFF
GO
ALTER DATABASE [CuttingMrp] SET TRUSTWORTHY OFF
GO
ALTER DATABASE [CuttingMrp] SET ALLOW_SNAPSHOT_ISOLATION OFF
GO
ALTER DATABASE [CuttingMrp] SET PARAMETERIZATION SIMPLE
GO
ALTER DATABASE [CuttingMrp] SET READ_COMMITTED_SNAPSHOT OFF
GO
ALTER DATABASE [CuttingMrp] SET HONOR_BROKER_PRIORITY OFF
GO
ALTER DATABASE [CuttingMrp] SET  READ_WRITE
GO
ALTER DATABASE [CuttingMrp] SET RECOVERY FULL
GO
ALTER DATABASE [CuttingMrp] SET  MULTI_USER
GO
ALTER DATABASE [CuttingMrp] SET PAGE_VERIFY CHECKSUM
GO
ALTER DATABASE [CuttingMrp] SET DB_CHAINING OFF
GO
EXEC sys.sp_db_vardecimal_storage_format N'CuttingMrp', N'ON'
GO
USE [CuttingMrp]
GO

/****** Object:  Table [dbo].[User]    Script Date: 12/09/2016 14:08:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[User](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[nr] [varchar](200) NOT NULL,
	[name] [varchar](200) NOT NULL,
	[pwd] [varchar](500) NOT NULL,
	[role] [int] NOT NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[StockSumRecord]    Script Date: 12/09/2016 14:08:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[StockSumRecord](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[partNr] [varchar](200) NOT NULL,
	[quantity] [float] NOT NULL,
	[date] [datetime] NOT NULL,
	[rate] [float] NULL,
 CONSTRAINT [PK_StockSumRecord] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[StockMovement]    Script Date: 12/09/2016 14:08:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[StockMovement](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[partNr] [varchar](50) NOT NULL,
	[quantity] [float] NOT NULL,
	[fifo] [date] NOT NULL,
	[moveType] [int] NOT NULL,
	[sourceDoc] [text] NULL,
	[createdAt] [datetime] NULL,
 CONSTRAINT [PK_StockMovement] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[StockBatchMoveRecord]    Script Date: 12/09/2016 14:08:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[StockBatchMoveRecord](
	[id] [varchar](200) NOT NULL,
	[sourceDoc] [varchar](200) NULL,
	[souceDocTime] [datetime] NULL,
	[partNr] [varchar](200) NOT NULL,
	[quantity] [float] NOT NULL,
	[moveType] [int] NOT NULL,
	[createdAt] [datetime] NULL,
 CONSTRAINT [PK_StockBatchMoveRecord] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ProductionUnit]    Script Date: 12/09/2016 14:08:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ProductionUnit](
	[unitId] [varchar](50) NOT NULL,
	[name] [varchar](200) NOT NULL,
 CONSTRAINT [PK_ProductioUnit] PRIMARY KEY CLUSTERED 
(
	[unitId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Part]    Script Date: 12/09/2016 14:08:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Part](
	[partNr] [varchar](200) NOT NULL,
	[partType] [int] NOT NULL,
	[partDesc] [text] NOT NULL,
	[partStatus] [int] NOT NULL,
	[moq] [float] NULL,
	[spq] [float] NULL,
 CONSTRAINT [PK_Part] PRIMARY KEY CLUSTERED 
(
	[partNr] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[EnumType]    Script Date: 12/09/2016 14:08:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EnumType](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[enumType] [varchar](100) NOT NULL,
	[enumKey] [varchar](100) NOT NULL,
	[enumValue] [int] NOT NULL,
	[enumDesc] [text] NOT NULL,
 CONSTRAINT [PK_EnumType] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [IX_EnumType] UNIQUE NONCLUSTERED 
(
	[enumType] ASC,
	[enumKey] ASC,
	[enumValue] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[NumericBuild]    Script Date: 12/09/2016 14:08:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[NumericBuild](
	[idUniq] [int] IDENTITY(1,1) NOT NULL,
	[idType] [varchar](100) NOT NULL,
	[prefix] [varchar](10) NULL,
	[suffix] [varchar](10) NULL,
	[max] [bigint] NOT NULL,
	[min] [int] NOT NULL,
	[currentNumber] [bigint] NOT NULL,
	[description] [varchar](200) NOT NULL,
 CONSTRAINT [PK_NumericBuild] PRIMARY KEY CLUSTERED 
(
	[idUniq] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [IX_NumericBuild] UNIQUE NONCLUSTERED 
(
	[idType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MrpRound]    Script Date: 12/09/2016 14:08:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MrpRound](
	[mrpRound] [varchar](200) NOT NULL,
	[runningStatus] [int] NOT NULL,
	[time] [datetime] NOT NULL,
	[launcher] [varchar](200) NOT NULL,
	[text] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_MrpRound] PRIMARY KEY CLUSTERED 
(
	[mrpRound] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[BackflushRecord]    Script Date: 12/09/2016 14:08:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BackflushRecord](
	[backflushId] [int] IDENTITY(1,1) NOT NULL,
	[partnr] [varchar](200) NOT NULL,
	[fifo] [date] NOT NULL,
	[sourceDoc] [varchar](50) NOT NULL,
	[quantity] [float] NOT NULL,
	[status] [int] NOT NULL,
	[message] [nvarchar](max) NOT NULL,
	[launchTime] [datetime] NOT NULL,
 CONSTRAINT [PK_BackflushRecord] PRIMARY KEY CLUSTERED 
(
	[backflushId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[StatusControl]    Script Date: 12/09/2016 14:08:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[StatusControl](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[type] [varchar](50) NOT NULL,
	[value] [int] NOT NULL,
 CONSTRAINT [PK_StatusControl] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [IX_StatusControl] UNIQUE NONCLUSTERED 
(
	[type] ASC,
	[value] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Requirement]    Script Date: 12/09/2016 14:08:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Requirement](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[partNr] [varchar](200) NOT NULL,
	[orderedDate] [datetime] NULL,
	[requiredDate] [datetime] NULL,
	[quantity] [float] NOT NULL,
	[status] [int] NOT NULL,
	[derivedFrom] [varchar](200) NOT NULL,
	[derivedType] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Requirement] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[BOM]    Script Date: 12/09/2016 14:08:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BOM](
	[id] [varchar](50) NOT NULL,
	[partNr] [varchar](200) NOT NULL,
	[validFrom] [datetime] NOT NULL,
	[validTo] [datetime] NOT NULL,
	[versionId] [varchar](50) NOT NULL,
	[bomDesc] [text] NOT NULL,
 CONSTRAINT [PK_BOM] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [IX_BOM] UNIQUE NONCLUSTERED 
(
	[partNr] ASC,
	[versionId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[BatchOrderTemplate]    Script Date: 12/09/2016 14:08:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BatchOrderTemplate](
	[orderNr] [varchar](50) NOT NULL,
	[partNr] [varchar](200) NOT NULL,
	[batchQuantity] [float] NOT NULL,
	[type] [int] NOT NULL,
	[bundle] [float] NOT NULL,
	[createdAt] [datetime] NOT NULL,
	[updatedAt] [datetime] NOT NULL,
	[operator] [varchar](50) NOT NULL,
	[remark1] [text] NULL,
	[remark2] [text] NULL,
 CONSTRAINT [PK_FixOrderTemplate] PRIMARY KEY CLUSTERED 
(
	[orderNr] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MPS]    Script Date: 12/09/2016 14:08:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MPS](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[partnr] [varchar](200) NOT NULL,
	[orderedDate] [datetime] NOT NULL,
	[requiredDate] [datetime] NOT NULL,
	[quantity] [float] NOT NULL,
	[source] [varchar](200) NOT NULL,
	[sourceDoc] [varchar](200) NULL,
	[status] [int] NOT NULL,
	[unitId] [varchar](50) NULL,
 CONSTRAINT [PK_MPS] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CapacityPlan]    Script Date: 12/09/2016 14:08:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CapacityPlan](
	[planId] [varchar](100) NOT NULL,
	[planDay] [date] NOT NULL,
	[workinghour] [int] NOT NULL,
	[yieldPerHour] [int] NOT NULL,
	[unitId] [varchar](50) NOT NULL,
 CONSTRAINT [PK_CapacityPlan] PRIMARY KEY CLUSTERED 
(
	[planId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ProcessOrder]    Script Date: 12/09/2016 14:08:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ProcessOrder](
	[orderNr] [varchar](50) NOT NULL,
	[sourceDoc] [varchar](2000) NULL,
	[derivedFrom] [varchar](50) NOT NULL,
	[proceeDate] [datetime] NULL,
	[partNr] [varchar](200) NOT NULL,
	[sourceQuantity] [float] NOT NULL,
	[actualQuantity] [float] NOT NULL,
	[completeRate] [float] NOT NULL,
	[status] [int] NOT NULL,
	[requirementId] [int] NULL,
	[batchQuantity] [float] NULL,
	[OrderType] [varchar](100) NULL,
	[createAt] [datetime] NULL,
	[requirementQuantity] [float] NULL,
	[currentStock] [float] NULL,
 CONSTRAINT [PK_ProcessOrder] PRIMARY KEY CLUSTERED 
(
	[orderNr] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Stock]    Script Date: 12/09/2016 14:08:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Stock](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[partNr] [varchar](200) NOT NULL,
	[fifo] [datetime] NOT NULL,
	[quantity] [float] NOT NULL,
	[container] [varchar](200) NOT NULL,
	[wh] [varchar](50) NOT NULL,
	[position] [varchar](50) NOT NULL,
	[source] [varchar](200) NULL,
	[sourceType] [varchar](200) NULL,
 CONSTRAINT [PK_Stock] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UnDoneStock]    Script Date: 12/09/2016 14:08:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UnDoneStock](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[partNr] [varchar](200) NOT NULL,
	[quantity] [float] NOT NULL,
	[kanbanNr] [varchar](200) NOT NULL,
	[sourceType] [int] NOT NULL,
	[state] [int] NOT NULL,
 CONSTRAINT [PK_UnDoneStock] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  View [dbo].[SumOfStock]    Script Date: 12/09/2016 14:08:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[SumOfStock]
AS
SELECT     partNr, SUM(SumOfStock) AS SumOfStock
FROM         (SELECT     NEWID() AS Expr1, SUM(ISNULL(quantity, 0)) AS SumOfStock, partNr
                       FROM          dbo.Stock
                       GROUP BY partNr
                       UNION
                       SELECT     NEWID() AS Expr1, SUM(ISNULL(quantity, 0)) AS SumOfStock, partNr
                       FROM         dbo.UnDoneStock
                       WHERE     (state = 100)
                       GROUP BY partNr) AS derivedtbl_1
GROUP BY partNr
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "derivedtbl_1"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 110
               Right = 201
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SumOfStock'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SumOfStock'
GO
/****** Object:  View [dbo].[ProcessOrderView]    Script Date: 12/09/2016 14:08:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[ProcessOrderView]
AS
SELECT     dbo.ProcessOrder.orderNr, dbo.ProcessOrder.sourceDoc, dbo.ProcessOrder.derivedFrom, dbo.ProcessOrder.partNr, dbo.ProcessOrder.proceeDate, 
                      dbo.ProcessOrder.sourceQuantity, dbo.ProcessOrder.actualQuantity, dbo.ProcessOrder.completeRate, dbo.ProcessOrder.status, dbo.ProcessOrder.requirementId, 
                      dbo.ProcessOrder.batchQuantity, dbo.ProcessOrder.OrderType, dbo.Part.partNr AS Expr1, dbo.Part.partType, dbo.Part.partDesc, dbo.Part.partStatus, dbo.Part.moq, 
                      dbo.Part.spq, dbo.ProcessOrder.createAt
FROM         dbo.Part INNER JOIN
                      dbo.ProcessOrder ON dbo.Part.partNr = dbo.ProcessOrder.partNr
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = -96
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Part"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 114
               Right = 189
            End
            DisplayFlags = 280
            TopColumn = 2
         End
         Begin Table = "ProcessOrder"
            Begin Extent = 
               Top = 6
               Left = 227
               Bottom = 193
               Right = 416
            End
            DisplayFlags = 280
            TopColumn = 4
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ProcessOrderView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ProcessOrderView'
GO
/****** Object:  Table [dbo].[OrderDerivation]    Script Date: 12/09/2016 14:08:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[OrderDerivation](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[orderId] [varchar](50) NOT NULL,
	[requirementId] [int] NOT NULL,
	[mrpRound] [varchar](50) NOT NULL,
	[deriveQty] [float] NOT NULL,
 CONSTRAINT [PK_OrderDerivation] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[BomItem]    Script Date: 12/09/2016 14:08:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BomItem](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[componentId] [varchar](200) NOT NULL,
	[validFrom] [datetime] NOT NULL,
	[validTo] [datetime] NOT NULL,
	[hasChind] [tinyint] NOT NULL,
	[uom] [int] NOT NULL,
	[quantity] [float] NOT NULL,
	[bomId] [varchar](50) NOT NULL,
 CONSTRAINT [PK_BomItem] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  View [dbo].[AvgOfCompleteRate]    Script Date: 12/09/2016 14:08:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[AvgOfCompleteRate]
AS
SELECT     partNr, proceeDate, AVG(actualQuantity) AS qty, AVG(completeRate) AS rate
FROM         dbo.ProcessOrder
GROUP BY partNr, proceeDate
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "ProcessOrder"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 176
               Right = 389
            End
            DisplayFlags = 280
            TopColumn = 5
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 1440
         Alias = 900
         Table = 1200
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AvgOfCompleteRate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AvgOfCompleteRate'
GO
/****** Object:  Default [DF_StockMovement_createdAt]    Script Date: 12/09/2016 14:08:22 ******/
ALTER TABLE [dbo].[StockMovement] ADD  CONSTRAINT [DF_StockMovement_createdAt]  DEFAULT (getdate()) FOR [createdAt]
GO
/****** Object:  Default [DF_StockBatchMoveRecord_createdAt]    Script Date: 12/09/2016 14:08:22 ******/
ALTER TABLE [dbo].[StockBatchMoveRecord] ADD  CONSTRAINT [DF_StockBatchMoveRecord_createdAt]  DEFAULT (getdate()) FOR [createdAt]
GO
/****** Object:  Default [DF_ProcessOrder_currentStock]    Script Date: 12/09/2016 14:08:22 ******/
ALTER TABLE [dbo].[ProcessOrder] ADD  CONSTRAINT [DF_ProcessOrder_currentStock]  DEFAULT ((0)) FOR [currentStock]
GO
/****** Object:  ForeignKey [FK_Requirement_Part]    Script Date: 12/09/2016 14:08:22 ******/
ALTER TABLE [dbo].[Requirement]  WITH CHECK ADD  CONSTRAINT [FK_Requirement_Part] FOREIGN KEY([partNr])
REFERENCES [dbo].[Part] ([partNr])
GO
ALTER TABLE [dbo].[Requirement] CHECK CONSTRAINT [FK_Requirement_Part]
GO
/****** Object:  ForeignKey [FK_BOM_Part]    Script Date: 12/09/2016 14:08:22 ******/
ALTER TABLE [dbo].[BOM]  WITH CHECK ADD  CONSTRAINT [FK_BOM_Part] FOREIGN KEY([partNr])
REFERENCES [dbo].[Part] ([partNr])
GO
ALTER TABLE [dbo].[BOM] CHECK CONSTRAINT [FK_BOM_Part]
GO
/****** Object:  ForeignKey [FK_BatchOrderTemplate_Part]    Script Date: 12/09/2016 14:08:22 ******/
ALTER TABLE [dbo].[BatchOrderTemplate]  WITH CHECK ADD  CONSTRAINT [FK_BatchOrderTemplate_Part] FOREIGN KEY([partNr])
REFERENCES [dbo].[Part] ([partNr])
GO
ALTER TABLE [dbo].[BatchOrderTemplate] CHECK CONSTRAINT [FK_BatchOrderTemplate_Part]
GO
/****** Object:  ForeignKey [FK_MPS_MPS]    Script Date: 12/09/2016 14:08:22 ******/
ALTER TABLE [dbo].[MPS]  WITH CHECK ADD  CONSTRAINT [FK_MPS_MPS] FOREIGN KEY([id])
REFERENCES [dbo].[MPS] ([id])
GO
ALTER TABLE [dbo].[MPS] CHECK CONSTRAINT [FK_MPS_MPS]
GO
/****** Object:  ForeignKey [FK_MPS_Part]    Script Date: 12/09/2016 14:08:22 ******/
ALTER TABLE [dbo].[MPS]  WITH CHECK ADD  CONSTRAINT [FK_MPS_Part] FOREIGN KEY([partnr])
REFERENCES [dbo].[Part] ([partNr])
GO
ALTER TABLE [dbo].[MPS] CHECK CONSTRAINT [FK_MPS_Part]
GO
/****** Object:  ForeignKey [FK_MPS_ProductionUnit]    Script Date: 12/09/2016 14:08:22 ******/
ALTER TABLE [dbo].[MPS]  WITH CHECK ADD  CONSTRAINT [FK_MPS_ProductionUnit] FOREIGN KEY([unitId])
REFERENCES [dbo].[ProductionUnit] ([unitId])
GO
ALTER TABLE [dbo].[MPS] CHECK CONSTRAINT [FK_MPS_ProductionUnit]
GO
/****** Object:  ForeignKey [FK_CapacityPlan_ProductionUnit]    Script Date: 12/09/2016 14:08:22 ******/
ALTER TABLE [dbo].[CapacityPlan]  WITH CHECK ADD  CONSTRAINT [FK_CapacityPlan_ProductionUnit] FOREIGN KEY([unitId])
REFERENCES [dbo].[ProductionUnit] ([unitId])
GO
ALTER TABLE [dbo].[CapacityPlan] CHECK CONSTRAINT [FK_CapacityPlan_ProductionUnit]
GO
/****** Object:  ForeignKey [FK_ProcessOrder_Part]    Script Date: 12/09/2016 14:08:22 ******/
ALTER TABLE [dbo].[ProcessOrder]  WITH CHECK ADD  CONSTRAINT [FK_ProcessOrder_Part] FOREIGN KEY([partNr])
REFERENCES [dbo].[Part] ([partNr])
GO
ALTER TABLE [dbo].[ProcessOrder] CHECK CONSTRAINT [FK_ProcessOrder_Part]
GO
/****** Object:  ForeignKey [FK_Stock_Part]    Script Date: 12/09/2016 14:08:22 ******/
ALTER TABLE [dbo].[Stock]  WITH CHECK ADD  CONSTRAINT [FK_Stock_Part] FOREIGN KEY([partNr])
REFERENCES [dbo].[Part] ([partNr])
GO
ALTER TABLE [dbo].[Stock] CHECK CONSTRAINT [FK_Stock_Part]
GO
/****** Object:  ForeignKey [FK_UnDoneStock_Part]    Script Date: 12/09/2016 14:08:22 ******/
ALTER TABLE [dbo].[UnDoneStock]  WITH CHECK ADD  CONSTRAINT [FK_UnDoneStock_Part] FOREIGN KEY([partNr])
REFERENCES [dbo].[Part] ([partNr])
GO
ALTER TABLE [dbo].[UnDoneStock] CHECK CONSTRAINT [FK_UnDoneStock_Part]
GO
/****** Object:  ForeignKey [FK_OrderDerivation_ProcessOrder]    Script Date: 12/09/2016 14:08:23 ******/
ALTER TABLE [dbo].[OrderDerivation]  WITH CHECK ADD  CONSTRAINT [FK_OrderDerivation_ProcessOrder] FOREIGN KEY([orderId])
REFERENCES [dbo].[ProcessOrder] ([orderNr])
GO
ALTER TABLE [dbo].[OrderDerivation] CHECK CONSTRAINT [FK_OrderDerivation_ProcessOrder]
GO
/****** Object:  ForeignKey [FK_OrderDerivation_Requirement]    Script Date: 12/09/2016 14:08:23 ******/
ALTER TABLE [dbo].[OrderDerivation]  WITH CHECK ADD  CONSTRAINT [FK_OrderDerivation_Requirement] FOREIGN KEY([requirementId])
REFERENCES [dbo].[Requirement] ([id])
GO
ALTER TABLE [dbo].[OrderDerivation] CHECK CONSTRAINT [FK_OrderDerivation_Requirement]
GO
/****** Object:  ForeignKey [FK_BomItem_BOM]    Script Date: 12/09/2016 14:08:23 ******/
ALTER TABLE [dbo].[BomItem]  WITH CHECK ADD  CONSTRAINT [FK_BomItem_BOM] FOREIGN KEY([bomId])
REFERENCES [dbo].[BOM] ([id])
GO
ALTER TABLE [dbo].[BomItem] CHECK CONSTRAINT [FK_BomItem_BOM]
GO
