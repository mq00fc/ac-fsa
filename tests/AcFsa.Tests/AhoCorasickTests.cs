using AcFsa;
using Xunit;

namespace AcFsa.Tests;

public class AhoCorasickTests
{
    [Fact]
    public void Filter_ReplacesExactMatch()
    {
        var ac = new AhoCorasick();
        ac.Insert("尼玛");
        ac.Build();
        Assert.Equal("我**", ac.Filter("我尼玛"));
    }

    [Fact]
    public void Filter_MultipleWords()
    {
        var ac = new AhoCorasick();
        ac.Insert("操");
        ac.Insert("尼玛");
        ac.Build();
        var result = ac.Filter("我操尼玛");
        Assert.Equal("我***", result);
    }

    [Fact]
    public void Filter_NoMatch_ReturnsOriginal()
    {
        var ac = new AhoCorasick();
        ac.Insert("敏感词");
        ac.Build();
        Assert.Equal("你好世界", ac.Filter("你好世界"));
    }

    [Fact]
    public void Filter_OverlappingWords()
    {
        var ac = new AhoCorasick();
        ac.Insert("abc");
        ac.Insert("bc");
        ac.Build();
        var result = ac.Filter("xabcx");
        // "abc" covers positions 1-3, "bc" at 2-3 — all masked
        Assert.Equal("x***x", result);
    }

    [Fact]
    public void Search_ReturnsAllMatches()
    {
        var ac = new AhoCorasick();
        ac.Insert("he");
        ac.Insert("she");
        ac.Insert("his");
        ac.Insert("hers");
        ac.Build();
        var hits = ac.Search("ushers");
        var words = hits.Select(h => h.Word).ToHashSet();
        Assert.Contains("he", words);
        Assert.Contains("she", words);
        Assert.Contains("hers", words);
    }
}

public class SensitiveFilterTests
{
    [Fact]
    public void LoadWords_And_Filter()
    {
        var filter = new SensitiveFilter();
        filter.LoadWords(["操蛋", "傻逼", "尼玛"]);
        Assert.Equal("你**啊", filter.Filter("你操蛋啊"));
        Assert.Equal("**", filter.Filter("傻逼"));
        Assert.True(filter.ContainsSensitiveWord("尼玛蛋"));
    }

    [Fact]
    public void FindAll_ReturnsDistinctWords()
    {
        var filter = new SensitiveFilter();
        filter.LoadWords(["abc", "def"]);
        var found = filter.FindAll("xabcxdefxabc");
        Assert.Contains("abc", found);
        Assert.Contains("def", found);
        Assert.Equal(2, found.Count);
    }

    [Fact]
    public void Reset_ClearsWords()
    {
        var filter = new SensitiveFilter();
        filter.LoadWords(["abc"]);
        filter.Reset();
        Assert.False(filter.ContainsSensitiveWord("abc"));
        Assert.Equal(0, filter.WordCount);
    }
}
