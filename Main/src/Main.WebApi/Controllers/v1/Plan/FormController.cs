using Main.WebApi.Application.Commands.Bizform.Forms;

namespace Main.WebApi.Controllers.v1.Plan;

public class FormController(IMediator mediator) : BaseController
{
    [HttpGet("")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> Form([FromQuery] GetFormsCommand command) => ActionResultBuilder(await mediator.Send(command));
}
