using Microsoft.AspNetCore.Mvc;
using ProjectDaedalus.Core.Interfaces;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    private readonly IEmailTemplateService _templateService;

    public TestController(IEmailTemplateService templateService)
    {
        _templateService = templateService;
    }

    [HttpGet("email-template")]
    public IActionResult TestTemplate()
    {
        var html = _templateService.RenderTemplate("LowMoistureAlert", new Dictionary<string, string>
        {
            { "PlantName", "Test Fern" },
            { "CurrentMoisture", "12" },
            { "ThresholdValue", "30" },
            { "DeviceName", "PLANT_99999" },
            { "DashboardUrl", "https://localhost:7001/plants/1" },
            { "Timestamp", DateTime.Now.ToString("MMMM d, yyyy 'at' h:mm tt") }
        });

        return Content(html, "text/html");
    }
}