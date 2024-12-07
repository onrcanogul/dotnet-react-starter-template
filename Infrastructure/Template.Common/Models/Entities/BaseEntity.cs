namespace Template.Common.Models.Entities;

public abstract class BaseEntity
{
    //Identity
    public Guid Id { get; set; }
    
    //Audit
    public virtual DateTime CreatedDate { get; set; }
    public virtual DateTime? UpdatedDate { get; set; }
    public virtual string CreatedBy { get; set; }
    public virtual string? UpdatedBy { get; set; }
    
    //Soft Delete
    public bool IsDeleted { get; set; }
}