namespace Toss.Infrastructure.Services.ArtificialIntelligence;

/// <summary>
/// Represents artificial intelligence constants
/// </summary>
public static class ArtificialIntelligenceDefaults
{
    #region ChatGPT

    /// <summary>
    /// Gets ChatGPT API model
    /// </summary>
    public static string ChatGptApiModel => "gpt-4";

    /// <summary>
    /// Gets base ChatGPT API URL
    /// </summary>
    public static string ChatGptBaseApiUrl => "https://api.openai.com/v1/chat/completions";

    #endregion

    #region Gemini

    /// <summary>
    /// Gets base Gemini API URL
    /// </summary>
    public static string GeminiBaseApiUrl => "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash-exp:generateContent";

    /// <summary>
    /// Gets a header of the API key authorization
    /// </summary>
    public static string GeminiApiKeyHeader => "x-goog-api-key";

    #endregion

    #region DeepSeek

    /// <summary>
    /// Gets DeepSeek API model
    /// </summary>
    public static string DeepSeekApiModel => "deepseek-chat";

    /// <summary>
    /// Gets base DeepSeek API URL
    /// </summary>
    public static string DeepSeekBaseApiUrl => "https://api.deepseek.com/chat/completions";

    #endregion

    /// <summary>
    /// Gets a period (in seconds) before the request times out
    /// </summary>
    public static int RequestTimeout => 30;

    /// <summary>
    /// Gets a query format string for generate product description with AI
    /// </summary>
    public static string ProductDescriptionQuery => "Create a description in the {4} language for the product with the \"{0}\" name. Use the following features and keywords: {1}. Use a {2}. {3}.";

    /// <summary>
    /// Gets a query format string to generate meta title (SEO) with AI
    /// </summary>
    public static string MetaTitleQuery => "Create the most suitable meta title in the {2} language for the page with \"{0}\" title and \"{1}\" text. Optimize the result for SEO. Use plain text for response. Keep it under 60 characters.";

    /// <summary>
    /// Gets a query format string to generate meta keywords (SEO) with AI
    /// </summary>
    public static string MetaKeywordsQuery => "Create five or six keywords in the {2} language for the page with \"{0}\" title and \"{1}\" text. Optimize the result for SEO, list them separated by commas. Use plain text for response.";

    /// <summary>
    /// Gets a query format string to generate meta description (SEO) with AI
    /// </summary>
    public static string MetaDescriptionQuery => "Create a meta description in the {2} language for the page with \"{0}\" title and \"{1}\" text. Optimize the result for SEO. Use plain text for response. Keep it under 160 characters.";
}

