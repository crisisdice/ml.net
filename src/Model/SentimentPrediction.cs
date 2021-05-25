using Microsoft.ML.Data;

namespace Sentiment.Model
{
    public class SentimentPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool PredictedLabel { get; set; }
    }
}
