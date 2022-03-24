using Api.Controllers;
using Api.Requests.Product;
using Domain.Repositories.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mysql.Identity;

namespace Api.Requests.Validations;
public class RegistrationValidator : AbstractValidator<AuthController.RegisterRequest>
{
    protected readonly IUserRepository<ApplicationUser> _userRepository;
    protected readonly PasswordValidator<ApplicationUser> _passwordValidator;

    public RegistrationValidator(IUserRepository<ApplicationUser> userRepository)
    {
        _userRepository = userRepository;

        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3);

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .EmailAddress()
            .NotEmpty()
            .MustAsync(NotBeTaken);

        RuleFor(x => x.Password)
            .NotEmpty()
            .NotNull()
            .MinimumLength(8)
            .Equal(x => x.PasswordConfirmation)
            .WithMessage("Passwords doesn't match")
            .NotEmpty()
            .Must(BeStrong)
            .WithMessage("Password too weak");
    }

    private bool BeStrong(string password)
    {
        if (password == null) return false;
        if (password.Count(p => char.IsUpper(p)) == 0) return false;
        if (password.Count(p => char.IsLower(p)) == 0) return false;
        if (password.Count(p => char.IsDigit(p)) == 0) return false;
        if (password.Count(p => !char.IsLetterOrDigit(p)) == 0) return false;

        return true;
    }

    private async Task<bool> NotBeTaken(string email, CancellationToken arg2)
    {
        return !(await _userRepository.EmailIsTakenAsync(email));
    }
}
