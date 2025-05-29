namespace LLSFramework.Core.UnitTests.Parsers;

public class GenericParserTests
{
    public enum TestEnum
    {
        None,
        Value1,
        Value2
    }

    [Fact]
    public void Parse_ShouldParseIntSuccessfully()
    {
        // Arrange
        var input = "123";

        // Act
        var result = GenericParser.Parse<int>(input);

        // Assert
        result.ShouldBe(123);
    }

    [Fact]
    public void Parse_ShouldParseNullableIntSuccessfully()
    {
        // Arrange
        var input = "456";

        // Act
        var result = GenericParser.Parse<int?>(input);

        // Assert
        result.ShouldBe(456);
    }

    [Fact]
    public void Parse_ShouldReturnNullForNullableIntWithEmptyString()
    {
        // Arrange
        var input = string.Empty;

        // Act
        var result = GenericParser.Parse<int?>(input);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void Parse_ShouldParseDecimalSuccessfully()
    {
        // Arrange
        var input = "123.45";

        // Act
        var result = GenericParser.Parse<decimal>(input);

        // Assert
        result.ShouldBe(123.45m);
    }

    [Fact]
    public void Parse_ShouldParseNullableDecimalSuccessfully()
    {
        // Arrange
        var input = "456.78";

        // Act
        var result = GenericParser.Parse<decimal?>(input);

        // Assert
        result.ShouldBe(456.78m);
    }

    [Fact]
    public void Parse_ShouldParseDateTimeSuccessfully()
    {
        // Arrange
        var input = "2024-12-14";

        // Act
        var result = GenericParser.Parse<DateTime>(input);

        // Assert
        result.ShouldBe(new DateTime(2024, 12, 14));
    }

    [Fact]
    public void Parse_ShouldParseNullableDateTimeSuccessfully()
    {
        // Arrange
        var input = "2024-12-14";

        // Act
        var result = GenericParser.Parse<DateTime?>(input);

        // Assert
        result.ShouldBe(new DateTime(2024, 12, 14));
    }

    [Fact]
    public void Parse_ShouldParseEnumSuccessfully()
    {
        // Arrange
        var input = "Value1";

        // Act
        var result = GenericParser.Parse<TestEnum>(input);

        // Assert
        result.ShouldBe(TestEnum.Value1);
    }

    [Fact]
    public void Parse_ShouldParseListOfEnumsSuccessfully()
    {
        // Arrange
        var input = "Value1,Value2";

        // Act
        var result = GenericParser.Parse<List<TestEnum>>(input);

        // Assert        
        result.ShouldBeEquivalentTo(new List<TestEnum> { TestEnum.Value1, TestEnum.Value2 });
    }

    [Fact]
    public void Parse_ShouldReturnInputAsStringIfTypeNotHandled()
    {
        // Arrange
        var input = "UnhandledType";

        // Act
        var result = GenericParser.Parse<string>(input);

        // Assert
        result.ShouldBe("UnhandledType");
    }

    [Fact]
    public void Parse_ShouldParseBoolSuccessfully()
    {
        // Arrange
        var input = "true";

        // Act
        var result = GenericParser.Parse<bool>(input);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void Parse_ShouldParseNullableBoolSuccessfully()
    {
        // Arrange
        var input = "false";

        // Act
        var result = GenericParser.Parse<bool?>(input);

        // Assert
        result.GetValueOrDefault().ShouldBeFalse();
    }

    [Fact]
    public void Parse_ShouldReturnNullForNullableBoolWithEmptyString()
    {
        // Arrange
        var input = string.Empty;

        // Act
        var result = GenericParser.Parse<bool?>(input);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void Parse_ShouldParseDoubleSuccessfully()
    {
        // Arrange
        var input = "123.456";

        // Act
        var result = GenericParser.Parse<double>(input);

        // Assert
        result.ShouldBe(123.456);
    }

    [Fact]
    public void Parse_ShouldParseNullableDoubleSuccessfully()
    {
        // Arrange
        var input = "456.789";

        // Act
        var result = GenericParser.Parse<double?>(input);

        // Assert
        result.ShouldBe(456.789);
    }

    [Fact]
    public void Parse_ShouldReturnNullForNullableDoubleWithEmptyString()
    {
        // Arrange
        var input = string.Empty;

        // Act
        var result = GenericParser.Parse<double?>(input);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void Parse_ShouldParseGuidSuccessfully()
    {
        // Arrange
        var input = "d3b07384-d9a0-4c9b-8a0d-1b2e4f3c4d5e";

        // Act
        var result = GenericParser.Parse<Guid>(input);

        // Assert
        result.ShouldBe(Guid.Parse("d3b07384-d9a0-4c9b-8a0d-1b2e4f3c4d5e"));
    }

    [Fact]
    public void Parse_ShouldParseNullableGuidSuccessfully()
    {
        // Arrange
        var input = "d3b07384-d9a0-4c9b-8a0d-1b2e4f3c4d5e";

        // Act
        var result = GenericParser.Parse<Guid?>(input);

        // Assert
        result.ShouldBe(Guid.Parse("d3b07384-d9a0-4c9b-8a0d-1b2e4f3c4d5e"));
    }

    [Fact]
    public void Parse_ShouldReturnNullForNullableGuidWithEmptyString()
    {
        // Arrange
        var input = string.Empty;

        // Act
        var result = GenericParser.Parse<Guid?>(input);

        // Assert
        result.ShouldBeNull();
    }
}