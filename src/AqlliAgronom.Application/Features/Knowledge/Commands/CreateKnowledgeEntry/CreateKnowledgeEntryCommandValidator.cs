using FluentValidation;

namespace AqlliAgronom.Application.Features.Knowledge.Commands.CreateKnowledgeEntry;

public class CreateKnowledgeEntryCommandValidator : AbstractValidator<CreateKnowledgeEntryCommand>
{
    public CreateKnowledgeEntryCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(500);
        RuleFor(x => x.CropName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ProblemName).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Symptoms).NotEmpty().MaximumLength(5000);
        RuleFor(x => x.DetailedExplanation).NotEmpty().MaximumLength(20000);
        RuleFor(x => x.RecommendedProducts).NotEmpty().MaximumLength(5000);
        RuleFor(x => x.DosagePerHectare).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.ApplicationInstructions).NotEmpty().MaximumLength(5000);
        RuleFor(x => x.SafetyPrecautions).NotEmpty().MaximumLength(5000);
    }
}
