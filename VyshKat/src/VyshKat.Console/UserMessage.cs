namespace VyshKat.ConsoleApp;

/// <summary>
/// Убирает из текста ArgumentException служебные хвосты .NET
/// («(Parameter 'x')», «Actual value was ...»), оставляя сообщение доменной модели.
/// </summary>
public static class UserMessage
{
    public static string From(ArgumentException exception)
    {
        var firstLine = exception.Message.Split(Environment.NewLine)[0];
        if (exception.ParamName is not null)
        {
            var suffix = $" (Parameter '{exception.ParamName}')";
            if (firstLine.EndsWith(suffix, StringComparison.Ordinal))
            {
                firstLine = firstLine[..^suffix.Length];
            }
        }

        return firstLine;
    }
}
