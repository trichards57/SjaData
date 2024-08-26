// <copyright file="NewPatientValidator.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentValidation;
using SjaData.Model;
using SjaData.Model.Patient;

namespace SjaData.Server.Validation;

/// <summary>
/// Validator for new patient entries.
/// </summary>
public class NewPatientValidator : AbstractValidator<NewPatient>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NewPatientValidator"/> class.
    /// </summary>
    public NewPatientValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("The ID must be provided.");
        RuleFor(x => x.Date).NotEmpty().WithMessage("The date must be provided.");
        RuleFor(x => x.CallSign).NotEmpty().WithMessage("The call sign must be provided.")
            .MaximumLength(10).WithMessage("The call sign must be no more than 10 characters.");
        RuleFor(x => x.EventType).IsInEnum().WithMessage("The event type must be a valid value.");
        RuleFor(x => x.Region).IsInEnum().WithMessage("The region must be a valid value.");
        RuleFor(x => x.Trust).IsInEnum().WithMessage("The trust must be a valid value.");
        RuleFor(x => x.PresentingComplaint).MaximumLength(100).WithMessage("The presenting complaint must be no more than 100 characters.");
        RuleFor(x => x.FinalClinicalImpression).MaximumLength(100).WithMessage("The final clinical impression must be no more than 100 characters.");
        RuleFor(x => x.Outcome).IsInEnum().WithMessage("The outcome must be a valid value.");
        RuleFor(x => x).Custom((x, context) =>
        {
            if (x.EventType == EventType.NHSAmbOps && x.Trust == Trust.Undefined)
            {
                context.AddFailure(nameof(x.Trust), "The trust must be provided for NHS Ambulance Operations.");
            }
            else if (x.EventType != EventType.NHSAmbOps && x.Trust != Trust.Undefined)
            {
                context.AddFailure(nameof(x.Trust), "The trust must not be provided for non-NHS Ambulance Operations.");
            }
        });
        RuleFor(x => x).Custom((x, context) =>
        {
            if (x.EventType == EventType.NHSAmbOps && x.Region != Region.Undefined)
            {
                context.AddFailure(nameof(x.Region), "The region must not be provided for NHS Ambulance Operations.");
            }
            else if (x.EventType != EventType.NHSAmbOps && x.Region == Region.Undefined)
            {
                context.AddFailure(nameof(x.Region), "The region must be provided for non-NHS Ambulance Operations.");
            }
        });
    }
}
