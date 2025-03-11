using App.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers;

[Route("ads")]
public class AdController : ControllerBase
{
    private readonly IAdService _adService;
    
    public AdController(IAdService adService)
    {
        _adService = adService;
    }
    
    //REST метод для поиска рекламных площадок для локации
    [HttpGet("search")]
    public IActionResult Search(string location)
    {
        if (string.IsNullOrEmpty(location))
            return BadRequest();
        return Ok(_adService.GetPlatformsForLocation(location.Trim()));
    }
    //Rest метод для загрузки данных из файла
    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile? uploadFile)
    {
        if (uploadFile == null || uploadFile.Length == 0)
        {
            return BadRequest("Файл пуст");
        }
        using StreamReader stream = new StreamReader(uploadFile.OpenReadStream());
        var content = await stream.ReadToEndAsync();
        
        return Ok(_adService.LoadData(content));
    }
}