using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Entities;
using DestilatechPlatform.API.Shared.Domain.Model.Aggregates.Internal;

namespace DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Aggregates;

public sealed partial class ProductionBatch : AggregateRoot
{
    private readonly List<BatchStageChange> _stageHistory = [];

    public int Id { get; private set; }
    public int UserId { get; private set; }
    public string Code { get; private set; } = "";
    public string Variety { get; private set; } = "";
    public DateTime StartDate { get; private set; }
    public decimal EstimatedQuantity { get; private set; }
    public string Tank { get; private set; } = "";
    public string Stage { get; private set; } = "RECEIVED";
    public string Notes { get; private set; } = "";
    public int? ProductId { get; private set; }
    public int BottledUnits { get; private set; }
    public int Version { get; private set; }
    public IReadOnlyCollection<BatchStageChange> StageHistory => _stageHistory.AsReadOnly();

    private ProductionBatch() { }

    public ProductionBatch(int userId, string code, string variety, DateTimeOffset startDate,
        decimal estimatedQuantity, string? tank, string? notes)
    {
        if (userId <= 0 || string.IsNullOrWhiteSpace(code) || code.Trim().Length > 80)
            throw new ArgumentException("Invalid account or batch code.");

        if (startDate == default || startDate > DateTimeOffset.UtcNow)
            throw new ArgumentException("A valid start date is required.");

        UserId = userId;
        Code = code.Trim();
        StartDate = startDate.UtcDateTime;
        Edit(variety, estimatedQuantity, tank, notes);
        _stageHistory.Add(new BatchStageChange(Stage, StartDate, ""));
    }

    public void Edit(string variety, decimal estimatedQuantity, string? tank, string? notes,
        bool recordChange = false)
    {
        if (Stage == "BOTTLED")
            throw new InvalidOperationException("The batch is closed.");

        if (string.IsNullOrWhiteSpace(variety) || variety.Trim().Length > 80)
            throw new ArgumentException("A grape variety is required.");

        if (estimatedQuantity <= 0 || estimatedQuantity >= 1_000_000_000m ||
            decimal.Round(estimatedQuantity, 3) != estimatedQuantity)
            throw new ArgumentException("Estimated quantity must be positive with up to three decimals.");

        if (tank?.Length > 80 || notes?.Length > 255)
            throw new ArgumentException("Tank or notes is too long.");

        if (recordChange && (Variety != variety.Trim() || EstimatedQuantity != estimatedQuantity ||
            Tank != (tank ?? "").Trim() || Notes != (notes ?? "").Trim()))
            _stageHistory.Add(new BatchStageChange(Stage, DateTime.UtcNow, "Batch details updated."));

        Variety = variety.Trim();
        EstimatedQuantity = estimatedQuantity;
        Tank = (tank ?? "").Trim();
        Notes = (notes ?? "").Trim();
        Version++;
    }

    public void Advance(string stage, int? productId, int bottledUnits, string? notes)
    {
        var next = Stage switch
        {
            "RECEIVED" => "FERMENTATION",
            "FERMENTATION" => "DISTILLATION",
            "DISTILLATION" => "RESTING",
            "RESTING" => "BOTTLED",
            _ => null
        };

        if (next is null || stage != next)
            throw new InvalidOperationException("Invalid batch stage transition.");

        if (notes?.Length > 255)
            throw new ArgumentException("Stage notes are too long.");

        if (stage == "BOTTLED")
        {
            if (productId is null or <= 0 || bottledUnits <= 0)
                throw new ArgumentException("Select a product and enter the number of bottles.");

            ProductId = productId;
            BottledUnits = bottledUnits;
        }

        Stage = stage;
        Version++;
        _stageHistory.Add(new BatchStageChange(stage, DateTime.UtcNow, notes ?? ""));
    }
}