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

    public class SupportDetailsDto_SupportTikcetDetailsView
    {
        public string SupportId {get; set;} = "";
        public string SupportStatus {get; set;} = "";
        public string SupportTitle {get; set;} = "";
        public string SupportDescription {get; set;} = "";
        public DateTime SupportRaisedDate {get; set;} = DateTime.MinValue;
    }

    
}