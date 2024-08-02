using System.Linq.Expressions;
using DataExplorer.Tests.Shared;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using ExpressionExtensions = DataExplorer.EfCore.Specifications.Extensions.ExpressionExtensions;

namespace DataExplorer.EfCore.Tests.Unit;

public class ExpressionExtensionsTests
{
#if NET7_0_OR_GREATER
    public class JoinShould
    {
        [Fact]
        public void WithTwoExpressionsReturnsJoinedExpression()
        {
            // Arrange
            Expression<Func<SetPropertyCalls<TestEntity>, SetPropertyCalls<TestEntity>>> expression1 = x => x.SetProperty(p => p.Description, "John Doe");
            Expression<Func<SetPropertyCalls<TestEntity>, SetPropertyCalls<TestEntity>>> expression2 = x => x.SetProperty(p => p.Name, "John");
            Expression<Func<SetPropertyCalls<TestEntity>, SetPropertyCalls<TestEntity>>> expression3 = x => x.SetProperty(p => p.UpdatedAt, DateTime.Now);
            Expression<Func<SetPropertyCalls<TestEntity>, SetPropertyCalls<TestEntity>>> expression4 = x => x.SetProperty(p => p.Version, 5);

            // Act
            var result = ExpressionExtensions.Join(expression1, expression2, expression3, expression4);

            // Assert
            result.Should().NotBeNull();
        }
    
        [Fact]
        public void WithTwoOrMoreExpressionsDoesNotThrow()
        {
            // Arrange
            Expression<Func<SetPropertyCalls<TestEntity>, SetPropertyCalls<TestEntity>>> expression1 = x => x.SetProperty(p => p.Description, "John Doe");
            Expression<Func<SetPropertyCalls<TestEntity>, SetPropertyCalls<TestEntity>>> expression2 = x => x.SetProperty(p => p.Name, "John");
            Expression<Func<SetPropertyCalls<TestEntity>, SetPropertyCalls<TestEntity>>> expression3 = x => x.SetProperty(p => p.UpdatedAt, DateTime.Now);
            Expression<Func<SetPropertyCalls<TestEntity>, SetPropertyCalls<TestEntity>>> expression4 = x => x.SetProperty(p => p.Version, 5);

            // Act
            var result = () => ExpressionExtensions.Join(expression1, expression2, expression3, expression4);

            // Assert
            result.Should().NotThrow();
        }
    
        [Fact]
        public void WithTwoOrMoreExpressionsCompiles()
        {
            // Arrange
            Expression<Func<SetPropertyCalls<TestEntity>, SetPropertyCalls<TestEntity>>> expression1 = x => x.SetProperty(p => p.Description, "John Doe");
            Expression<Func<SetPropertyCalls<TestEntity>, SetPropertyCalls<TestEntity>>> expression2 = x => x.SetProperty(p => p.Name, "John");
            Expression<Func<SetPropertyCalls<TestEntity>, SetPropertyCalls<TestEntity>>> expression3 = x => x.SetProperty(p => p.UpdatedAt, DateTime.Now);
            Expression<Func<SetPropertyCalls<TestEntity>, SetPropertyCalls<TestEntity>>> expression4 = x => x.SetProperty(p => p.Version, 5);

            // Act
            var result = () => ExpressionExtensions.Join(expression1, expression2, expression3, expression4).Compile();

            // Assert
            result.Should().NotThrow();
        }
    }
#endif
}
