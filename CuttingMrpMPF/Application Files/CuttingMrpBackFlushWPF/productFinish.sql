USE [CuttingMrp]
GO

/****** Object:  Table [dbo].[ProductFinish]    Script Date: 12/15/2016 14:15:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[ProductFinish](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[productnr] [varchar](50) NOT NULL,
	[partNr] [varchar](50) NOT NULL,
	[status] [int] NOT NULL,
	[finishTime] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_ProductFinish] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


