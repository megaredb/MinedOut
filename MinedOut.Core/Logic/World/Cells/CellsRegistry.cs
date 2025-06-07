namespace MinedOut.Core.Logic.World.Cells;

public static class CellsRegistry
{
    public static Air Air = new();
    public static Wall Wall = new();
    public static Path Path = new();
    public static Mine Mine = new();
    public static Exit Exit = new();
    public static BonusCoin BonusCoin = new();
}