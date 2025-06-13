using System.Globalization;
using Microsoft.Extensions.Localization;
using Template.WebAPI.Localization;

public class JsonStringLocalizerFactory : IStringLocalizerFactory
{
    private readonly string _resourcesPath;

    public JsonStringLocalizerFactory(string resourcesPath)
    {
        _resourcesPath = resourcesPath;
    }

    public IStringLocalizer Create(Type resourceSource)
    {
        var culture = CultureInfo.CurrentCulture.Name;
        var filePath = Path.Combine(_resourcesPath, $"localization.{culture}.json");
        if (!File.Exists(filePath))
            filePath = Path.Combine(_resourcesPath, "localization.en-US.json");
        return new JsonStringLocalizer(filePath);
    }

    public IStringLocalizer Create(string baseName, string location) => Create(typeof(object));
}
