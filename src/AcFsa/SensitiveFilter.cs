namespace AcFsa;

/// <summary>
/// 敏感词过滤器门面类，封装词库加载与 AC 自动机。
/// </summary>
public sealed class SensitiveFilter
{
    private AhoCorasick _ac = new();
    private readonly object _lock = new();
    private int _wordCount;

    /// <summary>
    /// 已加载的词条数量。
    /// </summary>
    public int WordCount => _wordCount;

    /// <summary>
    /// 从字符串集合批量加载敏感词。
    /// </summary>
    public SensitiveFilter LoadWords(IEnumerable<string> words)
    {
        lock (_lock)
        {
            foreach (var w in words)
            {
                var trimmed = w.Trim();
                if (!string.IsNullOrEmpty(trimmed))
                {
                    _ac.Insert(trimmed);
                    _wordCount++;
                }
            }
            _ac.Build();
        }
        return this;
    }

    /// <summary>
    /// 从文本文件（每行一个词）加载敏感词。
    /// </summary>
    public SensitiveFilter LoadFile(string filePath, System.Text.Encoding? encoding = null)
    {
        var lines = System.IO.File.ReadAllLines(filePath, encoding ?? System.Text.Encoding.UTF8);
        return LoadWords(lines);
    }

    /// <summary>
    /// 从目录中加载所有 .txt 文件的敏感词。
    /// </summary>
    public SensitiveFilter LoadDirectory(string directoryPath, string pattern = "*.txt", System.Text.Encoding? encoding = null)
    {
        foreach (var file in System.IO.Directory.EnumerateFiles(directoryPath, pattern, System.IO.SearchOption.AllDirectories))
            LoadFile(file, encoding);
        return this;
    }

    /// <summary>
    /// 检测文本中是否包含敏感词。
    /// </summary>
    public bool ContainsSensitiveWord(string text)
    {
        lock (_lock) return _ac.Search(text).Count > 0;
    }

    /// <summary>
    /// 过滤文本，将敏感词替换为等长 * 号。
    /// </summary>
    public string Filter(string text)
    {
        lock (_lock) return _ac.Filter(text);
    }

    /// <summary>
    /// 获取文本中所有匹配的敏感词列表。
    /// </summary>
    public IReadOnlyList<string> FindAll(string text)
    {
        lock (_lock) return _ac.Search(text).Select(x => x.Word).Distinct().ToList();
    }

    /// <summary>
    /// 重置词库，清空所有已加载的敏感词。
    /// </summary>
    public SensitiveFilter Reset()
    {
        lock (_lock)
        {
            _ac = new AhoCorasick();
            _wordCount = 0;
        }
        return this;
    }
}
