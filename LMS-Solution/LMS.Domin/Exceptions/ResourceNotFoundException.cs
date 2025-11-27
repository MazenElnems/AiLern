namespace LMS.Domin.Exceptions;

public class ResourceNotFoundException : Exception
{
    public ResourceNotFoundException(string resourceType, string resourceIdentifier)
        : base($"Resource {resourceType} with ID: {resourceIdentifier} not found")
    {
        
    }
}
