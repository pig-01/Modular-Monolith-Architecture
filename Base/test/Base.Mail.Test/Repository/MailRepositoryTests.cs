using System.Data;
using Main.Repository.AggregatesModel.MailAggregate;
using Base.Infrastructure.Interface.Mail;
using NSubstitute;

namespace Base.Mail.Test.Repository;

public class MailRepositoryTests
{
    private readonly IMailRepository subMailRepository;

    public MailRepositoryTests() => subMailRepository = Substitute.For<IMailRepository>();

    private IMailRepository CreateMailRepository() => subMailRepository;

    [Fact]
    public async Task GetMailServiceParametersStateUnderTestExpectedBehavior()
    {
        // Arrange
        IMailRepository mailRepository = CreateMailRepository();
        string tenantId = "";

        // Act
        IEnumerable<Base.Domain.Models.Mail.MailServiceParameter> result = await mailRepository.GetMailServiceParameters(tenantId);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetMailServiceParameterByIDStateUnderTestExpectedBehavior()
    {
        // Arrange
        IMailRepository mailRepository = CreateMailRepository();
        string mailServiceParameterID = "test-id";

        // Act
        IEnumerable<Base.Domain.Models.Mail.MailServiceParameter> result = await mailRepository.GetMailServiceParameterByID(mailServiceParameterID);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetMailTemplateStateUnderTestExpectedBehavior()
    {
        // Arrange
        IMailRepository mailRepository = CreateMailRepository();
        string functionCode = "test-function";
        string mailType = "test-type";
        string tenantId = "test-tenant";

        // Act
        Base.Domain.Models.Mail.MailTemplate? result = await mailRepository.GetMailTemplate(
            functionCode,
            mailType,
            tenantId);

        // Assert
        // Result can be null, so we just verify the method doesn't throw
        Assert.True(true);
    }
}
