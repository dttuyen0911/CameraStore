using System.ComponentModel.DataAnnotations.Schema;

namespace CameraStore.Models
{
    public class contentChatbot
    {
        [ForeignKey("Chatbot")]
        public int chatID { get; set; }
        public Chatbot? Chatbots { get; set; }
        public Boolean isSend { get; set; }
        public DateTime? chatTime { get; set; }
        public string content { get; set; }
        public contentChatbot()
        {
            chatTime = DateTime.Now;
        }
    }
}
