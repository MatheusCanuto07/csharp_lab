namespace SalesSystem.Models;

/// <summary>
/// Sales regions with distinct tax policies for the lab scenario.
/// </summary>
public enum Region
{
    Southeast,
    South,
    Northeast,
    North,
    Midwest,
    /// <summary>Zero-rate region (e.g. free-trade zone).</summary>
    FreeTradeZone
}
