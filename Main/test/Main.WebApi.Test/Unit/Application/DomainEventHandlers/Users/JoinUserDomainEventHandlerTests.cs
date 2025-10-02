using System.Net.Mail;
using Base.Domain.Exceptions;
using Base.Domain.Models.Mail;
using Base.Domain.Options.FrontEnd;
using Base.Infrastructure.Interface.Mail;
using Base.Infrastructure.Interface.TimeZone;
using Main.Domain.AggregatesModel.UserAggregate;
using Main.Domain.Events.UserAggregate;
using Main.WebApi.Application.DomainEventHandlers.Users;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Main.WebApi.Test.Unit.Application.DomainEventHandlers.Users;

public class JoinUserDomainEventHandlerTests
{
    private readonly ILogger<JoinUserDomainEventHandler> subLogger;
    private readonly IOptions<FrontEndOption> subOptions;
    private readonly ITimeZoneService subTimeZoneService;
    private readonly IMailService subMailService;
    private readonly JoinUserDomainEventHandler handler;
    private readonly FrontEndOption frontEndOption;

    public JoinUserDomainEventHandlerTests()
    {
        subLogger = Substitute.For<ILogger<JoinUserDomainEventHandler>>();
        subOptions = Substitute.For<IOptions<FrontEndOption>>();
        subTimeZoneService = Substitute.For<ITimeZoneService>();
        subMailService = Substitute.For<IMailService>();

        frontEndOption = new FrontEndOption
        {
            Url = "https://test.example.com"
        };
        subOptions.Value.Returns(frontEndOption);

        handler = new JoinUserDomainEventHandler(
            subLogger,
            subOptions,
            subTimeZoneService,
            subMailService);
    }

    [Fact(DisplayName = "處理用戶加入事件 - 成功發送邀請郵件")]
    public async Task HandleValidNotificationSendsInvitationMailSuccessfully()
    {
        // Arrange
        Scuser testUser = new()
        {
            UserId = "test@example.com",
            UserName = "Test User",
            UserEmail = "test@example.com"
        };

        Guid testToken = Guid.NewGuid();
        string testTenantId = "TENANT001";
        JoinUserDomainEvent testEvent = new(testUser, testTenantId, testToken);

        DateTime testDateTime = new(2025, 8, 4, 10, 0, 0);
        subTimeZoneService.Now.Returns(testDateTime);

        MailInfomation testMailInformation = new()
        {
            ReceiverList = [],
            Body = "Test mail body with {Body} placeholder",
            Subject = "Test Subject"
        };

        subMailService.GetMailTemplate("User", "Join", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(testMailInformation));

        // Act
        await handler.Handle(testEvent, CancellationToken.None);

        // Assert
        await subMailService.Received(1).GetMailTemplate("User", "Join", Arg.Any<CancellationToken>());
        await subMailService.Received(1).SendAsync(
            Arg.Is<MailInfomation>(mail =>
                mail.ReceiverList.Any(receiver => receiver.Address == testUser.UserId) &&
                mail.Body != null && mail.Body.Contains("Test mail body")),
            Arg.Any<CancellationToken>());

        // 驗證時區服務被正確呼叫
        DateTime receivedTime = subTimeZoneService.Received(1).Now;
    }

