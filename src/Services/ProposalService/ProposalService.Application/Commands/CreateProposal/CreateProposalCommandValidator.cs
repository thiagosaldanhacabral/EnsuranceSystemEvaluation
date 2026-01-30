using FluentValidation;

namespace ProposalService.Application.Commands.CreateProposal;

/// <summary>
/// Validator for CreateProposalCommand
/// </summary>
public sealed class CreateProposalCommandValidator : AbstractValidator<CreateProposalCommand>
{
    public CreateProposalCommandValidator()
    {
        RuleFor(x => x.CustomerName)
            .NotEmpty().WithMessage("Customer name is required")
            .MaximumLength(200).WithMessage("Customer name cannot exceed 200 characters");

        RuleFor(x => x.CustomerCPF)
            .NotEmpty().WithMessage("CPF is required")
            .Must(BeValidCPF).WithMessage("Invalid CPF format");

        RuleFor(x => x.InsuranceValue)
            .GreaterThan(0).WithMessage("Insurance value must be greater than zero")
            .LessThanOrEqualTo(1_000_000).WithMessage("Insurance value cannot exceed 1,000,000");
    }

    private static bool BeValidCPF(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        // Remove formatting
        var cleanCpf = new string(cpf.Where(char.IsDigit).ToArray());

        if (cleanCpf.Length != 11)
            return false;

        // Check for known invalid CPFs
        if (cleanCpf.Distinct().Count() == 1)
            return false;

        // Validate check digits
        var sum = 0;
        for (int i = 0; i < 9; i++)
            sum += (cleanCpf[i] - '0') * (10 - i);

        var remainder = sum % 11;
        var firstCheckDigit = remainder < 2 ? 0 : 11 - remainder;

        if (firstCheckDigit != (cleanCpf[9] - '0'))
            return false;

        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += (cleanCpf[i] - '0') * (11 - i);

        remainder = sum % 11;
        var secondCheckDigit = remainder < 2 ? 0 : 11 - remainder;

        return secondCheckDigit == (cleanCpf[10] - '0');
    }
}
