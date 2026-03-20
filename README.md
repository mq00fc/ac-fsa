# AC-FSA — Aho-Corasick 敏感词过滤库

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/download/dotnet/10.0)

基于 **Aho-Corasick 自动机（AC 自动机）** 实现的高性能敏感词过滤库，适用于 .NET 10。

词库来源：[fwwdn/sensitive-stop-words](https://github.com/fwwdn/sensitive-stop-words)，涵盖广告、政治、色情、涉爆等类别。

---

## 特性

- ⚡ **高效匹配** — AC 自动机时间复杂度 O(n + m)，一次遍历完成所有模式匹配
- 🔄 **等长替换** — 敏感词替换为等长 `*` 号，保持原文结构
- 📦 **灵活加载** — 支持从字符串集合、文件、目录批量加载词库
- 🌐 **在线词库** — 内置工具类，可直接从 GitHub 拉取最新词库
- 🔒 **线程安全** — 过滤操作加锁，适合并发场景
- 🧪 **完善测试** — xUnit 单元测试覆盖核心逻辑

---

## 项目结构

```
ac-fsa/
├── src/
│   └── AcFsa/
│       ├── AcFsa.csproj       # 库项目
│       ├── AhoCorasick.cs     # AC 自动机核心实现
│       ├── SensitiveFilter.cs # 敏感词过滤门面类
│       └── WordListLoader.cs  # 在线词库加载工具
├── tests/
│   └── AcFsa.Tests/
│       ├── AcFsa.Tests.csproj
│       └── AhoCorasickTests.cs
├── samples/
│   └── Demo/
│       ├── Demo.csproj
│       └── Program.cs         # 控制台演示
├── AcFsa.sln
├── LICENSE
└── README.md
```

---

## 快速开始

### 1. 引用项目

```xml
<ProjectReference Include="path/to/src/AcFsa/AcFsa.csproj" />
```

### 2. 从字符串集合加载词库并过滤

```csharp
using AcFsa;

var filter = new SensitiveFilter();
filter.LoadWords(["操", "尼玛", "傻逼", "妈的"]);

string result = filter.Filter("我操尼玛");
Console.WriteLine(result); // 输出: 我***
```

### 3. 从本地文件加载

```csharp
var filter = new SensitiveFilter();
filter.LoadFile("stopword.dic", Encoding.UTF8);
filter.LoadDirectory("./wordlists/", "*.txt");

Console.WriteLine(filter.Filter("这是一段包含敏感词的文本"));
```

### 4. 从 GitHub 在线词库加载

```csharp
using AcFsa;

var words = await WordListLoader.FetchRemoteAsync();
var filter = new SensitiveFilter();
filter.LoadWords(words);

Console.WriteLine(filter.Filter("你好，草泥马！"));
// 输出: 你好，***！
```

### 5. 其他 API

```csharp
// 检测是否含有敏感词
bool hasSensitive = filter.ContainsSensitiveWord("你好");  // false

// 获取所有匹配的敏感词
IReadOnlyList<string> found = filter.FindAll("我操尼玛啊");
// found => ["操", "尼玛"]

// 重置词库
filter.Reset();
```

---

## 词库说明

词库来自 [fwwdn/sensitive-stop-words](https://github.com/fwwdn/sensitive-stop-words)，包含以下类别：

| 文件 | 说明 |
|------|------|
| `stopword.dic` | 通用停用词 |
| `广告.txt` | 广告推广类 |
| `政治类.txt` | 政治敏感类 |
| `涉枪涉爆违法信息关键词.txt` | 涉枪涉爆类 |
| `网址.txt` | 违规网址 |
| `色情类.txt` | 色情类 |

---

## 算法原理

**Aho-Corasick 自动机** 是多模式字符串匹配算法，由 Alfred Aho 和 Margaret Corasick 于 1975 年提出。

核心思想：
1. **Trie 树构建**：将所有模式词插入前缀树
2. **失败指针（Fail 指针）**：BFS 构建失败链接，指向最长真后缀对应节点
3. **文本扫描**：一次线性遍历即可找出所有模式词出现位置

时间复杂度：
- 构建：O(∑|pattern|)
- 搜索：O(|text| + 匹配数)

---

## 运行测试

```bash
dotnet test tests/AcFsa.Tests/
```

---

## 运行演示

```bash
dotnet run --project samples/Demo/
```

---

## 需求环境

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) 或更高版本

---

## 词库授权

敏感词库来源：[fwwdn/sensitive-stop-words](https://github.com/fwwdn/sensitive-stop-words)，该项目采用 MIT 许可证。

---

## 许可证

本项目基于 [MIT License](LICENSE) 开源。
