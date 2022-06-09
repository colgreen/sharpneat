namespace SharpNeat.Experiments;

/// <summary>
/// Static helper methods for working with strongly typed model classes.
/// </summary>
public static class ModelHelper
{
    // TODO: Consider using expression tree, so that the property name can be obtained and used in the exception message.
    /// <summary>
    /// Invoke a property getter functions, and throw a <see cref="ConfigurationException"/> if the retrieved value is null.
    /// </summary>
    /// <typeparam name="T">Property type.</typeparam>
    /// <param name="getter">Property getter functions.</param>
    /// <returns>The property value.</returns>
    /// <exception cref="ConfigurationException">Thrown if the retrieved property value is null.</exception>
    public static T GetMandatoryProperty<T>(Func<T?> getter)
        where T : class
    {
        T? val = getter();
        if(val is null) throw new ConfigurationException("Missing mandatory property.");
        return val;
    }

    /// <summary>
    /// Invoke a property getter functions, and throw a <see cref="ConfigurationException"/> if the retrieved value is null.
    /// </summary>
    /// <typeparam name="T">Property type.</typeparam>
    /// <param name="getter">Property getter functions.</param>
    /// <returns>The property value.</returns>
    /// <exception cref="ConfigurationException">Thrown if the retrieved property value is null.</exception>
    public static T GetMandatoryProperty<T>(Func<T?> getter)
        where T : struct
    {
        T? val = getter();
        if(!val.HasValue) throw new ConfigurationException("Missing mandatory property.");
        return val.Value;
    }
}
