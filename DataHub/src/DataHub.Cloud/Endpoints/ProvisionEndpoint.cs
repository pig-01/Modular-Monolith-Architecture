using System.Net;
using System.Text.Encodings.Web;
using System.Web;
using Asp.Versioning;
using Base.Domain.Exceptions;
using Base.Infrastructure.Toolkits.Extensions;
using DataHub.Cloud.Application.Commands;
using DataHub.Cloud.Models.Provision;
using DataHub.Domain.AggregatesModel.OrderAggregate;
using DataHub.Infrastructure.Extensions;
using DataHub.Infrastructure.Filters;
using Microsoft.AspNetCore.Mvc;

namespace DataHub.Cloud.Endpoints;

public static class ProvisionEndpoint
{
    public const string GroupName = "Cloud";
    private const int MajorVersion = 1;
    private const int MinorVersion = 0;

    /// <summary>
    /// 統一註冊到供裝API的Endpoint
    /// </summary>
    /// <param name="app">供裝API Application</param>
    /// <returns></returns>
    public static RouteGroupBuilder MapProvisionV1EndPoint(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("api/v{apiVersion:apiVersion}/")
            .AddEndpointFilter<DemoAuthorizationFilter>()
            .HasApiVersion(new ApiVersion(MajorVersion, MinorVersion));

        group.MapPost($"/{GroupName}/Provision/", ProvisionOrder).MapToApiVersion(MajorVersion, MinorVersion)
            .WithSummary("供裝中心呼叫供裝API(試用開通)")
            .WithDescription("供裝中心呼叫供裝API(試用開通)")
            .WithOpenApi(operation =>
            {
                // 在 Header 中加入訂單到期日描述
                operation.AddEnpointParameter(Microsoft.OpenApi.Models.ParameterLocation.Header, "X-Instance-ExpirationDate", "2026-01-20", "訂單到期日(驗證資訊)", schemaType: "string");
                // 設定參數描述
                operation.RequestBody.Description = "供裝訂單內容";
                return operation;
            });

        // group.MapPost("/DoRenewalAndPurchase", DoRenewalAndPurchase).MapToApiVersion(majorVersion, minorVersion)
        //     .WithSummary("處理加購or續約相關")
        //     .WithDescription("處理加購or續約相關");

        group.MapPost($"/{GroupName}/AddProvision/", AddProvision).MapToApiVersion(MajorVersion, MinorVersion)
            .WithSummary("供裝中心呼叫供裝API(加購續約開通)")
            .WithDescription("供裝中心呼叫供裝API(加購續約開通)")
            .WithOpenApi(operation =>
            {
                // 在 Header 中加入訂單到期日描述
                operation.AddEnpointParameter(Microsoft.OpenApi.Models.ParameterLocation.Header, "X-Instance-ExpirationDate", "2026-01-20", "訂單到期日(驗證資訊)", schemaType: "string");
                // 設定參數描述
                operation.RequestBody.Description = "供裝訂單內容";
                return operation;
            });


        return group;
    }

