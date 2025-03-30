using MinedOut.Core.UI.Elements;

namespace MinedOut.Core.UI;

public abstract class Screen
{
    protected Root Root = new();

    public void PlaceElement(Element element)
    {
        Root.AddChild(element);
    }

    public void RemoveElement(Element element)
    {
        Root.RemoveChild(element);
    }
}