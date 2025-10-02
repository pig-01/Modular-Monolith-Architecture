namespace Base.Aspose;

public class AsposeFactory : IAsposeFactory
{
    public IAsposeCore Create(Stream document) => new AsposeCore(document);

    public IAsposeCore Create(string licensePath, Stream document) => new AsposeCore(licensePath, document);
}
