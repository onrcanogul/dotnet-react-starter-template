using System.Text.Json;
using Microsoft.Extensions.Localization;

namespace Template.WebAPI.Localization;

public class JsonStringLocalizer : IStringLocalizer
{
    private readonly Dictionary<string, string> _localization;
    public JsonStringLocalizer(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Localization file not found: {path}");

        var jsonContent = File.ReadAllText(path);
        _localization = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent) ?? new Dictionary<string, string>();
    }
    public LocalizedString this[string name]
    {
        get
        {
            var value = _localization.TryGetValue(name, out var localizedValue) ? localizedValue : name;
            return new LocalizedString(name, value, resourceNotFound: value == name);
        }
    }
    public LocalizedString this[string name, params object[] arguments] => this[name];
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
        _localization.Select(l => new LocalizedString(l.Key, l.Value));
}