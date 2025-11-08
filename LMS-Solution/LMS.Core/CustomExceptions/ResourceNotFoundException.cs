namespace LMS.Core.CustomExceptions;

public class ResourceNotFoundException : Exception
{
    public ResourceNotFoundException(string resourceType, string resourceIdentifier)
        : base($"Resource {resourceType} with ID: {resourceIdentifier} not found")
    {
        
    }
}
