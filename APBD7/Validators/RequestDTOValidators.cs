using APBD7.DTOs;
using FluentValidation;

namespace APBD7.Validators;

public class RequestDTOValidators : AbstractValidator<RequestDTO>
{
    public RequestDTOValidators()
    {
        RuleFor(e => e.IdProduct).NotNull();
        RuleFor(e => e.IdWarehouse).NotNull();
        RuleFor(e => e.Amount).GreaterThan(0).NotNull();
        RuleFor(e => e.CreatedAt).NotEmpty().NotNull();
    }
}