namespace EdiX12Reader.Models
{
    public class EdiDocument
    {
        public string DocumentType { get; set; } = string.Empty;
        public List<EdiSegment> Segments { get; set; } = new List<EdiSegment>();
        public string RawContent { get; set; } = string.Empty;
        public DateTime ParsedDate { get; set; } = DateTime.Now;
        
        // EDI metadata
        public string InterchangeControlNumber { get; set; } = string.Empty;
        public string SenderId { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
        public string FunctionalGroupControlNumber { get; set; } = string.Empty;
        public string TransactionSetControlNumber { get; set; } = string.Empty;
    }

    public class EdiSegment
    {
        public string SegmentId { get; set; } = string.Empty;
        public List<string> Elements { get; set; } = new List<string>();
        public int LineNumber { get; set; }
        
        public override string ToString()
        {
            return $"{SegmentId}*{string.Join("*", Elements)}";
        }
    }
}
