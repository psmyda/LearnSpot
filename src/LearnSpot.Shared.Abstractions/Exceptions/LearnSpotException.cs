namespace LearnSpot.Shared.Abstractions.Exceptions;

public abstract class LearnSpotException : Exception
{
    protected LearnSpotException(string message) : base(message)
    {
    }
}