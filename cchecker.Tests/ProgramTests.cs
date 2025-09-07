using System;
using System.IO;
using cchecker;
using Xunit;

namespace cchecker.Tests;

public class ProgramTests
{
    [Fact]
    public void Main_NoArgs_Returns1()
    {
        var code = Program.Main(Array.Empty<string>());
        Assert.Equal(1, code);
    }

    [Fact]
    public void Main_FileNotFound_Returns2()
    {
        var code = Program.Main(new[] { "nonexistent_file_12345.ccalc" });
        Assert.Equal(2, code);
    }

    [Fact]
    public void Main_ValidInput_Returns0()
    {
        var tmp = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tmp, "1+2*3");
            var code = Program.Main(new[] { tmp });
            Assert.Equal(0, code);
        }
        finally
        {
            if (File.Exists(tmp)) File.Delete(tmp);
        }
    }
}