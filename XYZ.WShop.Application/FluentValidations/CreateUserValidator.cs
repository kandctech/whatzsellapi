using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYZ.WShop.Application.Dtos.User;

namespace XYZ.WShop.Application.FluentValidations
{
    public class CreateUserValidator: AbstractValidator<CreateUserRequest>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .NotNull();

        }
    }
}
