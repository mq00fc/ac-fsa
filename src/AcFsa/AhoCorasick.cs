using System.Text;

namespace AcFsa;

/// <summary>
/// Aho-Corasick 自动机节点
/// </summary>
internal sealed class AcNode
{
    public Dictionary<char, AcNode> Children { get; } = new();
    public AcNode? Fail { get; set; }
    public AcNode? Output { get; set; }  // suffix link for output
    public string? Word { get; set; }    // terminal word (longest at this node)
    public List<string> Outputs { get; } = new(); // all words ending here
}

/// <summary>
/// Aho-Corasick 自动机，用于多模式字符串匹配。
/// </summary>
public sealed class AhoCorasick
{
    private readonly AcNode _root = new();
    private bool _built;

    /// <summary>
    /// 向自动机插入一个模式词。
    /// </summary>
    public void Insert(string word)
    {
        if (string.IsNullOrEmpty(word)) return;
        _built = false;
        var node = _root;
        foreach (var ch in word)
        {
            if (!node.Children.TryGetValue(ch, out var next))
            {
                next = new AcNode();
                node.Children[ch] = next;
            }
            node = next;
        }
        node.Word = word;
        node.Outputs.Add(word);
    }

    /// <summary>
    /// 构建失败指针（BFS）。
    /// </summary>
    public void Build()
    {
        var queue = new Queue<AcNode>();
        foreach (var child in _root.Children.Values)
        {
            child.Fail = _root;
            queue.Enqueue(child);
        }

        while (queue.Count > 0)
        {
            var curr = queue.Dequeue();
            foreach (var (ch, child) in curr.Children)
            {
                var fail = curr.Fail;
                while (fail != null && !fail.Children.ContainsKey(ch))
                    fail = fail.Fail;

                child.Fail = (fail == null) ? _root : fail.Children.GetValueOrDefault(ch, _root);
                if (child.Fail == child) child.Fail = _root;

                // merge output list
                child.Outputs.AddRange(child.Fail.Outputs);
                if (child.Fail.Outputs.Count > 0)
                    child.Output = child.Fail;

                queue.Enqueue(child);
            }
        }
        _built = true;
    }

    /// <summary>
    /// 在文本中查找所有匹配的敏感词，返回 (起始索引, 词) 列表。
    /// </summary>
    public List<(int Start, string Word)> Search(string text)
    {
        if (!_built) Build();
        var results = new List<(int, string)>();
        var node = _root;

        for (int i = 0; i < text.Length; i++)
        {
            var ch = text[i];
            while (node != _root && !node.Children.ContainsKey(ch))
                node = node.Fail!;

            if (node.Children.TryGetValue(ch, out var next))
                node = next;

            // collect all outputs at this node
            var tmp = node;
            while (tmp != null && tmp != _root)
            {
                foreach (var w in tmp.Outputs)
                    results.Add((i - w.Length + 1, w));
                tmp = tmp.Output;
                if (tmp == node) break; // safety
            }
        }
        return results;
    }

    /// <summary>
    /// 将文本中所有敏感词替换为 * 号（等长）。
    /// </summary>
    public string Filter(string text)
    {
        if (!_built) Build();
        if (string.IsNullOrEmpty(text)) return text;

        var hits = Search(text);
        if (hits.Count == 0) return text;

        // mark positions to replace
        var mask = new bool[text.Length];
        foreach (var (start, word) in hits)
        {
            for (int j = start; j < start + word.Length && j < text.Length; j++)
                mask[j] = true;
        }

        var sb = new StringBuilder(text.Length);
        for (int i = 0; i < text.Length; i++)
            sb.Append(mask[i] ? '*' : text[i]);

        return sb.ToString();
    }
}
