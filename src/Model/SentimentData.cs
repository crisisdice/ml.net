using Microsoft.ML.Data;

namespace Sentiment.Model
{
    public class SentimentData
    {
        [ColumnName("col0"), LoadColumn(0)]
        public string Data { get; set; }
        [ColumnName("col1"), LoadColumn(1)]
        public string Col1 { get; set; }
    }
}
