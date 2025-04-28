namespace Turis.Location;
public record Point
{
    public double lng { get; set; }
    public double lat { get; set; }
}
public record Place
{
    public string? name { get; set; }
    public string? country { get; set; }
    public string? city { get; set; }
    public Point? point { get; set; }
    public string? osmValue { get; set; }
}