using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Final.Services
{
    public static class LanguageService
    {
        public static void ApplyCulture(string? cultureCode)
        {
            var culture = new CultureInfo(string.IsNullOrWhiteSpace(cultureCode) ? "fr-CA" : cultureCode);

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    System.Windows.Markup.XmlLanguage.GetLanguage(culture.IetfLanguageTag)));
        }

        internal static void ApplyCulture(object langue)
        {
            throw new NotImplementedException();
        }
    }
}
