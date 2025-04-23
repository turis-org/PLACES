namespace Turis.Sights;
public record Properties
{
    public string? xid { get; set; }
    public string? name { get; set; }
    public double dist { get; set; }
    public string? kinds { get; set; }
}
public record Sight
{
    public string? id { get; set; }
    public Properties? properties { get; set; }
}