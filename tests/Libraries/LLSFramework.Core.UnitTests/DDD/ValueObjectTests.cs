namespace LLSFramework.Core.UnitTests.DDD;

public class ValueObjectTests
{
    public class TestValueObject : ValueObject
    {
        public int Prop1 { get; set; }
        public DateTime Prop2 { get; set; }
        public string? Prop3 { get; set; }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Prop1;
            yield return Prop2;
            yield return Prop3;
        }
    }

    [Fact]
    public void ValueObjects_ShouldNotBeEqual()
    {
        var entity1 = new TestValueObject
        {
            Prop1 = 1,
            Prop2 = DateTime.UtcNow,
            Prop3 = "Test1"
        };

        var entity2 = new TestValueObject
        {
            Prop1 = 2,
            Prop2 = DateTime.UtcNow,
            Prop3 = "Test2"
        };

        (entity1 == entity2).ShouldBeFalse();
        (entity2 == entity1).ShouldBeFalse();

        entity1.Equals(entity2).ShouldBeFalse();
        entity2.Equals(entity1).ShouldBeFalse();
    }

    [Fact]
    public void ValueObjects_ShouldBeEqual()
    {
        var now = DateTime.UtcNow;

        var entity1 = new TestValueObject
        {
            Prop1 = 1,
            Prop2 = now,
            Prop3 = "Test1"
        };

        var entity2 = new TestValueObject
        {
            Prop1 = 1,
            Prop2 = now,
            Prop3 = "Test1"
        };

        (entity1 == entity2).ShouldBeTrue();
        (entity2 == entity1).ShouldBeTrue();

        entity1.Equals(entity2).ShouldBeTrue();
        entity2.Equals(entity1).ShouldBeTrue();
    }
}