    /// <summary>
    /// 供裝中心呼叫供裝API(試用開通)
    /// </summary>
    /// <param name="order">供裝訂單內容</param>
    /// <param name="services">供裝訂單內容</param>
    /// <returns></returns>
    [ServiceFilter(typeof(DemoAuthorizationFilter))]
    private static async Task<Results<Ok, Conflict<string>, BadRequest<string>, StatusCodeHttpResult>> ProvisionOrder(OrderDto order, [AsParameters] CloudServices services)
    {
        string expirationDate = string.Empty;
        string newtrialPricingCode = string.Empty;
        Order currentOrder = null;

        try
        {
            services.Logger.LogInformation
            (
                "接收供裝中心呼叫供裝API(試用開通) IP:{RemoteIpAddress} OrderId:{Id}",
                services.HttpRequest.HttpContext.Connection.RemoteIpAddress,
                order.Id
            );

            Assertion.IsNotNull(order, new ArgumentNullException(nameof(order), "參數無法解析"));
            Assertion.IsNotNull(order.Items, new ArgumentNullException(nameof(order), "參數無法解析"));
            Assertion.IsNotTrue(order.Items!.Count == 0, new ArgumentNullException("order.Items", "參數無法解析"));
            Assertion.IsNotTrue(order.Customer is null, new ArgumentNullException("order.Customer", "參數無法解析"));

            //訂單必須包含 demo_專案相關產品定價
            List<Item> items = order.Items;
            Customer customer = order.Customer!;
            Affiliate? affiliate = order.Affiliate; // 可能為null，代表非企業用戶
            Item? orderItem = items.FirstOrDefault(x => !x.Sku.IsNullOrEmpty() && x.Sku!.StartsWith(services.DataHubOptions.Value.SkuPrefix, StringComparison.OrdinalIgnoreCase));
            ArgumentNullException.ThrowIfNull(orderItem, "訂單內容異常 - 找不到 demo_專案的 sku。");

            LogOrderInfo(order, services.Logger);
            //Request Header的ExpirationDate資訊
            expirationDate = GetReuestHeaderValue(services.HttpRequest, "X-Instance-ExpirationDate");


            //檢查是否已經供裝過了
            //應只檢查id ，可能供狀新的試用
            if (await services.OrderQuery.HasProvision(customer.Email, orderItem.Sku))
            {
                LogOrderError(order, services.Logger, new InvalidOperationException("帳號已存在"));
                string errMsg = Convert.ToBase64String(Encoding.UTF8.GetBytes("帳號已存在，系統將引導您回到隸屬的公司，請稍候 ..."));
                if (order.Store != null && order.Store.Locale != "zh-CHT")
                {
                    errMsg = "Your account has already exists, and you will be directed back to your company. Please wait ...";
                }

                string encodedErrMsg = WebUtility.HtmlEncode(errMsg);
                services.Http.Response.Headers.Append("X-Provision-Error", encodedErrMsg);
                services.Http.Response.Headers.Append(
                    "Location",
                    WebUtility.UrlEncode(services.DataHubOptions.Value.APUrl));

                return TypedResults.Conflict(encodedErrMsg);
            }

            //供裝
            currentOrder = await services.Mediator.Send(new CreateProvisionCommand(customer, affiliate?.Name, orderItem.Sku, expirationDate));

            newtrialPricingCode = orderItem.Sku;
        }
        catch (WarningException ex)
        {
            LogOrderError(order, services.Logger, ex);
        }
        catch (ParameterException ex)
        {
            LogOrderError(order, services.Logger, ex);
            return TypedResults.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            services.Logger.LogError(ex, "供裝失敗");
            return TypedResults.StatusCode(500);
        }

        //供裝成功，回覆供裝中心
        services.Http.Response.Headers.Append(
            "Location",
            services.DataHubOptions.Value.APUrl);

        //取得訂單狀態
        //改用新的試用sku而非固定的sku的
        if (order.Customer is not null)
        {
            //回覆供裝中心租用實體相關資訊
            if (currentOrder is null)
            {
                LogOrderError(order, services.Logger, new InvalidOperationException("系統供裝資料異常"));
                return TypedResults.BadRequest("訂單內容異常");
            }

            string identity = $"{services.DataHubOptions.Value.SkuPrefix}-{currentOrder.OrderId}";
            string activationDate = currentOrder.Sdate.ToString();
            string currentExpirationDate = string.IsNullOrEmpty(expirationDate) ? currentOrder.Edate.ToString() : expirationDate;


            // 為了對應多個採購平台的狀態的續約處理，
            // 請服務開通 HTTP API 於 HTTP 回應表頭中提供所供裝服務的租戶識別碼於自訂表頭 X-Instance-Identity 中
            // 之後在續約時會用來查詢並重導回原採購平台下單
            services.Http.Response.Headers.Append("X-Instance-Identity", HttpUtility.HtmlAttributeEncode(identity));

            // 為了因應促銷需求 (例如：新購客戶可免費增加三個月的使用授權，暨有客戶若續約兩年，可免費增加半年的使用授權)，
            // 供裝任務時將會在 HTTP 請求表頭中送出 X-Instance-ExpirationDate(到期日) 的自訂表頭以延長符合促銷條件的租戶的訂閱到期日，
            // 其值格式應為 ISO 8601 的日期表示，例如：2016-11-30。故服務在開通時若發現此表頭時應以此取代原本的配置做為租戶的訂閱到期日基準。
            services.Http.Response.Headers.Append("X-Instance-ActivationDate", HttpUtility.HtmlAttributeEncode(activationDate));

            // 追踪所供裝租戶租戶的訂閱生效及到期日，
            // 請服務開通 HTTP API 於 HTTP 回應表頭中提供所供裝租戶生效及到期日於自訂表頭 X-Instance-ActivationDate(生效日，若未提供則預設為訂單建立日期)，
            // X-Instance-ExpirationDate(到期日)，其值格式應為 ISO 8601 的日期表示，例如：2016-11-30。
            services.Http.Response.Headers.Append("X-Instance-ExpirationDate", HttpUtility.HtmlAttributeEncode(currentExpirationDate));

            services.Logger.LogInformation
            (
                "已完成供裝中心呼叫供裝API(試用開通) IP:{RemoteIpAddress} OrderId:{Order_Id} CustomerId:{Customer_Id} ",
                services.HttpRequest.HttpContext.Connection.RemoteIpAddress,
                order.Id,
                order.Customer.Id
            );
            services.Logger.LogDebug
            (
                "Location:{APUrl}, X-Instance-Identity:{Identity}, X-Instance-ActivationDate:{ActivationDate}, X-Instance-ExpirationDate:{CurrentExpirationDate}",
                services.DataHubOptions.Value.APUrl,
                identity,
                activationDate,
                currentExpirationDate
            );

            return TypedResults.Ok();
        }

        return TypedResults.BadRequest("訂單內容異常");

    }

