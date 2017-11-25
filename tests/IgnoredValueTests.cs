
using System;
using System.Collections.Immutable;
using CsFunAnalyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;
using Xunit;

namespace CsFunAnalyzers
{
    public class IgnoredValueTests : DiagnosticVerifier
    {
        private static readonly MetadataReference s_corlibReference = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new CsFunAnalyzer.IgnoredValueAnalyzer();
        }

        [Fact]
        public void UnitTest()
        {
            var src = @"
class C
{
    int M() => 0;
    
    void M2()
    {
        M2(); // fine, void returning method
        M(); // warning, ignore with '_ ='
        _ = M(); // fine
    }
}";
            VerifyCSharpDiagnostic(src,
                // Test0.cs(9,9): warning IgnoredValue: Value of expression is being ignored. Assign result with '_ = ' if you mean to ignore the value
                GetCSharpResultAt(IgnoredValueAnalyzer.IgnoredValueDiagnosticRule, 9, 9));
        }
    }
}