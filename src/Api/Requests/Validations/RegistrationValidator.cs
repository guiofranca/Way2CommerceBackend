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
        if (!password.Any(p => char.IsUpper(p))) return false;
        if (!password.Any(p => char.IsLower(p))) return false;
        if (!password.Any(p => char.IsDigit(p))) return false;
        if (!password.Any(p => !char.IsLetterOrDigit(p))) return false;

        return true;
    }

    private async Task<bool> NotBeTaken(string email, CancellationToken arg2)
    {
        return !await _userRepository.EmailIsTakenAsync(email);
    }
}
