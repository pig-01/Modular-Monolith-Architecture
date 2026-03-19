using Product.Application.Validation;
using Product.Application.Commands;

namespace Product.UnitTest;

public class CreateProductCommandValidatorTests
{
    [Fact]
    public void Price_must_be_positive()
    {
        var validator = new CreateProductCommandValidator();
        var result = validator.Validate(new CreateProductCommand("Item", 0));
        Assert.False(result.IsValid);
    }
}
