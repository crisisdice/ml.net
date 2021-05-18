namespace Sentiment
{
    public interface IPredictionServices
    {
        bool Predict(string rating);
    }
}