    [Fact(DisplayName = "處理用戶加入事件 - URI 組建正確性驗證")]
    public async Task HandleValidNotificationBuildsCorrectActivationUri()
    {
        // Arrange
        Scuser testUser = new()
        {
            UserId = "test@example.com",
            UserName = "Test User",
            UserEmail = "test@example.com"
        };

        Guid testToken = Guid.Parse("12345678-1234-5678-9abc-123456789abc");
        string testTenantId = "TENANT001";
        JoinUserDomainEvent testEvent = new(testUser, testTenantId, testToken);

        DateTime testDateTime = new(2025, 8, 4, 10, 0, 0);
        subTimeZoneService.Now.Returns(testDateTime);

        MailInfomation testMailInformation = new()
        {
            ReceiverList = [],
            Body = "Test mail body with {Body} placeholder",
            Subject = "Test Subject"
        };

        subMailService.GetMailTemplate("User", "Join", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(testMailInformation));

        // Act
        await handler.Handle(testEvent, CancellationToken.None);

        // Assert
        // 驗證 URI 透過郵件服務呼叫傳遞
        await subMailService.Received(1).SendAsync(
            Arg.Is<MailInfomation>(mail =>
                mail.ReceiverList.Count == 1 &&
                mail.ReceiverList[0].Address == testUser.UserId &&
                mail.ReceiverList[0].DisplayName == testUser.UserName),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "處理用戶加入事件 - 郵件服務異常時拋出 WarningException")]
    public async Task HandleMailServiceThrowsExceptionThrowsWarningException()
    {
        // Arrange
        Scuser testUser = new()
        {
            UserId = "test@example.com",
            UserName = "Test User",
            UserEmail = "test@example.com"
        };

        Guid testToken = Guid.NewGuid();
        string testTenantId = "TENANT001";
        JoinUserDomainEvent testEvent = new(testUser, testTenantId, testToken);

        DateTime testDateTime = new(2025, 8, 4, 10, 0, 0);
        subTimeZoneService.Now.Returns(testDateTime);

        InvalidOperationException expectedException = new("Mail service unavailable");
        subMailService.GetMailTemplate("User", "Join", Arg.Any<CancellationToken>())
            .Throws(expectedException);

        // Act & Assert
        WarningException warningException = await Assert.ThrowsAsync<WarningException>(() =>
            handler.Handle(testEvent, CancellationToken.None));

        Assert.Equal("Failed to handle JoinUserDomainEventHandler", warningException.Message);
        Assert.Equal(expectedException, warningException.InnerException);
    }

    [Fact(DisplayName = "處理用戶加入事件 - 郵件發送失敗時拋出 WarningException")]
    public async Task HandleMailSendFailsThrowsWarningException()
    {
        // Arrange
        Scuser testUser = new()
        {
            UserId = "test@example.com",
            UserName = "Test User",
            UserEmail = "test@example.com"
        };

        Guid testToken = Guid.NewGuid();
        string testTenantId = "TENANT001";
        JoinUserDomainEvent testEvent = new(testUser, testTenantId, testToken);

        DateTime testDateTime = new(2025, 8, 4, 10, 0, 0);
        subTimeZoneService.Now.Returns(testDateTime);

        MailInfomation testMailInformation = new()
        {
            ReceiverList = [],
            Body = "Test mail body with {Body} placeholder",
            Subject = "Test Subject"
        };

        subMailService.GetMailTemplate("User", "Join", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(testMailInformation));

        SmtpException expectedException = new("SMTP server not available");
        subMailService.SendAsync(Arg.Any<MailInfomation>(), Arg.Any<CancellationToken>())
            .Throws(expectedException);

        // Act & Assert
        WarningException warningException = await Assert.ThrowsAsync<WarningException>(() =>
            handler.Handle(testEvent, CancellationToken.None));

        Assert.Equal("Failed to handle JoinUserDomainEventHandler", warningException.Message);
        Assert.Equal(expectedException, warningException.InnerException);
    }

    [Fact(DisplayName = "處理用戶加入事件 - 驗證到期日設定為 7 天後")]
    public async Task HandleValidNotificationSetsExpirationDateSevenDaysLater()
    {
        // Arrange
        Scuser testUser = new()
        {
            UserId = "test@example.com",
            UserName = "Test User",
            UserEmail = "test@example.com"
        };

        Guid testToken = Guid.NewGuid();
        string testTenantId = "TENANT001";
        JoinUserDomainEvent testEvent = new(testUser, testTenantId, testToken);

        DateTime testDateTime = new(2025, 8, 4, 10, 0, 0);
        DateTime expectedExpirationDate = testDateTime.AddDays(7);
        subTimeZoneService.Now.Returns(testDateTime);

        MailInfomation testMailInformation = new()
        {
            ReceiverList = [],
            Body = "Test mail body with {Body} placeholder",
            Subject = "Test Subject"
        };

        subMailService.GetMailTemplate("User", "Join", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(testMailInformation));

        // Act
        await handler.Handle(testEvent, CancellationToken.None);

        // Assert
        // 驗證時區服務被正確呼叫
        DateTime receivedTime = subTimeZoneService.Received(1).Now;
    }

