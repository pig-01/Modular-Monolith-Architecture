using System.Text;
using Xunit.Abstractions;

namespace Base.Infrastructure.Toolkits.Test;

public class TestBase
{
    protected class Converter : TextWriter
    {
        private readonly ITestOutputHelper _output;
        public Converter(ITestOutputHelper output) => _output = output;
        public override Encoding Encoding => Encoding.UTF8;
        public override void WriteLine(string value) => _output.WriteLine(value);
        public override void WriteLine(string format, params object[] arg) => _output.WriteLine(format, arg);

        public override void Write(char value) => throw new NotSupportedException("This text writer only supports WriteLine(string) and WriteLine(string, params object[]).");
    }
}


