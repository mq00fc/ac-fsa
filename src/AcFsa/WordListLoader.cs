using System.Net.Http;

namespace AcFsa;

/// <summary>
/// 工具类：从 fwwdn/sensitive-stop-words 仓库下载词库。
/// </summary>
public static class WordListLoader
{
    private const string BaseRawUrl = "https://raw.githubusercontent.com/fwwdn/sensitive-stop-words/master/";

    /// <summary>
    /// 内置的词库文件名列表（对应 fwwdn/sensitive-stop-words 仓库）。
    /// </summary>
    public static readonly string[] DefaultFiles =
    [
        "stopword.dic",
        "广告.txt",
        "政治类.txt",
        "涉枪涉爆违法信息关键词.txt",
        "网址.txt",
        "色情类.txt",
    ];

    /// <summary>
    /// 从 GitHub 下载并返回词库字符串列表。
    /// </summary>
    public static async Task<IEnumerable<string>> FetchRemoteAsync(
        string[]? fileNames = null,
        HttpClient? httpClient = null,
        CancellationToken ct = default)
    {
        fileNames ??= DefaultFiles;
        var client = httpClient ?? new HttpClient();
        var words = new List<string>();

        foreach (var name in fileNames)
        {
            var url = BaseRawUrl + Uri.EscapeDataString(name);
            var content = await client.GetStringAsync(url, ct);
            words.AddRange(content.Split('\n').Select(l => l.Trim()).Where(l => l.Length > 0));
        }

        return words;
    }
}
