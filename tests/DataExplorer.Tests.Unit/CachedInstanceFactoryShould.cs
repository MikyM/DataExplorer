using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using DataExplorer.Utilities;

namespace DataExplorer.Tests.Unit;

[Collection("CachedInstanceFactoryShould")]
public class CachedInstanceFactoryShould
{
    private ConcurrentDictionary<Type, CreateDelegate> GetCachedFuncs<TArg1,TArg2,TArg3,TArg4>(CachedInstanceFactory factory)
        => (ConcurrentDictionary<Type, CreateDelegate>)(typeof(CachedInstanceFactory<TArg1,TArg2,TArg3,TArg4>).GetField("_cachedFuncs", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(factory)!);
    
    [Fact]
    public void Use_cached_ctor_delegate_when_called_twice_no_arg_ctor()
    {
        // Arrange
        var factory = new CachedInstanceFactory();
        var type = typeof(Test0);
        var st1 = new Stopwatch();
        var st2 = new Stopwatch();
        
        // Act
        st1.Start();
        var instance1 = factory.CreateInstance(type);
        st1.Stop();
        st2.Start();
        var instance2 = factory.CreateInstance(type);
        st2.Stop();
        
        // Assert
        var cached = GetCachedFuncs<TypeToIgnore,TypeToIgnore,TypeToIgnore,TypeToIgnore>(factory);
        cached.Should().ContainSingle();
        cached.First().Key.Should().Be(type);
        st1.Elapsed.Should().BeGreaterThan(st2.Elapsed);
        instance1.Should().NotBeNull();
        instance2.Should().NotBeNull();
    }
    
    [Fact]
    public void Use_cached_ctor_delegate_when_called_twice_1_arg_ctor()
    {
        // Arrange
        var factory = new CachedInstanceFactory();
        var type = typeof(Test1);
        var st1 = new Stopwatch();
        var st2 = new Stopwatch();
        
        // Act
        st1.Start();
        var instance1 = factory.CreateInstance(type, "test");
        st1.Stop();
        st2.Start();
        var instance2 = factory.CreateInstance(type, "test1");
        st2.Stop();
        
        // Assert
        var cached = GetCachedFuncs<string,TypeToIgnore,TypeToIgnore,TypeToIgnore>(factory);
        cached.Should().ContainSingle();
        cached.First().Key.Should().Be(type);
        st1.Elapsed.Should().BeGreaterThan(st2.Elapsed);
        instance1.Should().NotBeNull();
        instance2.Should().NotBeNull();
    }
    
    [Fact]
    public void Use_cached_ctor_delegate_when_called_twice_2_arg_ctor()
    {
        // Arrange
        var factory = new CachedInstanceFactory();
        var type = typeof(Test2);
        var st1 = new Stopwatch();
        var st2 = new Stopwatch();
        
        // Act
        st1.Start();
        var instance1 = factory.CreateInstance(type, "test", 1);
        st1.Stop();
        st2.Start();
        var instance2 = factory.CreateInstance(type, "test1", 2);
        st2.Stop();
        
        // Assert
        var cached = GetCachedFuncs<string,int,TypeToIgnore,TypeToIgnore>(factory);
        cached.Should().ContainSingle();
        cached.First().Key.Should().Be(type);
        st1.Elapsed.Should().BeGreaterThan(st2.Elapsed);
        instance1.Should().NotBeNull();
        instance2.Should().NotBeNull();
    }
    
    [Fact]
    public void Use_cached_ctor_delegate_when_called_twice_3_arg_ctor()
    {
        // Arrange
        var factory = new CachedInstanceFactory();
        var type = typeof(Test3);
        var st1 = new Stopwatch();
        var st2 = new Stopwatch();
        
        // Act
        st1.Start();
        var instance1 = factory.CreateInstance(type, "test", 1, new Test2("test", 1));
        st1.Stop();
        st2.Start();
        var instance2 = factory.CreateInstance(type, "test1", 2, new Test2("test", 1));
        st2.Stop();
        
        // Assert
        var cached = GetCachedFuncs<string,int,Test2,TypeToIgnore>(factory);
        cached.Should().ContainSingle();
        cached.First().Key.Should().Be(type);
        st1.Elapsed.Should().BeGreaterThan(st2.Elapsed);
        instance1.Should().NotBeNull();
        instance2.Should().NotBeNull();
    }
    
    [Fact]
    public void Use_cached_ctor_delegate_when_called_twice_4_arg_ctor()
    {
        // Arrange
        var factory = new CachedInstanceFactory();
        var type = typeof(Test4);
        var st1 = new Stopwatch();
        var st2 = new Stopwatch();
        
        // Act
        st1.Start();
        var instance1 = factory.CreateInstance(type, "test", 1, new Test2("test", 1), new Test3("test", 1, new Test2("test", 1)));
        st1.Stop();
        st2.Start();
        var instance2 = factory.CreateInstance(type, "test1", 2, new Test2("test", 1), new Test3("test", 1, new Test2("test", 1)));
        st2.Stop();
        
        // Assert
        var cached = GetCachedFuncs<string,int,Test2,Test3>(factory);
        cached.Should().ContainSingle();
        cached.First().Key.Should().Be(type);
        st1.Elapsed.Should().BeGreaterThan(st2.Elapsed);
        instance1.Should().NotBeNull();
        instance2.Should().NotBeNull();
    }
    
    [Fact]
    public void Retain_two_cached_delegates()
    {
        // Arrange
        var factory = new CachedInstanceFactory();
        var type1 = typeof(Test5);
        var type2 = typeof(Test6);
        
        // Act
        var instance1 = factory.CreateInstance(type1, "test", 1, 1);
        var instance2 = factory.CreateInstance(type2, "test1", 2, 1);

        // Assert
        var cached = GetCachedFuncs<string,int,int,TypeToIgnore>(factory);
        cached.Should().HaveCount(2);
        cached.Should().ContainKeys(type1, type2);
        instance1.Should().NotBeNull();
        instance2.Should().NotBeNull();
    }

    public class Test0
    {
        public Test0()
        {
        }
    }
    
    public class Test1
    {
        public Test1(string test)
        {
            
        }
    }
    
    public class Test2
    {
        public Test2(string test, int test1)
        {
            
        }
    }
    
    public class Test3
    {
        public Test3(string test, int test1, Test2 test2)
        {
            
        }
    }
    
    public class Test4
    {
        public Test4(string test, int test1, Test2 test2, Test3 test3)
        {
            
        }
    }
    
    public class Test5
    {
        public Test5(string test, int test1, int test2)
        {
            
        }
    }
    
    public class Test6
    {
        public Test6(string test, int test1, int test2)
        {
            
        }
    }
}
