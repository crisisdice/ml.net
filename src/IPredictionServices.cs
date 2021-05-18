namespace Sentiment
{
    public interface IPredictionServices
    {
        bool Predict(string rating);
        public void WriteTestData(string text, string score);
    }
}
