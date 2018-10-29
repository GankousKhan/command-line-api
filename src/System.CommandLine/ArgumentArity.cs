﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;

namespace System.CommandLine
{
    public class ArgumentArity : IArgumentArity
    {
        public ArgumentArity(int minimumNumberOfArguments, int maximumNumberOfArguments)
        {
            if (minimumNumberOfArguments < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minimumNumberOfArguments));
            }

            if (maximumNumberOfArguments < minimumNumberOfArguments)
            {
                throw new ArgumentException($"{nameof(maximumNumberOfArguments)} must be greater than or equal to {nameof(minimumNumberOfArguments)}");
            }

            MinimumNumberOfArguments = minimumNumberOfArguments;
            MaximumNumberOfArguments = maximumNumberOfArguments;
        }

        public int MinimumNumberOfArguments { get; set; }

        public int MaximumNumberOfArguments { get; set; }

        internal static FailedArgumentArityResult Validate(
            SymbolResult symbolResult,
            int minimumNumberOfArguments,
            int maximumNumberOfArguments)
        {
            if (symbolResult.Arguments.Count < minimumNumberOfArguments)
            {
                return new FailedArgumentArityResult(symbolResult.ValidationMessages.RequiredArgumentMissing(symbolResult));
            }

            if (symbolResult.Arguments.Count > maximumNumberOfArguments)
            {
                if (maximumNumberOfArguments == 1)
                {
                    return new FailedArgumentArityResult(symbolResult.ValidationMessages.ExpectsOneArgument(symbolResult));
                }
                else
                {
                    return new FailedArgumentArityResult(symbolResult.ValidationMessages.ExpectsFewerArguments(symbolResult, maximumNumberOfArguments));
                }
            }

            return null;
        }

        public static IArgumentArity Zero { get; } = new ArgumentArity(0, 0);

        public static IArgumentArity ZeroOrOne { get; } = new ArgumentArity(0, 1);

        public static IArgumentArity ExactlyOne { get; } = new ArgumentArity(1, 1);

        public static IArgumentArity ZeroOrMore { get; } = new ArgumentArity(0, int.MaxValue);

        public static IArgumentArity OneOrMore { get; } = new ArgumentArity(1, int.MaxValue);

        public static IArgumentArity DefaultForType(Type type)
        {
            if (typeof(IEnumerable).IsAssignableFrom(type) &&
                type != typeof(string))
            {
                return OneOrMore;
            }

            if (type == typeof(bool))
            {
                return ZeroOrOne;
            }

            return ExactlyOne;
        }
    }
}
