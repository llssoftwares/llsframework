namespace LLSFramework.Core.UnitTests.Filter;

public class FilterExtensionsTests
{
    public class TestEntity
    {
        public int Int { get; set; }

        public int? NullableInt { get; set; }

        public Guid Guid { get; set; }

        public Guid? NullableGuid { get; set; }

        public string? String { get; set; }

        public DateTime DateTime { get; set; }

        public DateTime? NullableDateTime { get; set; }

        public TestEnum Enum { get; set; }
    }

    public class TestFilter : EntityFilter
    {
        public int Int { get; set; }

        public int? NullableInt { get; set; }

        public Guid? Guid { get; set; }

        public Guid? NullableGuid { get; set; }

        [FilterContains]
        [FilterUnderlyingName("String")]
        public string? StringContains { get; set; }

        [FilterEquals]
        [FilterUnderlyingName("String")]
        public string? StringEquals { get; set; }

        [FilterDateTimeGreaterThanOrEqual]
        [FilterUnderlyingName("DateTime")]
        public DateTime? DateTimeStart { get; set; }

        [FilterDateTimeLessThanOrEqual]
        [FilterUnderlyingName("DateTime")]
        public DateTime? DateTimeEnd { get; set; }

        [FilterDateTimeGreaterThanOrEqual]
        [FilterUnderlyingName("NullableDateTime")]
        public DateTime? NullableDateTimeStart { get; set; }

        [FilterDateTimeLessThanOrEqual]
        [FilterUnderlyingName("NullableDateTime")]
        public DateTime? NullableDateTimeEnd { get; set; }

        [FilterEnumParameter]
        public TestEnum? Enum { get; set; }

        [FilterEnumParameter]
        [FilterUnderlyingName("Enum")]
        public List<TestEnum> EnumList { get; set; } = [];
    }

    public enum TestEnum
    {
        Value1,
        Value2,
        Value3
    }

    [Fact]
    public void Filter_Should_Return_All_Items_When_Filter_Is_Null()
    {
        // Arrange
        var entities = new List<TestEntity>
            {
                new(),
                new()
            }.AsQueryable();

        TestFilter? filter = null;

        // Act
        var result = entities.Filter(filter);

        // Assert
        result.ShouldBeEquivalentTo(entities);
    }

    [Fact]
    public void Filter_Should_Filter_By_Int()
    {
        // Arrange
        var entities = new List<TestEntity>
            {
                new() { Int = 1 },
                new() { Int = 2 }
            }.AsQueryable();

        var filter = new TestFilter { Int = 1 };

        // Act
        var result = entities.Filter(filter);

        // Assert
        result.Count().ShouldBe(1);
        result.First().Int.ShouldBe(1);
    }

    [Fact]
    public void Filter_Should_Filter_By_NullableInt()
    {
        // Arrange
        var entities = new List<TestEntity>
            {
                new() { NullableInt = 1 },
                new() { NullableInt = 2 }
            }.AsQueryable();

        var filter = new TestFilter { NullableInt = 1 };

        // Act
        var result = entities.Filter(filter);

        // Assert
        result.Count().ShouldBe(1);
        result.First().NullableInt.ShouldBe(1);
    }

    [Fact]
    public void Filter_Should_Filter_By_Guid()
    {
        // Arrange
        var guid1 = Guid.NewGuid();
        var guid2 = Guid.NewGuid();

        var entities = new List<TestEntity>
            {
                new() { Guid = guid1 },
                new() { Guid = guid2 }
            }.AsQueryable();

        var filter = new TestFilter { Guid = guid1 };

        // Act
        var result = entities.Filter(filter);

        // Assert
        result.Count().ShouldBe(1);
        result.First().Guid.ShouldBe(guid1);
    }

