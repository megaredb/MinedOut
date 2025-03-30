using MinedOut.Core.Logic;

namespace MinedOut.Core.Renderer;

public interface IRenderer
{
    public static abstract IRenderer CreateInstance(GameState gameState);
    public void Render();
}