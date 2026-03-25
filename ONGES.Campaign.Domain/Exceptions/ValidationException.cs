namespace ONGES.Campaign.Domain.Exceptions;

/// <summary>
/// Exception thrown when validation fails.
/// </summary>
public class ValidationException : Exception
{
    public IDictionary<string, string[]> Failures { get; }

    public ValidationException() : base("One or more validation errors occurred.")
    {
        Failures = new Dictionary<string, string[]>();
    }

    public ValidationException(string key, string message) : this()
    {
        Failures.Add(key, [message]);
    }

    public ValidationException(string key, string[] messages) : this()
    {
        Failures.Add(key, messages);
    }

    public ValidationException(List<FluentValidation.Results.ValidationFailure> failures) : this()
    {
        var propertyNames = failures
            .Select(e => e.PropertyName)
            .Distinct();

        foreach (var propertyName in propertyNames)
        {
            var propertyFailures = failures
                .Where(e => e.PropertyName == propertyName)
                .Select(e => e.ErrorMessage)
                .ToArray();

            Failures.Add(propertyName, propertyFailures);
        }
    }
}
