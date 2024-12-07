namespace Template.Common.Models.Dtos;

public abstract class BaseDto
{
    //Identity
    public Guid Id { get; set; }
    
    //Audit
    public virtual DateTime CreatedDate { get; set; }
    public virtual DateTime? UpdatedDate { get; set; }
    public virtual string CreatedBy { get; set; }
    public virtual string? UpdatedBy { get; set; }  
}