    [Fact]
    public void Filter_Should_Filter_By_NullableGuid()
    {
        // Arrange
        var guid1 = Guid.NewGuid();
        var guid2 = Guid.NewGuid();

        var entities = new List<TestEntity>
            {
                new() { NullableGuid = guid1 },
                new() { NullableGuid = guid2 }
            }.AsQueryable();

        var filter = new TestFilter { NullableGuid = guid1 };

        // Act
        var result = entities.Filter(filter);

        // Assert
        result.Count().ShouldBe(1);
        result.First().NullableGuid.ShouldBe(guid1);
    }

    [Fact]
    public void Filter_Should_Filter_By_StringContains()
    {
        // Arrange
        var entities = new List<TestEntity>
            {
                new() { String = "Abcd" },
                new() { String = "Efgh" }
            }.AsQueryable();

        var filter = new TestFilter { StringContains = "bc" };

        // Act
        var result = entities.Filter(filter);

        // Assert
        result.Count().ShouldBe(1);
        result.First().String.ShouldBe("Abcd");
    }

    [Fact]
    public void Filter_Should_Filter_By_StringEquals()
    {
        // Arrange
        var entities = new List<TestEntity>
            {
                new() { String = "Abcd" },
                new() { String = "Efgh" }
            }.AsQueryable();

        var filter = new TestFilter { StringEquals = "Abcd" };

        // Act
        var result = entities.Filter(filter);

        // Assert
        result.Count().ShouldBe(1);
        result.First().String.ShouldBe("Abcd");
    }

    [Fact]
    public void Filter_Should_Filter_By_Enum()
    {
        // Arrange
        var entities = new List<TestEntity>
            {
                new() { Enum = TestEnum.Value1 },
                new() { Enum = TestEnum.Value2 }
            }.AsQueryable();

        var filter = new TestFilter { Enum = TestEnum.Value1 };

        // Act
        var result = entities.Filter(filter);

        // Assert
        result.Count().ShouldBe(1);
        result.First().Enum.ShouldBe(TestEnum.Value1);
    }

    [Fact]
    public void Filter_Should_Filter_By_EnumList()
    {
        // Arrange
        var entities = new List<TestEntity>
            {
                new() { Enum = TestEnum.Value1 },
                new() { Enum = TestEnum.Value2 },
                new() { Enum = TestEnum.Value3 },
            }.AsQueryable();

        var filter = new TestFilter { EnumList = [TestEnum.Value1, TestEnum.Value2] };

        // Act
        var result = entities.Filter(filter).ToList();

        // Assert
        result.Count.ShouldBe(2);

        result[0].Enum.ShouldBe(TestEnum.Value1);
        result[1].Enum.ShouldBe(TestEnum.Value2);
    }

    [Fact]
    public void Filter_Should_Filter_By_DateTime_Range()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var entities = new List<TestEntity>
            {
                new() { DateTime = now.AddDays(-1) },
                new() { DateTime = now.AddDays(-2) },
                new() { DateTime = now.AddDays(-3) },
                new() { DateTime = now.AddDays(-4) }
            }.AsQueryable();

        var filter = new TestFilter { DateTimeStart = now.AddDays(-3), DateTimeEnd = now.AddDays(-2) };

        // Act
        var result = entities.Filter(filter).ToList();

        // Assert        
        result.Count.ShouldBe(2);

        result[0].DateTime.ShouldBe(now.AddDays(-2));
        result[1].DateTime.ShouldBe(now.AddDays(-3));
    }

    [Fact]
    public void Filter_Should_Filter_By_NullableDateTime_Range()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var entities = new List<TestEntity>
            {
                new() { NullableDateTime = now.AddDays(-1) },
                new() { NullableDateTime = now.AddDays(-2) },
                new() { NullableDateTime = now.AddDays(-3) },
                new() { NullableDateTime = now.AddDays(-4) }
            }.AsQueryable();

        var filter = new TestFilter { NullableDateTimeStart = now.AddDays(-3), NullableDateTimeEnd = now.AddDays(-2) };

        // Act
        var result = entities.Filter(filter).ToList();

        // Assert        
        result.Count.ShouldBe(2);

        result[0].NullableDateTime.ShouldBe(now.AddDays(-2));
        result[1].NullableDateTime.ShouldBe(now.AddDays(-3));
    }
}