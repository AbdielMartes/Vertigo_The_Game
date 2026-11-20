using System;
using System.Collections.Generic;

namespace Vertigo.Client.UI.ViewModels.Presentation;

public sealed class MatchTowerResult
{
    private const double DrawingHeight = 108;

    internal MatchTowerResult((int Built, int Bonus, int Total) floors, int tallest, bool includeBonusInDrawing)
    {
        BuiltFloors = floors.Built;
        BonusFloors = floors.Bonus;
        TotalFloors = floors.Total;

        double floorHeight = DrawingHeight / Math.Max(1, tallest);
        List<MatchTowerSegment> segments = new();
        if (includeBonusInDrawing && BonusFloors > 0)
        {
            segments.Add(new MatchTowerSegment(BonusFloors * floorHeight, IsBonus: true));
        }

        if (BuiltFloors > 0)
        {
            segments.Add(new MatchTowerSegment(BuiltFloors * floorHeight, IsBonus: false));
        }

        Segments = segments;
    }

    public int BuiltFloors { get; }
    public int BonusFloors { get; }
    public int TotalFloors { get; }
    public IReadOnlyList<MatchTowerSegment> Segments { get; }
}

public sealed record MatchTowerSegment(double Height, bool IsBonus);
