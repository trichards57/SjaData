// <copyright file="NewHoursValidator.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentValidation;
using SjaData.Model;
using SjaData.Model.Hours;

namespace SjaData.Server.Validation;

/// <summary>
/// Validator for new hours entries.
/// </summary>
public class NewHoursValidator : AbstractValidator<NewHoursEntry>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NewHoursValidator"/> class.
    /// </summary>
    public NewHoursValidator()
    {
        RuleFor(x => x.Date).NotEmpty().WithMessage("The date must be provided.");
        RuleFor(x => x.Region).IsInEnum().WithMessage("The region must be a valid value.");
        RuleFor(x => x.Trust).IsInEnum().WithMessage("The trust must be a valid value.");
        RuleFor(x => x.Hours).GreaterThan(TimeSpan.Zero).WithMessage("The hours must be greater than zero.");
        RuleFor(x => x.PersonId).GreaterThan(0).WithMessage("The person ID must be provided.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("The name must be provided.")
            .MaximumLength(100).WithMessage("The name must be no more than 100 characters.");
        RuleFor(x => x).Custom((x, context) =>
        {
            if (x.Region == Region.Undefined && x.Trust == Trust.Undefined)
            {
                context.AddFailure("region", "The region must be provided if the trust is not provided.");
                context.AddFailure("trust", "The trust must be provided if the region is not provided.");
            }
        });
    }
}
