namespace VyshKat.Core.Inspection;

/// <summary>Решение техосмотра: одобрено или отклонено с перечнем причин.</summary>
public sealed class InspectionResult
{
    private InspectionResult(bool isApproved, IReadOnlyList<string> remarks)
    {
        IsApproved = isApproved;
        Remarks = remarks;
    }

    public bool IsApproved { get; }

    /// <summary>Причины отказа. Пуст, если техосмотр пройден.</summary>
    public IReadOnlyList<string> Remarks { get; }

    public static InspectionResult Approved() => new(true, Array.Empty<string>());

    /// <summary>Отказ обязательно сопровождается хотя бы одной причиной.</summary>
    public static InspectionResult Rejected(params string[] reasons)
    {
        ArgumentNullException.ThrowIfNull(reasons);

        var cleaned = reasons
            .Where(r => !string.IsNullOrWhiteSpace(r))
            .Select(r => r.Trim())
            .ToArray();

        if (cleaned.Length == 0)
        {
            throw new ArgumentException("Отказ в техосмотре должен содержать причину.", nameof(reasons));
        }

        return new InspectionResult(false, cleaned);
    }
}
