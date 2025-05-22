using MinedOut.Core.Logic.World.Cells;

namespace MinedOut.Core.Utilities;

public class AStar
{
    public static List<Vector2I> FindPath(Vector2I start, Vector2I end, Cell[,] grid)
    {
        var width = grid.GetLength(0);
        var height = grid.GetLength(1);

        var startNode = new Node(start) { GCost = 0 };
        var endNode = new Node(end);

        var openList = new List<Node> { startNode };
        var closedList = new HashSet<Vector2I>();

        while (openList.Count > 0)
        {
            var currentNode = openList.OrderBy(n => n.FCost).ThenBy(n => n.HCost).First();

            if (currentNode.Position.Equals(end))
                return ReconstructPath(currentNode);

            openList.Remove(currentNode);
            closedList.Add(currentNode.Position);

            foreach (var neighborPos in GetNeighborPositions(currentNode.Position, width, height, grid))
            {
                if (closedList.Contains(neighborPos)) continue;

                var neighborNode = openList.FirstOrDefault(n => n.Position == neighborPos)
                                   ?? new Node(neighborPos) { GCost = int.MaxValue };

                var tentativeGCost = currentNode.GCost + 1;

                if (tentativeGCost < neighborNode.GCost)
                {
                    neighborNode.GCost = tentativeGCost;
                    neighborNode.HCost = CalculateManhattan(neighborPos, end);
                    neighborNode.Parent = currentNode;

                    if (!openList.Contains(neighborNode))
                        openList.Add(neighborNode);
                }
            }
        }

        return new List<Vector2I>();
    }

    private static IEnumerable<Vector2I> GetNeighborPositions(Vector2I current, int width, int height, Cell[,] grid)
    {
        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };

        for (var i = 0; i < 4; i++)
        {
            var x = current.X + dx[i];
            var y = current.Y + dy[i];

            if (x >= 0 && x < width && y >= 0 && y < height && grid[x, y].IsPassable)
                yield return new Vector2I(x, y);
        }
    }

    private static List<Vector2I> ReconstructPath(Node endNode)
    {
        var path = new List<Vector2I>();
        var currentNode = endNode;

        while (currentNode != null)
        {
            path.Add(currentNode.Position);
            currentNode = currentNode.Parent;
        }

        path.Reverse();
        return path;
    }

    private static int CalculateManhattan(Vector2I a, Vector2I b)
    {
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }

    private class Node
    {
        public Node(Vector2I position)
        {
            Position = position;
        }

        public Vector2I Position { get; }
        public int GCost { get; set; } = int.MaxValue;
        public int HCost { get; set; }
        public int FCost => GCost + HCost;
        public Node? Parent { get; set; }
    }
}