using System.Security.Cryptography;

namespace Vertigo.Server.Game.Matches.Models;

// The 100 numbered modules come from the architecture document.
// Initial hand size remains provisional until the rules are finalized.
internal sealed class MatchDeck
{
    public const int ModuleCount = 100;
    public const int InitialHandSize = 6;

    private readonly Queue<int> _remaining;

    public MatchDeck()
    {
        int[] cards = Enumerable.Range(1, ModuleCount).ToArray();
        for (int index = cards.Length - 1; index > 0; index--)
        {
            int selected = RandomNumberGenerator.GetInt32(index + 1);
            (cards[index], cards[selected]) = (cards[selected], cards[index]);
        }

        _remaining = new Queue<int>(cards);
    }

    public int RemainingCount => _remaining.Count;

    public IReadOnlyList<int> Draw(int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        if (count > _remaining.Count)
        {
            throw new InvalidOperationException("There are not enough modules in the deck.");
        }

        List<int> drawn = new(count);
        for (int index = 0; index < count; index++)
        {
            drawn.Add(_remaining.Dequeue());
        }

        return drawn.AsReadOnly();
    }
}