    /// <summary>
    /// 供裝中心呼叫供裝API(加購續約開通)
    /// </summary>
    /// <param name="addOns">供裝訂單內容</param>
    /// <param name="services">供裝訂單內容</param>
    /// <returns></returns>
    private static async Task<Results<Ok, BadRequest<string>, StatusCodeHttpResult>> AddProvision(AddOnOrder addOns, [AsParameters] CloudServices services)
    {
        string? expirationDate = string.Empty;

        try
        {
            services.Logger.LogInformation("供裝訂單內容:{AddOns}", addOns);

            if (addOns == null) return TypedResults.BadRequest("參數無法解析");
            if (string.IsNullOrEmpty(addOns.Email) || string.IsNullOrEmpty(addOns.CustomerId)) return TypedResults.BadRequest("參數無法解析");
            //Request Header的ExpirationDate資訊
            expirationDate = services.HttpRequest.Headers.TryGetValue("X-Instance-ExpirationDate", out StringValues value) ? value.First() : string.Empty;

            //供裝
            //await services.ProvisionService.AddProvision(addOns);
        }
        catch (WarningException)
        {
            LogAddOnOrderError(addOns, services.Logger);
        }
        catch (ParameterException ex)
        {
            LogAddOnOrderError(addOns, services.Logger);
            return TypedResults.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            LogAddOnOrderError(addOns, services.Logger);
            services.Logger.LogError(ex, "供裝失敗");
            return TypedResults.StatusCode(500);
        }

        //供裝成功，回覆供裝中心
        services.Http.Response.Headers.Append(
            "Location",
            WebUtility.UrlEncode(services.DataHubOptions.Value.APUrl));

        //取得訂單狀態
        Order? orderInfo = await services.OrderQuery.GetUserOrders(addOns.Email, services.DataHubOptions.Value.TrialSkuPrefix);

        //回覆供裝中心租用實體相關資訊
        if (orderInfo is null)
        {
            LogAddOnOrderError(addOns, services.Logger);
            return TypedResults.BadRequest("訂單內容異常");
        }

        // 為了對應多個採購平台的狀態的續約處理，
        // 請服務開通 HTTP API 於 HTTP 回應表頭中提供所供裝服務的租戶識別碼於自訂表頭 X-Instance-Identity 中
        // 之後在續約時會用來查詢並重導回原採購平台下單
        services.Http.Response.Headers.Append("X-Instance-Identity", HttpUtility.HtmlAttributeEncode($"{services.DataHubOptions.Value.SkuPrefix}-{orderInfo.OrderId}"));

        // 為了因應促銷需求 (例如：新購客戶可免費增加三個月的使用授權，暨有客戶若續約兩年，可免費增加半年的使用授權)，
        // 供裝任務時將會在 HTTP 請求表頭中送出 X-Instance-ExpirationDate(到期日) 的自訂表頭以延長符合促銷條件的租戶的訂閱到期日，
        // 其值格式應為 ISO 8601 的日期表示，例如：2016-11-30。故服務在開通時若發現此表頭時應以此取代原本的配置做為租戶的訂閱到期日基準。
        services.Http.Response.Headers.Append("X-Instance-ActivationDate", HttpUtility.HtmlAttributeEncode(orderInfo.Sdate.ToString()));

        // 追踪所供裝租戶租戶的訂閱生效及到期日，
        // 請服務開通 HTTP API 於 HTTP 回應表頭中提供所供裝租戶生效及到期日於自訂表頭 X-Instance-ActivationDate(生效日，若未提供則預設為訂單建立日期)，
        // X-Instance-ExpirationDate(到期日)，其值格式應為 ISO 8601 的日期表示，例如：2016-11-30。
        services.Http.Response.Headers.Append("X-Instance-ExpirationDate", HttpUtility.HtmlAttributeEncode(string.IsNullOrEmpty(expirationDate) ? orderInfo.Edate.ToString() : expirationDate));


        return TypedResults.Ok();
    }

