// <copyright file="WeibullGenerator.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaInNumbers.Client.Maths;

public class WeibullGenerator(double shape, double scale)
{
    private static readonly ThreadLocal<Random> ThreadLocalRandom = new(() => new Random(Guid.NewGuid().GetHashCode()));
    private readonly double scale = scale;
    private readonly double shape = shape;

    private static Random RandomInstance => ThreadLocalRandom.Value!;

    public int Generate()
    {
        double u = RandomInstance.NextDouble();
        var val = scale * Math.Pow(-Math.Log(1 - u), 1 / shape);

        return (int)Math.Round(val);
    }
}
