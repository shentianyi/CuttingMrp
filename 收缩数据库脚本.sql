
--- 此脚本是用来收缩数据库 CuttingMrp 的
--- 按需请修改所有的CuttingMrp为其它数据库

USE[master]
    GO
    ALTER DATABASE CuttingMrp SET RECOVERY SIMPLE WITH NO_WAIT 
    GO
    ALTER DATABASE CuttingMrp SET RECOVERY SIMPLE   --简单模式
    GO
    USE CuttingMrp 
    GO
    DBCC SHRINKFILE (N'CuttingMrp_Log' , 11, TRUNCATEONLY)
    GO
    
    USE[master]
    GO

    ALTER DATABASE CuttingMrp SET RECOVERY  FULL WITH NO_WAIT

    GO

    ALTER DATABASE CuttingMrp SET RECOVERY FULL  --还原为完全模式

    GO