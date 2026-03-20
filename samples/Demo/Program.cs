using AcFsa;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("=== AC-FSA 敏感词过滤演示 ===\n");

var filter = new SensitiveFilter();

// 内置测试词库
filter.LoadWords(["操", "尼玛", "傻逼", "妈的", "草泥马", "狗日", "日你"]);

Console.WriteLine($"已加载词条数: {filter.WordCount}");
Console.WriteLine();

string[] tests =
[
    "我操尼玛",
    "你好世界",
    "傻逼，妈的",
    "这句话是干净的",
    "草泥马的世界",
];

foreach (var text in tests)
{
    var filtered = filter.Filter(text);
    var found = filter.FindAll(text);
    Console.WriteLine($"原文: {text}");
    Console.WriteLine($"过滤: {filtered}");
    if (found.Count > 0)
        Console.WriteLine($"匹配: [{string.Join(", ", found)}]");
    Console.WriteLine();
}

// 可选：从远端词库加载（需要网络）
Console.Write("是否下载在线词库进行测试？(y/N): ");
var input = Console.ReadLine();
if (input?.Trim().ToLower() == "y")
{
    Console.WriteLine("正在下载 fwwdn/sensitive-stop-words ...");
    try
    {
        filter.Reset();
        var words = await WordListLoader.FetchRemoteAsync();
        filter.LoadWords(words);
        Console.WriteLine($"下载完成，已加载词条: {filter.WordCount}");

        Console.Write("请输入文本进行测试: ");
        var userInput = Console.ReadLine() ?? "";
        Console.WriteLine($"过滤结果: {filter.Filter(userInput)}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"下载失败: {ex.Message}");
    }
}
