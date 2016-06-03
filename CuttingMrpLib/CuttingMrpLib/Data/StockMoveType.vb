Public Enum StockMoveType
    ''' <summary>
    ''' 系统入库
    ''' </summary>
    SystemEntry = 0
    ''' <summary>
    ''' 移库
    ''' </summary>
    Move = 1
    ''' <summary>
    ''' 倒冲减少
    ''' </summary>
    Backflush = 2
    ''' <summary>
    ''' 取消倒冲后的入库
    ''' </summary>
    BackflushCancel = 3
    ''' <summary>
    ''' 手工入库
    ''' </summary>
    ManualEntry = 4
    ''' <summary>
    ''' 上传文件入库
    ''' </summary>
    UploadEntry = 5
    ''' <summary>
    ''' 手工盘点
    ''' </summary>
    InventoryManual = 6
    ''' <summary>
    ''' 系统盘点调整
    ''' </summary>
    InventorySystem = 7
End Enum
