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
}