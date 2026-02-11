namespace ProjectDaedalus.Core.Interfaces
{
    public interface IEmailTemplateService
    {
        string RenderTemplate(string templateName, Dictionary<string, string> data);
    }
}