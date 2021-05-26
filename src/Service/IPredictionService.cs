namespace Sentiment.Service
{
    public interface IPredictionService
    {
        string Predict(string rating, TrainingModel model);
        void WriteTestData(string text, string score);
        void Train(TrainingModel model);
    }
}
