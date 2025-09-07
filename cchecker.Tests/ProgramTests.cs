using System;
using System.IO;
using cchecker;
using Xunit;
using AwesomeAssertions;

namespace cchecker.Tests;

public class ProgramTests
{
    [Fact]
    public void Main_NoArgs_Returns1()
    {
        var code = Program.Main(Array.Empty<string>());
        code.Should().Be(1);
    }

    [Fact]
    public void Main_FileNotFound_Returns2()
    {
        var code = Program.Main(new[] { "nonexistent_file_12345.ccalc" });
        code.Should().Be(2);
    }

    [Fact]
    public void Main_ValidInput_Returns0()
    {
        var tmp = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tmp, "1+2*3");
            var code = Program.Main(new[] { tmp });
            code.Should().Be(0);
        }
        finally
        {
            if (File.Exists(tmp)) File.Delete(tmp);
        }
    }
}