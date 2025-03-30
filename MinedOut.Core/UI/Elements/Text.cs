using MinedOut.Core.Utilities;

namespace MinedOut.Core.UI.Elements;

public class Text : Element
{
    public string Value;

    public Text(string value, Vector2I position)
    {
        Value = value;
        Position = position;
    }
}