namespace HRApi.Domain.Common;

public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
}
