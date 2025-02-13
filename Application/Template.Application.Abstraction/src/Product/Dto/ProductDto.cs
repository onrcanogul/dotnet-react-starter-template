using Template.Common.Models.Dtos;

namespace Template.Application.src.Abstraction.Dto;

public class ProductDto : BaseDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int Likes { get; set; }
    public int Dislikes { get; set; }
    public double Rating { get; set; }
    public List<string> Comments { get; set; } = new();
}