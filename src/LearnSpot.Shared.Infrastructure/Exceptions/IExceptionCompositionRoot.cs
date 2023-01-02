using LearnSpot.Shared.Abstractions.Exceptions;

namespace LearnSpot.Shared.Infrastructure.Exceptions;

internal interface IExceptionCompositionRoot
{
    ExceptionResponse Map(Exception exception);
}