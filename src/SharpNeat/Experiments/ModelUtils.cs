using System.Linq.Expressions;

namespace SharpNeat.Experiments;

/// <summary>
/// Static utility methods for working with strongly typed model classes.
/// </summary>
public static class ModelUtils
{
    /// <summary>
    /// Attempt to get a mandatory property from a model object.
    /// </summary>
    /// <typeparam name="TModel">Model type.</typeparam>
    /// <typeparam name="TPropertyValue">Type of the property to get.</typeparam>
    /// <param name="model">The model object that contains the property to get.</param>
    /// <param name="propertyExpression">Expression that refers to the required property on the model type.</param>
    /// <returns>The property value, if the property is not null.</returns>
    /// <exception cref="ConfigurationException">Throw if the required property is null.</exception>
    public static TPropertyValue GetMandatoryProperty<TModel, TPropertyValue>(
        TModel model,
        Expression<Func<TModel, TPropertyValue?>> propertyExpression)
        where TModel : class
        where TPropertyValue : struct
    {
        var fn = propertyExpression.Compile();
        TPropertyValue? val = fn(model);

        if(!val.HasValue)
        {
            string propertyName = ((MemberExpression)propertyExpression.Body).Member.Name;
            throw new ConfigurationException(
                $"Missing mandatory value for model property [{model.GetType().Name}.{propertyName}].");
        }

        return val.Value;
    }
}
