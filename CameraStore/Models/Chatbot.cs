using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CameraStore.Models
{
    public class Chatbot
    {
        [Key]
        public int ChatID { get; set; }
        public string chatName { get; set; }
        public string chatTelephone { get; set; }
        public virtual ICollection<contentChatbot> ContentChatbots { get; set; }


    }
}
