namespace Backend.Features.PlayerItems
{
    public readonly struct PlayerItemId
    {
        public PlayerItemId(int value)
        {
            Value = value;
        }

        public readonly int Value;

        public bool Equals(PlayerItemId other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object? obj)
        {
            return obj is PlayerItemId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }

    public interface IPlayerItem
    {
        PlayerItemId Id { get; }
    }
}