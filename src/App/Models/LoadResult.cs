namespace App.Models;

public class LoadResult
{
    public bool Success { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
}