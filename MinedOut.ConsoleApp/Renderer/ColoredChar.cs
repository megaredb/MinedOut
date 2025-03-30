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
}