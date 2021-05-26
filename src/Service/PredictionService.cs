using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ML;
using Microsoft.ML;
using Sentiment.Model;

namespace Sentiment.Service
{
    public class PredictionService : IPredictionService
    {
        private readonly PredictionEnginePool<SentimentData, 
            SentimentPrediction> _predictionEnginePool;
        private readonly MLContext _mlContext;
        private readonly ILogger<PredictionService> _logger;
        private const string _testData = "..\\data\\seed.txt";

        public PredictionService(
            PredictionEnginePool<SentimentData,
                SentimentPrediction> predictionEnginePool,
            MLContext mlContext,
            ILogger<PredictionService> logger)
        {
            _predictionEnginePool = predictionEnginePool;
            _mlContext = mlContext;
            _logger = logger;
        }

        public string Predict(string text, TrainingModel model)
        {
            var modelName = model.ToString();

            var output = _predictionEnginePool.Predict(modelName, new SentimentData{ Data = text });

            var prediction = output.PredictedLabel ? "😀" : "☹️";

            _logger.LogInformation("Predicted {text} as {prediction} with model {model}", text, prediction, modelName);

            return prediction;
        }

        public void WriteTestData(string text, string score)
        {
            var display = score.Equals("1") ? "😀" : "☹️";

            _logger.LogInformation("Saving {text} as {display}", text, display);

            using var stream = File.AppendText(_testData);
            stream.Write($"{text}\t{score}\n");
        }

        public void Train(TrainingModel model)
        {
            var data = GetUpdatedData();

            _logger.LogInformation("Training with model {model}", model.ToString());

            var trainedModel = model switch
            {
                TrainingModel.Perceptron => TrainPerceptron(data),
                TrainingModel.Regression => TrainRegression(data),
                _ => throw new ArgumentOutOfRangeException(nameof(model), model, null)
            };

            var modelPath = GetAbsolutePath($"..\\model\\ML{model}Model.zip");

            _logger.LogInformation("Model trained. Writing to {path}", modelPath);

            using var stream = new StreamWriter(modelPath, false);

            _mlContext.Model.Save(trainedModel, data.Schema, stream.BaseStream);
        }

        private ITransformer TrainPerceptron(IDataView data)
        {
            var processPipeline = _mlContext.Transforms.Text.FeaturizeText("Features", "Rating");

            var trainer = _mlContext.BinaryClassification.Trainers.AveragedPerceptron("BooleanScore");

            var trainingPipeline = processPipeline.Append(trainer);

            _logger.LogInformation("Training...");

            return trainingPipeline.Fit(data);
        }

        private ITransformer TrainRegression(IDataView data)
        {
            var processPipeline = _mlContext.Transforms.Text.FeaturizeText("Features", "Rating");

            var trainer = _mlContext.BinaryClassification.Trainers.LbfgsLogisticRegression("BooleanScore");

            var trainingPipeline = processPipeline.Append(trainer);

            _logger.LogInformation("Training...");

            return trainingPipeline.Fit(data);
        }

        private IDataView GetUpdatedData()
        {
            var rawData = _mlContext.Data.LoadFromTextFile<SentimentData>(GetAbsolutePath(_testData), hasHeader: true);

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

            return preprocessPipeline.Fit(rawData).Transform(rawData);
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
        Perceptron,
        Regression
    }
}
