using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjectDaedalus.Core.Interfaces;

namespace ProjectDaedalus.Infrastructure.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger<EmailTemplateService> _logger;
        private readonly Dictionary<string, string> _templateCache;

        public EmailTemplateService(
            IHostEnvironment hostEnvironment,
            ILogger<EmailTemplateService> logger)
        {
            _hostEnvironment = hostEnvironment;
            _logger = logger;
            _templateCache = new Dictionary<string, string>();
        }

        public string RenderTemplate(string templateName, Dictionary<string, string> data)
        {
            try
            {
                // Load template (with caching)
                var template = LoadTemplate(templateName);

                // Replace all placeholders
                var rendered = template;
                foreach (var kvp in data)
                {
                    var placeholder = $"{{{{{kvp.Key}}}}}"; // Creates {{Key}}
                    rendered = rendered.Replace(placeholder, kvp.Value);
                }

                _logger.LogDebug("Successfully rendered template '{TemplateName}'", templateName);
                return rendered;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to render template '{TemplateName}'", templateName);
                throw;
            }
        }

        private string LoadTemplate(string templateName)
        {
            // Check cache first
            if (_templateCache.TryGetValue(templateName, out var cachedTemplate))
            {
                return cachedTemplate;
            }

            // Build file path
            var templatePath = Path.Combine(
                _hostEnvironment.ContentRootPath,
                "EmailTemplates",
                $"{templateName}.html"
            );

            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException($"Email template not found: {templateName}.html");
            }

            // Read template
            var template = File.ReadAllText(templatePath);

            // Cache it (templates don't change at runtime)
            _templateCache[templateName] = template;

            _logger.LogInformation("Loaded email template from {Path}", templatePath);
            return template;
        }
    }
}