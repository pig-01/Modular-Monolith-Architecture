using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Base.Infrastructure.Toolkits.Resources;
using Microsoft.AspNetCore.Http.Metadata;

namespace DataHub.Cloud.Models.Provision;


public partial class OrderDto : IEndpointParameterMetadataProvider
{
    [JsonPropertyName("id")]
    [DefaultValue("100003979")]
    public string? Id { get; set; }

    [JsonPropertyName("createAt")]
    [DefaultValue("2025-06-19T11:12:30")]
    public DateTimeOffset? CreateAt { get; set; }

    [JsonPropertyName("store")]
    public Store? Store { get; set; }

    [JsonPropertyName("customer")]
    public Customer? Customer { get; set; }

    [JsonPropertyName("enterprise")]
    public Enterprise? Enterprise { get; set; }

    [JsonPropertyName("affiliate")]
    public Affiliate? Affiliate { get; set; }

    [JsonPropertyName("items")]
    public List<Item>? Items { get; set; }


    public static void PopulateMetadata(ParameterInfo parameter, EndpointBuilder builder) => builder.Metadata.Add(new AcceptsMetadata(["application/json"], typeof(OrderDto)));
}

public partial class Affiliate
{
    [JsonPropertyName("email")]
    [DefaultValue("vendor@example.com")]
    public string? Email { get; set; }

    [JsonPropertyName("code")]
    [DefaultValue("cfcd208495d565ef66e7dff9f98764da")]
    public string? Code { get; set; }

    [JsonPropertyName("name")]
    [DefaultValue("Vendor")]
    public string? Name { get; set; }
}

public partial class Customer
{
    [JsonPropertyName("id")]
    [DefaultValue("9c497b72-7b28-4162-a4e2-389eb11c4f68")]
    [Required(ErrorMessageResourceName = "ArgumentExceptionMessage", ErrorMessageResourceType = typeof(MessageResource))]
    public Guid Id { get; set; }

    [JsonPropertyName("email")]
    [DefaultValue("jason_tsai@Demo.com.tw")]
    public required string Email { get; set; }

    [JsonPropertyName("emailVerified")]
    [DefaultValue(false)]
    public bool? EmailVerified { get; set; }

    [JsonPropertyName("loginId")]
    [DefaultValue("jason_tsai@Demo.com.tw")]
    public string? LoginId { get; set; }

    [JsonPropertyName("enterprises")]
    public List<Enterprise>? Enterprises { get; set; }
}

public partial class Enterprise
{
    [JsonPropertyName("id")]
    [DefaultValue("example_enterprise_id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    [DefaultValue("範例企業")]
    public string? Name { get; set; }
}

public partial class Item
{
    [JsonPropertyName("sku")]
    [DefaultValue("demo_Demo_trial")]
    public string Sku { get; set; } = "";

    [JsonPropertyName("name")]
    [DefaultValue("雲端指標管理系統-Demo永續指標管理系統（測試用）")]
    public string? Name { get; set; }

    [JsonPropertyName("unitPrice")]
    [DefaultValue(600)]
    public long UnitPrice { get; set; }

    [JsonPropertyName("discount")]
    [DefaultValue(60)]
    public long Discount { get; set; }

    [JsonPropertyName("netPrice")]
    [DefaultValue(540)]
    public long NetPrice { get; set; }

    [JsonPropertyName("quantity")]
    [DefaultValue(1)]
    public long Quantity { get; set; }

    [JsonPropertyName("options")]
    public List<Option>? Options { get; set; }
}

public partial class Option
{
    [JsonPropertyName("code")]
    [DefaultValue("description")]
    public string? Code { get; set; }

    [JsonPropertyName("name")]
    [DefaultValue("說明")]
    public string? Name { get; set; }

    [JsonPropertyName("value")]
    [DefaultValue("10 GB(1 年)")]
    public string? Value { get; set; }
}

public partial class Store
{
    [JsonPropertyName("id")]
    [DefaultValue("default")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    [DefaultValue("雲端服務採購平台 (正體中文)")]
    public string? Name { get; set; }

    [JsonPropertyName("locale")]
    [DefaultValue("zh-CHT")]
    public string? Locale { get; set; }

    [JsonPropertyName("timezone")]
    [DefaultValue("Asia/Taipei")]
    public string? Timezone { get; set; }

    [JsonPropertyName("currency")]
    [DefaultValue("TWD")]
    public string? Currency { get; set; }
}


