namespace Docker;

public sealed record ImageName(string Repository, string Tag)
{
    public string Full => $"{Repository}:{Tag}";
}