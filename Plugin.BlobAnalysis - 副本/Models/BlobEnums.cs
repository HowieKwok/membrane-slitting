namespace Plugin.BlobAnalysis.Models
{
    public enum DetectRangeMode { Global = 0, LocalRoi = 1 }
    public enum RoiEditRole { Local = 0, Mask = 1 }
    public enum RoiShapeType { Rectangle1 = 0, Rectangle2 = 1, Circle = 2 }
    public enum MorphMode { None = 0, Open = 1, Close = 2, FillUp = 3, OpenClose = 4, CloseOpen = 5 }
    public enum SortFeature { Area = 0, Row = 1, Column = 2 }
    public enum SortDirection { Asc = 0, Desc = 1 }
}

