using Microsoft.ML.Data;

namespace Sentiment.Model
{
    public class SentimentPrediction
    {
        [ColumnName("PredictedLabel")]
        public string Prediction { get; set; }
        public float[] Score { get; set; }
    }
}
