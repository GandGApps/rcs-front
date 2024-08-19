using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Kassa.Shared.RcsLocator.Analyzer;
public static class Diagnostics
{
    public static readonly DiagnosticDescriptor NotFoundConstructor = new(
        id: "RCSLOCATOR001",
        title: "Constructor not found",
        messageFormat: "Constructor not found",
        category: "RCSLOCATOR",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Constructor not found. Please ensure the constructor is either marked with the appropriate attribute or is the only constructor.");
}
