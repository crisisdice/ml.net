using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ML;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Sentiment.Model;

namespace Sentiment
{
    public class PredictionService : IPredictionService
    {
        private readonly PredictionEnginePool<SentimentData, SentimentPrediction> _predictionEnginePool;
        private readonly MLContext _mlContext;
        private readonly ILogger<PredictionService> _logger;

        public PredictionService(PredictionEnginePool<SentimentData, SentimentPrediction> predictionEnginePool, MLContext mlContext, ILogger<PredictionService> logger)
        {
            _predictionEnginePool = predictionEnginePool;
            _mlContext = mlContext;
            _logger = logger;
        }

        public string Predict(string text, TrainingModel model)
        {
            var input = new SentimentData
            {
                Data = text
            };

            var output = _predictionEnginePool.Predict(modelName: model.ToString(), example: input);

            var prediction = output.PredictedLabel ? "😀" : "☹️";

            _logger.LogInformation("Predicted {text} as {prediction} with model {model}", text, prediction, model.ToString());

            return prediction;
        }

        public void WriteTestData(string text, string score)
        {
            const string path = "..\\data\\seed.txt";

            var display = score.Equals("1") ? "😀" : "☹️";

            _logger.LogInformation("Saving {text} as {display}", text, display);

            using var stream = File.AppendText(path);
            stream.Write($"{text}\t{score}\n");
        }

        public void Train(TrainingModel model)
        {
            _logger.LogInformation("Training with model {model}", model.ToString());
            
            var dataPath = GetAbsolutePath("..\\..\\..\\..\\data\\seed.txt");
            

            var dataView = _mlContext.Data.LoadFromTextFile<SentimentData>(dataPath, hasHeader: true);

            var mappingTable = new[]
            {
                new  {Score = "0", BooleanScore = false},
                new  {Score = "1", BooleanScore = true}
            };

            var mapping = _mlContext.Data.LoadFromEnumerable(mappingTable);

            _logger.LogInformation("Mapping new input data");

            var preprocessPipeline = _mlContext.Transforms.Conversion.MapValue("BooleanScore",
                mapping, mapping.Schema["Score"], mapping.Schema[
                    "BooleanScore"], "Score");

            var transformedData = preprocessPipeline.Fit(dataView).Transform(dataView);

            var modelPath = GetAbsolutePath($"..\\..\\..\\..\\model\\ML{model}Model.zip");

            var trainedModel = model switch
            {
                TrainingModel.Perceptron => TrainPipeline(transformedData, model),
                _ => throw new ArgumentOutOfRangeException(nameof(model), model, null)
            };

            _logger.LogInformation("Model trained. Writing to {path}", modelPath);

            using var stream = new StreamWriter(modelPath, false);

            _mlContext.Model.Save(trainedModel, transformedData.Schema, stream.BaseStream);
        }

        private TransformerChain<BinaryPredictionTransformer<LinearBinaryModelParameters>> TrainPipeline(IDataView data, TrainingModel model)
        {
            var processPipeline = _mlContext.Transforms.Text.FeaturizeText("Features", "Rating");

            var trainer = _mlContext.BinaryClassification.Trainers.AveragedPerceptron("BooleanScore");

            var trainingPipeline = processPipeline.Append(trainer);

            _logger.LogInformation("Training...");

            return  trainingPipeline.Fit(data);
        }

        private static string GetAbsolutePath(string relativePath)
        {
            var _dataRoot = new FileInfo(typeof(Program).Assembly.Location);
            var assemblyFolderPath = _dataRoot.Directory?.FullName;

            var fullPath = Path.Combine(assemblyFolderPath, relativePath);

            return fullPath;
        }

    }
    public enum TrainingModel
    {
        Perceptron
    }
}
