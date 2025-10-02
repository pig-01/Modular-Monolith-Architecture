using Aspose.Words;
using Aspose.Words.Saving;

namespace Base.Aspose;

public interface IAsposeCore
{
    SaveOutputParameters Save(Stream output, SaveFormat format);

    string ShowFiledInWord();

    string ShowFiledInWord(string filePath);

    string ShowFiledInWord(Document document);

    void InsertImageIntoWord(byte[] image, int left, int top, int width, int height);

    void InsertImageIntoWord(string licensePath, Stream input, Stream output, byte[] image, int left, int top, int width, int height);

    void ReplaceParameterIntoWord(Dictionary<string, string> parameters);

    void ReplaceParameterIntoWord(string licensePath, Stream input, Stream output, Dictionary<string, string> parameters);

}
