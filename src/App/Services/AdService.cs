using App.Interfaces;
using App.Models;

namespace App.Services;

public class AdService : IAdService
{
    private SortedDictionary<string, HashSet<string>> _sortedAds = new();
    readonly LoadResult _result = new();

    //Загрузка содержимого файла
    public LoadResult LoadData(string fileContent)
    {
        //Проверка файла
        if (string.IsNullOrWhiteSpace(fileContent))
        {
            _result.Errors.Add("Файл пуст");
            return _result;
        }

        _sortedAds.Clear();
        string[] lines = fileContent.Split("\n");
        //Проверки строки в файле 
        foreach (string line in lines)
        {
            var values = line.Split(":");
            if (values.Length != 2)
            {
                _result.Errors.Add($"Строка({line}) имеет неверный формат");
                continue;
            }

            var platformName = values[0].Trim();
            if (string.IsNullOrWhiteSpace(platformName))
            {
                _result.Errors.Add($"Строка({line}) имя платформы не может быть пустым");
                continue;
            }

            List<string> locations = values[1].Split(",").Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x)).ToList();

            if (!locations.Any())
            {
                _result.Errors.Add($"Строка({line}) локации должны существовать для платформы ({platformName})");
                continue;
            }

            //Добавление Локации и рекламной площадки в коллекцию
            foreach (string location in locations)
            {
                AddLocation(location, platformName);
            }
        }

        var updatedAds = new SortedDictionary<string, HashSet<string>>(_sortedAds);
        //Вложенный цикл для объединения рекламных площадок по ключу
        foreach (var reverseCollection in _sortedAds.Reverse())
        {
            foreach (var originalCollection in _sortedAds)
            {
                if (reverseCollection.Key != originalCollection.Key && reverseCollection.Key.StartsWith(originalCollection.Key))
                {
                    updatedAds[reverseCollection.Key].UnionWith(originalCollection.Value);
                }
            }
        }

        _sortedAds.Clear();
        _sortedAds = updatedAds;

        _result.Success = _result.Errors.Count == 0;
        return _result;
    }

    //Метод получения списка рекламных площадок для локации
    public List<string> GetPlatformsForLocation(string location)
    {
        if (_sortedAds.TryGetValue(location, out var platforms))
        {
            return platforms.ToList();
        }

        return new List<string>();
    }

    //Метод добавления локации и рекламной площадки в коллекцию
    private void AddLocation(string location, string platformName)
    {
        if (!_sortedAds.ContainsKey(location))
        {
            _sortedAds[location] = new HashSet<string>();
        }

        _sortedAds[location].Add(platformName);
    }
}
//Алгоритм не идеален