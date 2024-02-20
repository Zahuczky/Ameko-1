using Avalonia.Data.Converters;
using Avalonia.Media;
using Holo;
using Holo.DC;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ameko.Converters
{
    public class DCScriptUpdateVersionConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not ScriptEntity script) return string.Empty;

            var updateCandidates = HoloContext.Instance.RepositoryManager.RepoScripts;

            var matches = updateCandidates.Where(s => s.QualifiedName!.Equals(script.QualifiedName));
            if (matches.Any())
            {
                var serverScript = matches.First();
                if (serverScript.CurrentVersion > script.CurrentVersion)
                    return $"Update available: v{serverScript.CurrentVersion}";
            }
            return "Up to date.";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
