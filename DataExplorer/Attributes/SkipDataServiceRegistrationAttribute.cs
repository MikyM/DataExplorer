using AttributeBasedRegistration.Attributes.Abstractions;

namespace DataExplorer.Attributes;

/// <summary>
/// Tells the automatic registration process to skip this data service and allow manual registration.
/// </summary>
[PublicAPI]
public interface ISkipDataServiceRegistrationAttribute : ISkipRegistrationAttribute
{
}

/// <summary>
/// Tells the automatic registration process to skip this data service and allow manual registration.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
[PublicAPI]
public class SkipDataServiceRegistrationAttribute : Attribute, ISkipDataServiceRegistrationAttribute
{
}
