using API.Entities;

namespace API.DTOs;

public class PhotoDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; }
    public string Url { get; set; }
    public bool IsMain { get; set; }
    public bool IsApproved { get; set; }
}