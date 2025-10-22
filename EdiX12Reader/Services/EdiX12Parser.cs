using EdiX12Reader.Models;

namespace EdiX12Reader.Services
{
    public class EdiX12Parser
    {
        private const string SEGMENT_TERMINATOR = "~";
        private const string ELEMENT_SEPARATOR = "*";
        private const string COMPONENT_SEPARATOR = ":";
        
        public EdiDocument ParseEdiContent(string content)
        {
            var document = new EdiDocument
            {
                RawContent = content,
                ParsedDate = DateTime.Now
            };

            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("EDI content cannot be empty");
            }

            // Remove any line breaks and extra whitespace
            content = content.Replace("\r\n", "").Replace("\n", "").Trim();

            // Split by segment terminator
            var segments = content.Split(new[] { SEGMENT_TERMINATOR }, StringSplitOptions.RemoveEmptyEntries);

            int lineNumber = 1;
            foreach (var segmentData in segments)
            {
                if (string.IsNullOrWhiteSpace(segmentData))
                    continue;

                var elements = segmentData.Split(new[] { ELEMENT_SEPARATOR }, StringSplitOptions.None);
                
                if (elements.Length == 0)
                    continue;

                var segment = new EdiSegment
                {
                    SegmentId = elements[0],
                    LineNumber = lineNumber++
                };

                // Add all elements except the segment ID
                for (int i = 1; i < elements.Length; i++)
                {
                    segment.Elements.Add(elements[i]);
                }

                document.Segments.Add(segment);

                // Extract metadata from specific segments
                ExtractMetadata(document, segment);
            }

            // Determine document type
            DetermineDocumentType(document);

            return document;
        }

        private void ExtractMetadata(EdiDocument document, EdiSegment segment)
        {
            switch (segment.SegmentId)
            {
                case "ISA": // Interchange Control Header
                    if (segment.Elements.Count >= 13)
                    {
                        document.SenderId = segment.Elements[5].Trim();
                        document.ReceiverId = segment.Elements[7].Trim();
                        document.InterchangeControlNumber = segment.Elements[12].Trim();
                    }
                    break;

                case "GS": // Functional Group Header
                    if (segment.Elements.Count >= 6)
                    {
                        document.FunctionalGroupControlNumber = segment.Elements[5].Trim();
                    }
                    break;

                case "ST": // Transaction Set Header
                    if (segment.Elements.Count >= 2)
                    {
                        document.DocumentType = segment.Elements[0].Trim();
                        document.TransactionSetControlNumber = segment.Elements[1].Trim();
                    }
                    break;
            }
        }

        private void DetermineDocumentType(EdiDocument document)
        {
            var stSegment = document.Segments.FirstOrDefault(s => s.SegmentId == "ST");
            if (stSegment != null && stSegment.Elements.Count > 0)
            {
                var transactionCode = stSegment.Elements[0];
                document.DocumentType = transactionCode switch
                {
                    "837" => "837 - Healthcare Claim",
                    "835" => "835 - Healthcare Claim Payment/Remittance Advice",
                    _ => $"{transactionCode} - Unknown Transaction Type"
                };
            }
            else
            {
                document.DocumentType = "Unknown";
            }
        }

        public Dictionary<string, string> GetSegmentDescription(string segmentId)
        {
            // Common EDI X12 segment descriptions
            var descriptions = new Dictionary<string, string>
            {
                { "ISA", "Interchange Control Header" },
                { "GS", "Functional Group Header" },
                { "ST", "Transaction Set Header" },
                { "BHT", "Beginning of Hierarchical Transaction" },
                { "NM1", "Individual or Organizational Name" },
                { "N3", "Address Information" },
                { "N4", "Geographic Location" },
                { "REF", "Reference Identification" },
                { "PER", "Administrative Communications Contact" },
                { "CLM", "Claim Information" },
                { "CLP", "Claim Level Payment Information" },
                { "SVC", "Service Payment Information" },
                { "DTP", "Date or Time or Period" },
                { "AMT", "Monetary Amount" },
                { "SE", "Transaction Set Trailer" },
                { "GE", "Functional Group Trailer" },
                { "IEA", "Interchange Control Trailer" }
            };

            return descriptions;
        }
    }
}
