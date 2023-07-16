using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using FluentValidation;

namespace SipayApi.Controllers;

public class PersonWithFluentValidation
{
    [DisplayName("Staff person name")]
    public string Name { get; set; }
    
    [DisplayName("Staff person lastname")]
    public string Lastname { get; set; }
    
    [DisplayName("Staff person phone number")]
    public string Phone { get; set; }
    
    [DisplayName("Staff person access level to system")]
    public int AccessLevel { get; set; }
    
    [DisplayName("Staff person salary")]
    //[SalaryAttribute]
    public decimal Salary { get; set; }
}

public class PersonWithFluentValidationValidator : AbstractValidator<PersonWithFluentValidation>
{
    public PersonWithFluentValidationValidator()
    {
        RuleFor(person => person.Name)
            .NotNull()
            .MinimumLength(5)
            .MaximumLength(100);

        RuleFor(person => person.Lastname)
            .NotNull()
            .MinimumLength(5)
            .MaximumLength(100);

        RuleFor(person => person.Phone)
            .NotNull()
            .Length(11)
            .Matches(@"^\d{11}$"); // 11 haneli rakam olmalı anlamına geliyor.

        RuleFor(person => person.AccessLevel)
            .NotNull()
            .InclusiveBetween(1, 5);

        RuleFor(person => person.Salary)
            .NotNull()
            .InclusiveBetween(5000, 50000);

        RuleFor(person => person)
            .Must(x => x.Salary > 10000).When(x => x.AccessLevel == 1)
            .Must(x => x.Salary > 20000).When(x => x.AccessLevel == 2)
            .Must(x => x.Salary > 30000).When(x => x.AccessLevel == 3)
            .Must(x => x.Salary > 40000).When(x => x.AccessLevel == 4);
    }
}

[ApiController]
[Route("sipy/api/[controller]")]
public class PersonWithFluentValidationController : ControllerBase
{
    public PersonWithFluentValidationController() { }

    [HttpPost]
    public IResult Post([FromBody] PersonWithFluentValidation person)
    {
        var validator = new PersonWithFluentValidationValidator();

        var result = validator.Validate(person);

        if (result.IsValid)
        {
            return Results.Ok(person);
        }

        var message = string.Empty;

        foreach (var error in result.Errors)
        {
            message += error.ErrorMessage + Environment.NewLine;
        }

        if (!string.IsNullOrWhiteSpace(message))
            return Results.BadRequest(message);

        return Results.BadRequest();
    }
}
