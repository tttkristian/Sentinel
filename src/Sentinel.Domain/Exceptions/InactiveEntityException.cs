namespace CallCenter.Domain.Exceptions;

public sealed class InactiveEntityException(string entityName, object key)
    : Exception($"Entity '{entityName}' with key '{key}' is inactive or deleted.")
{
    public string EntityName { get; } = entityName;
    public object Key { get; } = key;
}