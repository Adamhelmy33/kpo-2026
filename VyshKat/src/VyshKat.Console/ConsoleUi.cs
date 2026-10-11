using System.Globalization;

namespace VyshKat.ConsoleApp;

/// <summary>
/// Низкоуровневый ввод/вывод: чтение чисел с повтором при ошибке, заголовки, сообщения.
/// Бизнес-логики здесь нет.
/// </summary>
public sealed class ConsoleUi
{
    /// <summary>Ввод закончился (Ctrl+D / Ctrl+Z или конец перенаправленного файла).</summary>
    public sealed class InputClosedException : Exception
    {
    }

    public void Header(string title)
    {
        Console.WriteLine();
        Console.WriteLine($"=== {title} ===");
    }

    public void Info(string message) => Console.WriteLine(message);

    public void Success(string message) => WriteColored($"✔ {message}", ConsoleColor.Green);

    public void Warning(string message) => WriteColored($"✘ {message}", ConsoleColor.Yellow);

    public void Error(string message) => WriteColored($"Ошибка: {message}", ConsoleColor.Red);

    public string ReadText(string prompt)
    {
        while (true)
        {
            var line = ReadLine(prompt).Trim();
            if (line.Length > 0)
            {
                return line;
            }

            Error("значение не может быть пустым.");
        }
    }

    public int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            var line = ReadLine($"{prompt} [{min}–{max}]: ");
            if (int.TryParse(line, NumberStyles.Integer, CultureInfo.CurrentCulture, out var value)
                && value >= min && value <= max)
            {
                return value;
            }

            Error($"введите целое число от {min} до {max}.");
        }
    }

    /// <summary>Читает целое число без проверки диапазона — проверку выполняет доменная модель.</summary>
    public int ReadAnyInt(string prompt)
    {
        while (true)
        {
            var line = ReadLine(prompt);
            if (int.TryParse(line, NumberStyles.Integer, CultureInfo.CurrentCulture, out var value))
            {
                return value;
            }

            Error("введите целое число.");
        }
    }

    /// <summary>Принимает и «0,45», и «0.45».</summary>
    public decimal ReadDecimal(string prompt)
    {
        while (true)
        {
            var line = ReadLine(prompt).Replace('.', ',');
            if (decimal.TryParse(line, NumberStyles.Number, CultureInfo.GetCultureInfo("ru-RU"), out var value))
            {
                return value;
            }

            Error("введите число, например 0,45.");
        }
    }

    public bool ReadYesNo(string prompt)
    {
        while (true)
        {
            var line = ReadLine($"{prompt} (д/н): ").Trim().ToLowerInvariant();
            switch (line)
            {
                case "д" or "да" or "y" or "yes":
                    return true;
                case "н" or "нет" or "n" or "no":
                    return false;
                default:
                    Error("ответьте «д» или «н».");
                    break;
            }
        }
    }

    public void Pause()
    {
        Console.WriteLine();
        Console.Write("Нажмите Enter, чтобы вернуться в меню...");
        _ = Console.ReadLine();
    }

    private static string ReadLine(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine() ?? throw new InputClosedException();
    }

    private static void WriteColored(string message, ConsoleColor color)
    {
        var previous = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ForegroundColor = previous;
    }
}
