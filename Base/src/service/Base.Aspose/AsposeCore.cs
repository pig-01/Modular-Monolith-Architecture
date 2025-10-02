using System.Text;
using Aspose.Words;
using Aspose.Words.Drawing;
using Aspose.Words.Fields;
using Aspose.Words.Saving;
using Base.Domain.SeedWorks;

namespace Base.Aspose;

public class AsposeCore : BaseService, IAsposeCore
{
    private readonly License license = new();

    private Document? document;

    public AsposeCore(Stream document) => this.document = new Document(document);

    public AsposeCore(string licensePath, Stream document)
    {
        Initial(licensePath);
        this.document = new Document(document);
    }

    public SaveOutputParameters Save(Stream output, SaveFormat format) => document.Save(output, format);


    #region ShowFiledInWord

    public string ShowFiledInWord() => ShowFiledInWord(document);

    public string ShowFiledInWord(string filePath)
    {
        Document document = new(filePath);
        return ShowFiledInWord(document);
    }

    public string ShowFiledInWord(Document document)
    {
        StringBuilder stringBuilder = new();

        // 循環遍歷文檔字段。
        foreach (Field field in document.Range.Fields)
        {
            string fieldCode = field.GetFieldCode();
            string fieldResult = field.Result;

            //對字段的程式碼和結果執行一些操作。
            stringBuilder.AppendLine($"{fieldCode}:  {fieldResult}");
        }

        return stringBuilder.ToString();
    }

    #endregion

    #region InsertImageIntoWord

    public void InsertImageIntoWord(byte[] image, int left, int top, int width, int height) => InsertImageIntoWord(document, image, left, top, width, height);

    public void InsertImageIntoWord(string licensePath, Stream input, Stream output, byte[] image, int left, int top, int width, int height)
    {
        SetLicense(licensePath);
        // 加載 Word 文件
        Document doc = new(input);

        InsertImageIntoWord(doc, image, left, top, width, height);

        // 保存文件
        doc.Save(output, SaveFormat.Docx);
    }

    private static void InsertImageIntoWord(Document doc, byte[] image, int left, int top, int width, int height)
    {
        // 找到最後一個章節
        Section lastSection = doc.LastSection;

        // 尋找乙方區塊的段落
        Paragraph lastParagraph = lastSection.Body.LastParagraph;

        if (lastParagraph != null)
        {
            // 建立文件構建器
            DocumentBuilder builder = new(doc);

            // 獲取最後一節的最後一個段落
            builder.MoveTo(lastParagraph);

            // 插入圖片並設置浮動位置
            Shape shape = builder.InsertImage(image);

            // 設置圖片的浮動行為
            shape.WrapType = WrapType.None;
            shape.RelativeHorizontalPosition = RelativeHorizontalPosition.Page;
            shape.RelativeVerticalPosition = RelativeVerticalPosition.Page;

            // 設置圖片的具體位置（例如距離頁面左上角的偏移量）
            shape.Left = left;  // 以點數設置水平位置
            shape.Top = top;   // 以點數設置垂直位置

            // 設置圖片的大小（可以根據需求調整）
            shape.Width = width;
            shape.Height = height;
            //文字顯示在最前方
            shape.BehindText = true;

            shape.ZOrder = 100;
        }
    }
    #endregion

    #region ReplaceParameterIntoWord

    public void ReplaceParameterIntoWord(Dictionary<string, string> parameters) => ReplaceParameterIntoWord(document, parameters);

    public void ReplaceParameterIntoWord(string licensePath, Stream input, Stream output, Dictionary<string, string> parameters)
    {
        SetLicense(licensePath);
        // 加載 Word 文件
        Document doc = new(input);

        ReplaceParameterIntoWord(doc, parameters);

        // 保存文件
        doc.Save(output, SaveFormat.Docx);
    }

    private static void ReplaceParameterIntoWord(Document doc, Dictionary<string, string> parameters)
    {
        // 逐個替換參數
        foreach (KeyValuePair<string, string> parameter in parameters)
        {
            // 例如替換 {OUName} 為 "機構名稱"
            doc.Range.Replace($"{{{parameter.Key}}}", parameter.Value);
        }
    }

    #endregion


    #region Private
    private void Initial(string licensePath)
    {
        using MemoryStream stream = new(File.ReadAllBytes(licensePath));
        license.SetLicense(stream);
    }


    public static void SetLicense(string licensePath)
    {
        using MemoryStream stream = new(File.ReadAllBytes(licensePath));
        new License().SetLicense(stream);
    }

    #endregion

    #region IDispose

    public override void Dispose(bool disposing)
    {
        document = null;
        base.Dispose(disposing);
    }
    #endregion


}
