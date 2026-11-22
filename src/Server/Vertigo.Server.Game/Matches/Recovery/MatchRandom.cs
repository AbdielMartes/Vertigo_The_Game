using System.Buffers.Binary;
using System.Security.Cryptography;
using Vertigo.Server.Game.Matches.Models;

namespace Vertigo.Server.Game.Matches.Recovery;

internal sealed class MatchRandom
{
    private readonly Guid _seed;
    private ulong _counter;

    public MatchRandom(Guid seed)
    {
        _seed = seed;
    }

    public int Next(int exclusiveMaximum)
    {
        Span<byte> input = stackalloc byte[24];
        _seed.TryWriteBytes(input);
        Span<byte> digest = stackalloc byte[32];
        ulong bound = (ulong)exclusiveMaximum;
        ulong limit = ulong.MaxValue - ulong.MaxValue % bound;
        ulong value;
        do
        {
            BinaryPrimitives.WriteUInt64LittleEndian(input[16..], _counter++);
            SHA256.HashData(input, digest);
            value = BinaryPrimitives.ReadUInt64LittleEndian(digest);
        }
        while (value >= limit);

        return (int)(value % bound);
    }

    public MatchDeck CreateDeck()
    {
        int[] cards = Enumerable.Range(1, MatchDeck.ModuleCount).ToArray();
        for (int index = cards.Length - 1; index > 0; index--)
        {
            int selected = Next(index + 1);
            (cards[index], cards[selected]) = (cards[selected], cards[index]);
        }

        return new MatchDeck(cards);
    }
}
