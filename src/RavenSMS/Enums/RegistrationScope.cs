namespace RavenSMS.Enums;

/// <summary>
/// the registration scope types
/// </summary>
public enum RegistrationScope
{
    /// <summary>
    /// Transient life time
    /// </summary>
    Transient,

    /// <summary>
    /// scoped life time
    /// </summary>
    Scoped,

    /// <summary>
    /// singleton life time
    /// </summary>
    Singleton
}