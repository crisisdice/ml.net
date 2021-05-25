namespace Sentiment
{
    public interface IPredictionService
    {
        bool Predict(string rating);
        void WriteTestData(string text, string score);
    }
}
