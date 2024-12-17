using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace v1Remastered.Dto
{
    public class SupportDetailsDto_SupportForm
    {
        [Required(ErrorMessage = "Ticket title is required")]
        public string SupportTitle {get; set;} = "";
        
        [Required(ErrorMessage = "Ticket description is required")]
        public string SupportDescription {get; set;} = "";
    }

    public class SupportDetailsDto_SupportTicketsList
    {
        public string SupportId {get; set;} = "";
        public string SupportStatus {get; set;} = "";
        public string SupportTitle {get; set;} = "";
        public string SupportRaisedDate {get; set;} = "";

    }

    public class SupportDetailsDto_SupportCommentsDetails
    {
        public string SupportCommentId {get; set;} = "";
        public string SupportId {get; set;} = "";
        public string SupportComment {get; set;} = "";
        public DateTime SupportCommentDate {get; set;} = DateTime.MinValue;
        public string UserName {get; set;} = "";
    }

    public class SupportDetailsDto_SupportTicketDetailsView
    {
        public string SupportId {get; set;} = "";
        public string SupportStatus {get; set;} = "";
        public string SupportTitle {get; set;} = "";
        public string SupportDescription {get; set;} = "";
        public DateTime SupportRaisedDate {get; set;} = DateTime.MinValue;
        public List<SupportDetailsDto_SupportCommentsDetails> SupportComments {get; set;} = new List<SupportDetailsDto_SupportCommentsDetails>();
    }

    
}