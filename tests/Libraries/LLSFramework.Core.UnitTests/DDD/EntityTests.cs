namespace LLSFramework.Core.UnitTests.DDD;

public class EntityTests
{
    public class TestEntity : Entity<Guid>
    {
    }

    [Fact]
    public void Entities_ShouldNotBeEqual()
    {
        var entity1 = new TestEntity { Id = Guid.NewGuid() };
        var entity2 = new TestEntity { Id = Guid.NewGuid() };

        (entity1 == entity2).ShouldBeFalse();
        (entity2 == entity1).ShouldBeFalse();

        entity1.Equals(entity2).ShouldBeFalse();
        entity2.Equals(entity1).ShouldBeFalse();
    }

    [Fact]
    public void Entities_ShouldBeEqual()
    {
        var guid = Guid.NewGuid();

        var entity1 = new TestEntity { Id = guid };
        var entity2 = new TestEntity { Id = guid };

        (entity1 == entity2).ShouldBeTrue();
        (entity2 == entity1).ShouldBeTrue();

        entity1.Equals(entity2).ShouldBeTrue();
        entity2.Equals(entity1).ShouldBeTrue();
    }
}