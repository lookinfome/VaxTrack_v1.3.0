using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;


public class SupportDetailsModel
{
    [Required(ErrorMessage = "Support id is required")]
    [Key]
    public string SupportId {get; set;} = "";
    public int SupportStatus {get; set;} = -1;
    public string SupportTitle {get; set;} = "";
    public string SupportDescription {get; set;} = "";
    public DateTime SupportRaisedDate {get; set;} = DateTime.MinValue;
    public string UserId {get; set;} = "";
}

public class SupportConversationsModel
{
    [Required(ErrorMessage = "support comment id is required")]
    [Key]
    public string SupportCommentId {get; set;} = "";
    public string SupportId {get; set;} = "";
    public string SupportComment {get; set;} = "";
    public DateTime SupportCommentDate {get; set;} = DateTime.MinValue;
    public string UserId {get; set;} = "";
}