using System.IO;
using Microsoft.Extensions.ML;
using Sentiment.Model;

namespace Sentiment
{
    public class PredictionServices : IPredictionServices
    {
        private readonly PredictionEnginePool<SentimentData, SentimentPrediction> _predictionEnginePool;

        public PredictionServices(PredictionEnginePool<SentimentData, SentimentPrediction> predictionEnginePool)
        {
            _predictionEnginePool = predictionEnginePool;
        }

        public bool Predict(string text)
        {
            var input = new SentimentData
            {
                Data = text
            };

            var output = _predictionEnginePool.Predict(modelName: "SentimentAnalysisModel", example: input);

            return output.Prediction.Equals("1");
        }

        public void WriteTestData(string text, string score)
        {
            var path = "..\\data\\seed.txt";

            using (var stream = File.AppendText(path))
            {
                stream.Write($"{text}\t{score}\n");
            }
        }
    }
}
