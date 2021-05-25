using System.IO;
using Microsoft.Extensions.ML;
using Sentiment.Model;

namespace Sentiment
{
    public class PredictionService : IPredictionService
    {
        private readonly PredictionEnginePool<SentimentData, SentimentPrediction> _predictionEnginePool;

        public PredictionService(PredictionEnginePool<SentimentData, SentimentPrediction> predictionEnginePool)
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

            return output.PredictedLabel;
        }

        public void WriteTestData(string text, string score)
        {
            const string path = "..\\data\\seed.txt";

            using var stream = File.AppendText(path);
            stream.Write($"{text}\t{score}\n");
        }

    }
}
