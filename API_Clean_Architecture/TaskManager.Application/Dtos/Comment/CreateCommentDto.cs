namespace TaskManager.Application.Dtos.Comment;

public class CreateCommentDto
{
    public string Content { get; set; } = string.Empty; //usuario vem do jwt, taskid vem da url
}