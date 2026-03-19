using User.Application.Validation;
using User.Application.Commands;

namespace User.UnitTest;

public class CreateUserCommandValidatorTests
{
    [Fact]
    public void Invalid_email_fails_validation()
    {
        var validator = new CreateUserCommandValidator();
        var result = validator.Validate(new CreateUserCommand("Name", "not-an-email"));
        Assert.False(result.IsValid);
    }
}
