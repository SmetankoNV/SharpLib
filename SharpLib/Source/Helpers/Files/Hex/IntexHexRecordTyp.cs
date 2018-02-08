namespace SharpLib.Source.Helpers.Files.Hex
{
    public enum IntexHexRecordTyp : byte
    {
        Unknow = 0xFF,

        Data = 0x00,

        EndFile = 0x01,

        SegmentAddr = 0x02,

        StartSegmentAddr = 0x03,

        LinearAddr = 0x04,

        StartLinearAddr = 0x05
    }
}