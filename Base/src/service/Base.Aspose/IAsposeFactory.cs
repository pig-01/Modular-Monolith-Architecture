namespace Base.Aspose;

public interface IAsposeFactory
{
    IAsposeCore Create(Stream document);

    IAsposeCore Create(string licensePath, Stream document);
}
