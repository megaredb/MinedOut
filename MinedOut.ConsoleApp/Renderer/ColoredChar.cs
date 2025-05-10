namespace MinedOut.ConsoleApp.Renderer;

public class ColoredChar
{
    public ConsoleColor? BackgroundColor;
    public ConsoleColor Color;
    public char Value;

    public ColoredChar(char value, ConsoleColor color = ConsoleColor.White, ConsoleColor? backgroundColor = null)
    {
        Value = value;
        Color = color;
        BackgroundColor = backgroundColor;
    }

    public static List<ColoredChar> FromString(string value, ConsoleColor color = ConsoleColor.White,
        ConsoleColor? backgroundColor = null)
    {
        return value.Select(c => new ColoredChar(c, color, backgroundColor)).ToList();
    }
}