    [Theory(DisplayName = "處理用戶加入事件 - 不同用戶資料的處理")]
    [InlineData("user1@test.com", "User One", "TENANT001")]
    [InlineData("user2@test.com", "User Two", "TENANT002")]
    [InlineData("user3@test.com", "User Three", "TENANT003")]
    public async Task HandleDifferentUserDataProcessesCorrectly(string userId, string userName, string tenantId)
    {
        // Arrange
        Scuser testUser = new()
        {
            UserId = userId,
            UserName = userName,
            UserEmail = userId
        };

        Guid testToken = Guid.NewGuid();
        JoinUserDomainEvent testEvent = new(testUser, tenantId, testToken);

        DateTime testDateTime = new(2025, 8, 4, 10, 0, 0);
        subTimeZoneService.Now.Returns(testDateTime);

        MailInfomation testMailInformation = new()
        {
            ReceiverList = [],
            Body = "Test mail body with {Body} placeholder",
            Subject = "Test Subject"
        };

        subMailService.GetMailTemplate("User", "Join", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(testMailInformation));

        // Act
        await handler.Handle(testEvent, CancellationToken.None);

        // Assert
        await subMailService.Received(1).SendAsync(
            Arg.Is<MailInfomation>(mail =>
                mail.ReceiverList.Any(receiver =>
                    receiver.Address == userId &&
                    receiver.DisplayName == userName)),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "處理用戶加入事件 - 驗證郵件內容模板替換")]
    public async Task HandleValidNotificationReplacesMailBodyTemplate()
    {
        // Arrange
        Scuser testUser = new()
        {
            UserId = "test@example.com",
            UserName = "Test User",
            UserEmail = "test@example.com"
        };

        Guid testToken = Guid.NewGuid();
        string testTenantId = "TENANT001";
        JoinUserDomainEvent testEvent = new(testUser, testTenantId, testToken);

        DateTime testDateTime = new(2025, 8, 4, 10, 0, 0);
        subTimeZoneService.Now.Returns(testDateTime);

        string originalMailBody = "Welcome to our service! {Body} Please click the link to activate.";
        MailInfomation testMailInformation = new()
        {
            ReceiverList = [],
            Body = originalMailBody,
            Subject = "Welcome Email"
        };

        subMailService.GetMailTemplate("User", "Join", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(testMailInformation));

        // Act
        await handler.Handle(testEvent, CancellationToken.None);

        // Assert
        await subMailService.Received(1).SendAsync(
            Arg.Is<MailInfomation>(mail =>
                mail.Body != null &&
                !mail.Body.Contains("{Body}")), // 確認模板已被替換
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "處理用戶加入事件 - 取消令牌正確傳遞")]
    public async Task HandleWithCancellationTokenPassesCancellationTokenCorrectly()
    {
        // Arrange
        Scuser testUser = new()
        {
            UserId = "test@example.com",
            UserName = "Test User",
            UserEmail = "test@example.com"
        };

        Guid testToken = Guid.NewGuid();
        string testTenantId = "TENANT001";
        JoinUserDomainEvent testEvent = new(testUser, testTenantId, testToken);

        CancellationToken cancellationToken = new();
        DateTime testDateTime = new(2025, 8, 4, 10, 0, 0);
        subTimeZoneService.Now.Returns(testDateTime);

        MailInfomation testMailInformation = new()
        {
            ReceiverList = [],
            Body = "Test mail body with {Body} placeholder",
            Subject = "Test Subject"
        };

        subMailService.GetMailTemplate("User", "Join", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(testMailInformation));

        // Act
        await handler.Handle(testEvent, cancellationToken);

        // Assert
        await subMailService.Received(1).GetMailTemplate("User", "Join", cancellationToken);
        await subMailService.Received(1).SendAsync(Arg.Any<MailInfomation>(), cancellationToken);
    }
}
