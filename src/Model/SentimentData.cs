using Microsoft.ML.Data;

namespace Sentiment.Model
{
    public class SentimentData
    {
        [ColumnName("Rating"), LoadColumn(0)]
        public string Data { get; set; }
    }
}
