namespace ProjectDaedalus.Core.Interfaces
{
    public interface IEmailTemplateService
    {
        /// <summary>
        /// Renders an email template by replacing placeholders with provided data
        /// </summary>
        /// <param name="templateName">Name of the template file (without .html extension)</param>
        /// <param name="data">Dictionary of placeholder names and their values</param>
        /// <returns>Rendered HTML string</returns>
        string RenderTemplate(string templateName, Dictionary<string, string> data);
    }
}