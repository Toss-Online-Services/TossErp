namespace TossErp.Application.DTOs;

public class CopilotQueryDto
{
    public string Query { get; set; } = string.Empty;
    public Guid BusinessId { get; set; }
    public Guid UserId { get; set; }
}

public class CopilotResponseDto
{
    public string Response { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public List<string> Actions { get; set; } = new();
    public List<string> Suggestions { get; set; } = new();
    public bool RequiresAction { get; set; }
} 
