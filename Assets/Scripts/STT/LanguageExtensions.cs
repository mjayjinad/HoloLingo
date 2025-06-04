using System;
using System.Diagnostics;

public static class LanguageExtensions
{
    public static string ToDisplayString(this Languages lang)
    {
        switch (lang)
        {
            case Languages.en:
                return "English";
            case Languages.es:
                return "Spanish";
            case Languages.hu:
                return "Hungarian";
            case Languages.fr:
                return "French";
            case Languages.bg:
                return "Bulgarian";
            default:
                return lang.ToString();
        }
    }

    public static Languages ToLanguageEnum(string languageName)
    {
        switch (languageName.ToLower())
        {
            case "english":
                return Languages.en;
            case "spanish":
                return Languages.es;
            case "hungarian":
                return Languages.hu;
            case "french":
                return Languages.fr;
            case "bulgarian":
                return Languages.bg;
            default:
                throw new ArgumentException($"Unsupported language: {languageName}");
        }
    }
}