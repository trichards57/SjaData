// <copyright file="DataValidator.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace SjaData.Server.Api;

public static class DataValidator
{
    public static RouteHandlerBuilder Validate<T>(this RouteHandlerBuilder builder, bool firstErrorOnly = true)
        where T : struct
    {
        builder.AddEndpointFilter(async (context, next) =>
        {
            var argument = context.Arguments.OfType<T>().First();
            var valContext = new ValidationContext(argument);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(argument, valContext, results, true);

            if (!isValid)
            {
                var errors = results
                    .SelectMany(vr => vr.MemberNames.Select(mn => new { mn, ErrorMessage = vr.ErrorMessage ?? string.Empty }))
                    .GroupBy(x => x.mn, x => x.ErrorMessage)
                    .ToDictionary(g => g.Key, g => g.ToArray());

                return Results.ValidationProblem(errors, "The request is invalid. Please correct the errors and try again.");
            }

            return await next(context);
        });

        return builder;
    }
}
