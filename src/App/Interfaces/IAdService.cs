using App.Models;

namespace App.Interfaces;

public interface IAdService
{
    public LoadResult LoadData(string fileContent);
    public List<string> GetPlatformsForLocation(string location);
}