namespace Base.Domain.Options.FrontEnd;

public class FrontEndOption
{
    public const string Position = "FrontEnd";

    public string Url { get; set; } = "https://bdbu-Demo.Demo.com.tw/";

    public string ApiUrl { get; set; } = "https://bdbu-Demo.Demo.com.tw/WebAPI/api/";

    public string ApiVersion { get; set; } = "v1";

    public int Timeout { get; set; } = 30;

    public bool EnableCaching { get; set; } = true;
}