    /// <summary>
    /// 在 logger 中記錄供裝訂單的內容
    /// </summary>
    /// <param name="order">訂單</param>
    /// <param name="logger">logger紀錄</param>
    private static void LogOrderInfo(OrderDto order, ILogger logger)
    {
        string jsonOrder = JsonSerializer.Serialize(order, jsonSerializerOptions);
        logger.LogInformation("供裝訂單內容:{JsonOrder}", jsonOrder);
    }

    /// <summary>
    /// 在 logger 中記錄供裝訂單的錯誤
    /// </summary>
    /// <param name="order">訂單</param>
    /// <param name="logger">logger紀錄</param>
    /// <param name="exception">例外錯誤</param>
    private static void LogOrderError(OrderDto order, ILogger logger, Exception? exception)
    {
        string jsonOrder = JsonSerializer.Serialize(order, jsonSerializerOptions);
        logger.LogError(exception, "供裝失敗訂單內容:{JsonOrder}", jsonOrder);
    }

    /// <summary>
    /// 從快取中實例化的<see cref="JsonSerializerOptions"/>，避免每次呼叫都要重新建立
    /// </summary>
    private static readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    /// <summary>
    /// 在 logger 中記錄供裝訂單的錯誤
    /// </summary>
    /// <param name="addOns">訂單</param>
    /// <param name="logger">logger紀錄</param>
    private static void LogAddOnOrderError(AddOnOrder addOns, ILogger logger)
    {
        string jsonOrder = JsonSerializer.Serialize(addOns, jsonSerializerOptions);
        logger.LogError("供裝失敗訂單內容:{JsonOrder:lj}", jsonOrder);
    }

    /// <summary>
    /// 從Header中取得指定的參數值
    /// </summary>
    /// <param name="request"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string GetReuestHeaderValue(HttpRequest request, string key)
    {
        if (!request.Headers.TryGetValue(key, out StringValues values)) return string.Empty;
        return values.FirstOrDefault() ?? string.Empty;
    }
}
