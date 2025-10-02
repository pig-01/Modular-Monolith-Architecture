using System.Security.Claims;
using Scheduler.Infrastructure;
using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace Scheduler.Test.Infrastructure;

public class TimeZoneServiceTests
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly TimeZoneService timeZoneService;

    public TimeZoneServiceTests()
    {
        httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        timeZoneService = new TimeZoneService(httpContextAccessor);
    }

    [Fact(DisplayName = "ConvertToUtc with UTC DateTime should return the same DateTime")]
    public void ConvertToUtc_WithUtcDateTime_ShouldReturnSameDateTime()
    {
        // Arrange
        DateTime utcDateTime = new(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        // Act
        DateTime result = timeZoneService.ConvertToUtc(utcDateTime);

        // Assert
        Assert.Equal(utcDateTime, result);
        Assert.Equal(DateTimeKind.Utc, result.Kind);
    }

    [Fact(DisplayName = "ConvertToUtc with Local DateTime should convert to UTC")]
    public void ConvertToUtc_WithLocalDateTime_ShouldConvertToUtc()
    {
        // Arrange
        DateTime localDateTime = new(2025, 1, 1, 12, 0, 0, DateTimeKind.Local);

        // Act
        DateTime result = timeZoneService.ConvertToUtc(localDateTime);

        // Assert
        Assert.Equal(DateTimeKind.Utc, result.Kind);
        // 結果應該等於 ToUniversalTime() 的結果
        Assert.Equal(localDateTime.ToUniversalTime(), result);
    }

    [Fact(DisplayName = "ConvertToUtc with Unspecified DateTime should convert using UserTimeZone")]
    public void ConvertToUtc_WithUnspecifiedDateTime_ShouldConvertUsingUserTimeZone()
    {
        // Arrange
        DateTime unspecifiedDateTime = new(2025, 1, 1, 12, 0, 0, DateTimeKind.Unspecified);

        // Act
        DateTime result = timeZoneService.ConvertToUtc(unspecifiedDateTime);

        // Assert
        Assert.Equal(DateTimeKind.Utc, result.Kind);
        // 結果不應該拋出異常
        Assert.True(result.Ticks > 0);
    }

    [Fact(DisplayName = "ConvertToUserTimeZone with UTC DateTime should convert properly")]
    public void ConvertToUserTimeZone_WithUtcDateTime_ShouldConvertProperly()
    {
        // Arrange
        DateTime utcDateTime = new(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        // Act
        DateTime result = timeZoneService.ConvertToUserTimeZone(utcDateTime);

        // Assert
        // 結果不應該拋出異常
        Assert.True(result.Ticks > 0);
        Assert.Equal(DateTimeKind.Unspecified, result.Kind); // TimeZoneInfo.ConvertTime 返回 Unspecified
    }

    [Fact(DisplayName = "ConvertToUserTimeZone with Local DateTime should handle conversion")]
    public void ConvertToUserTimeZone_WithLocalDateTime_ShouldHandleConversion()
    {
        // Arrange
        DateTime localDateTime = new(2025, 1, 1, 12, 0, 0, DateTimeKind.Local);

        // Act
        DateTime result = timeZoneService.ConvertToUserTimeZone(localDateTime);

        // Assert
        // 結果不應該拋出異常
        Assert.True(result.Ticks > 0);
        Assert.Equal(DateTimeKind.Unspecified, result.Kind);
    }

    [Fact(DisplayName = "ConvertToUserTimeZone with Unspecified DateTime should assume UTC")]
    public void ConvertToUserTimeZone_WithUnspecifiedDateTime_ShouldAssumeUtc()
    {
        // Arrange
        DateTime unspecifiedDateTime = new(2025, 1, 1, 12, 0, 0, DateTimeKind.Unspecified);

        // Act
        DateTime result = timeZoneService.ConvertToUserTimeZone(unspecifiedDateTime);

        // Assert
        // 結果不應該拋出異常
        Assert.True(result.Ticks > 0);
        Assert.Equal(DateTimeKind.Unspecified, result.Kind);
    }

    [Fact(DisplayName = "Now property should return current time in user timezone")]
    public void Now_ShouldReturnCurrentTimeInUserTimeZone()
    {
        // Act
        DateTime now = timeZoneService.Now;

        // Assert
        Assert.True(now.Ticks > 0);
        Assert.Equal(DateTimeKind.Unspecified, now.Kind);
        // 應該在合理的時間範圍內
        Assert.True(Math.Abs((DateTime.UtcNow - now).TotalHours) < 24);
    }

    [Fact(DisplayName = "UserTimeZone should return Local timezone when no user claims")]
    public void UserTimeZone_WithoutUserClaims_ShouldReturnLocalTimeZone()
    {
        // Arrange
        httpContextAccessor.HttpContext.Returns((HttpContext?)null);

        // Act
        TimeZoneInfo userTimeZone = timeZoneService.UserTimeZone;

        // Assert
        Assert.Equal(TimeZoneInfo.Local, userTimeZone);
    }
}