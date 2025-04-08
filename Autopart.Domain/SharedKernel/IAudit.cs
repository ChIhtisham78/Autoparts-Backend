namespace Autopart.Domain.SharedKernel
{
    public interface IAudit
    {
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt { get; set; }
    }
}
