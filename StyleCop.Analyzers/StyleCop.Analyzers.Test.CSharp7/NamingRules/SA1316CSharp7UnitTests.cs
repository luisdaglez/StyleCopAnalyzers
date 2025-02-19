﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Lightup;
    using StyleCop.Analyzers.NamingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1316TupleElementNamesShouldUseCorrectCasing,
        StyleCop.Analyzers.NamingRules.SA1316CodeFixProvider>;

    /// <summary>
    /// This class contains the CSharp 7.x unit tests for SA1316.
    /// </summary>
    /// <seealso cref="SA1316TupleElementNamesShouldUseCorrectCasing"/>
    /// <seealso cref="SA1316CodeFixProvider"/>
    public class SA1316CSharp7UnitTests
    {
        private const string DefaultTestSettings = @"
{
  ""settings"": {
  }
}
";

        private const string CamelCaseTestSettings = @"
{
  ""settings"": {
    ""namingRules"": {
      ""tupleElementNameCasing"": ""camelCase""
    }
  }
}
";

        private const string PascalCaseTestSettings = @"
{
  ""settings"": {
    ""namingRules"": {
      ""tupleElementNameCasing"": ""PascalCase""
    }
  }
}
";

        private const string CamelCaseInferredTestSettings = @"
{
  ""settings"": {
    ""namingRules"": {
      ""includeInferredTupleElementNames"" : true,
      ""tupleElementNameCasing"": ""camelCase""
    }
  }
}
";

        private const string PascalCaseInferredTestSettings = @"
{
  ""settings"": {
    ""namingRules"": {
      ""includeInferredTupleElementNames"" : true,
      ""tupleElementNameCasing"": ""PascalCase""
    }
  }
}
";

        private const string CamelCaseExplicitOnlyTestSettings = @"
{
  ""settings"": {
    ""namingRules"": {
      ""includeInferredTupleElementNames"" : false,
      ""tupleElementNameCasing"": ""camelCase""
    }
  }
}
";

        private const string PascalCaseExplicitOnlyTestSettings = @"
{
  ""settings"": {
    ""namingRules"": {
      ""includeInferredTupleElementNames"" : false,
      ""tupleElementNameCasing"": ""PascalCase""
    }
  }
}
";

        /// <summary>
        /// Validates the properly named tuple element names will not produce diagnostics.
        /// </summary>
        /// <param name="settings">The test settings to use.</param>
        /// <param name="tupleElementName1">The expected tuple element name for the first field.</param>
        /// <param name="tupleElementName2">The expected tuple element name for the second field.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(DefaultTestSettings, "ElementName1", "ElementName2")]
        [InlineData(CamelCaseTestSettings, "elementName1", "elementName2")]
        [InlineData(PascalCaseTestSettings, "ElementName1", "ElementName2")]
        public async Task ValidateProperCasedTupleElementNamesAsync(string settings, string tupleElementName1, string tupleElementName2)
        {
            var testCode = $@"
public class TestClass
{{
    public (int {tupleElementName1}, int {tupleElementName2}) TestMethod()
    {{
        return (1, 1);
    }}
}}
";

            await VerifyCSharpDiagnosticAsync(LanguageVersionEx.CSharp7, testCode, settings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Validates that tuple fields with no name will not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task ValidateNoTupleElementNamesAsync()
        {
            var testCode = @"
public class TestClass
{
    public (int, int) TestMethod()
    {
        return (1, 1);
    }
}
";

            await VerifyCSharpDiagnosticAsync(LanguageVersionEx.CSharp7, testCode, DefaultTestSettings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Validates the properly named inferred tuple element names will not produce diagnostics.
        /// </summary>
        /// <param name="settings">The test settings to use.</param>
        /// <param name="tupleElementName1">The expected tuple element name for the first field.</param>
        /// <param name="tupleElementName2">The expected tuple element name for the second field.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(DefaultTestSettings, "elementName1", "elementName2")]
        [InlineData(CamelCaseTestSettings, "elementName1", "elementName2")]
        [InlineData(PascalCaseTestSettings, "ElementName1", "ElementName2")]
        [InlineData(CamelCaseInferredTestSettings, "elementName1", "elementName2")]
        [InlineData(PascalCaseInferredTestSettings, "ElementName1", "ElementName2")]
        [InlineData(CamelCaseExplicitOnlyTestSettings, "elementName1", "elementName2")]
        [InlineData(PascalCaseExplicitOnlyTestSettings, "ElementName1", "ElementName2")]
        public async Task ValidateProperCasedInferredTupleElementNamesAsync(string settings, string tupleElementName1, string tupleElementName2)
        {
            var testCode = $@"
public class TestClass
{{
    public void TestMethod()
    {{
        var {tupleElementName1} = 1;
        var {tupleElementName2} = ""test"";
        var tuple = ({tupleElementName1}, {tupleElementName2});
    }}
}}
";

            await VerifyCSharpDiagnosticAsync(LanguageVersionEx.CSharp7_1, testCode, settings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Validates the improperly named tuple element names will produce the expected diagnostics.
        /// </summary>
        /// <param name="settings">The test settings to use.</param>
        /// <param name="tupleElementName1">The expected tuple element name for the first field.</param>
        /// <param name="tupleElementName2">The expected tuple element name for the second field.</param>
        /// <param name="fixedTupleElementName1">The expected fixed tuple element name for the first field.</param>
        /// <param name="fixedTupleElementName2">The expected fixed tuple element name for the second field.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(DefaultTestSettings, "elementName1", "elementName2", "ElementName1", "ElementName2")]
        [InlineData(CamelCaseTestSettings, "ElementName1", "ElementName2", "elementName1", "elementName2")]
        [InlineData(PascalCaseTestSettings, "elementName1", "elementName2", "ElementName1", "ElementName2")]
        public async Task ValidateImproperCasedTupleElementNamesAsync(string settings, string tupleElementName1, string tupleElementName2, string fixedTupleElementName1, string fixedTupleElementName2)
        {
            var testCode = $@"
public class TestClass
{{
    public (int [|{tupleElementName1}|], int [|{tupleElementName2}|]) TestMethod1()
    {{
        return (1, 1);
    }}

    public (int /* 1 */ [|{tupleElementName1}|] /* 2 */ , int /* 3 */ [|{tupleElementName2}|] /* 4 */) TestMethod2()
    {{
        return (1, 1);
    }}
}}
";

            var fixedCode = $@"
public class TestClass
{{
    public (int {fixedTupleElementName1}, int {fixedTupleElementName2}) TestMethod1()
    {{
        return (1, 1);
    }}

    public (int /* 1 */ {fixedTupleElementName1} /* 2 */ , int /* 3 */ {fixedTupleElementName2} /* 4 */) TestMethod2()
    {{
        return (1, 1);
    }}
}}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // diagnostics are specified inline
            };

            await VerifyCSharpFixAsync(LanguageVersionEx.CSharp7, testCode, settings, expectedDiagnostics, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        ///  Verifies that improperly named inferred tuple element names are ignored when the 'includeInferredTupleElementNames' option is not set to true.
        /// </summary>
        /// <param name="settings">The test settings to use.</param>
        /// <param name="tupleElementName1">The expected tuple element name for the first field.</param>
        /// <param name="tupleElementName2">The expected tuple element name for the second field.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(DefaultTestSettings, "ElementName1", "ElementName2")]
        [InlineData(CamelCaseTestSettings, "ElementName1", "ElementName2")]
        [InlineData(PascalCaseTestSettings, "elementName1", "elementName2")]
        [InlineData(CamelCaseExplicitOnlyTestSettings, "ElementName1", "ElementName2")]
        [InlineData(PascalCaseExplicitOnlyTestSettings, "elementName1", "elementName2")]
        public async Task ValidateImproperCasedInferredTupleElementNamesAreIgnoredAsync(string settings, string tupleElementName1, string tupleElementName2)
        {
            var testCode = $@"
public class TestClass
{{
    public void TestMethod()
    {{
        var {tupleElementName1} = 1;
        var {tupleElementName2} = ""test"";
        var tuple = ({tupleElementName1}, {tupleElementName2});
    }}
}}
";

            await VerifyCSharpDiagnosticAsync(LanguageVersionEx.CSharp7_1, testCode, settings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Validates the improperly named inferred tuple element names will produce the expected diagnostics.
        /// </summary>
        /// <param name="settings">The test settings to use.</param>
        /// <param name="tupleElementName1">The expected tuple element name for the first field.</param>
        /// <param name="tupleElementName2">The expected tuple element name for the second field.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(CamelCaseInferredTestSettings, "ElementName1", "ElementName2")]
        [InlineData(PascalCaseInferredTestSettings, "elementName1", "elementName2")]
        public async Task ValidateImproperCasedImplicitTupleElementNamesAsync(string settings, string tupleElementName1, string tupleElementName2)
        {
            //// TODO: C# 7.1
            var testCode = $@"
public class TestClass
{{
    public void TestMethod1()
    {{
        var {tupleElementName1} = 1;
        var {tupleElementName2} = ""test"";
        var tuple = ([|{tupleElementName1}|], [|{tupleElementName2}|]);
    }}

    public void TestMethod2()
    {{
        var {tupleElementName1} = 1;
        var {tupleElementName2} = ""test"";
        var tuple = (/* 1 */ [|{tupleElementName1}|] /* 2 */, /* 3 */ [|{tupleElementName2}|] /* 4 */);
    }}
}}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // diagnostics are specified inline
            };

            await VerifyCSharpDiagnosticAsync(LanguageVersionEx.CSharp7_1, testCode, settings, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
