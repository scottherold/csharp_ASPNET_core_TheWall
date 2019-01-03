using System.Collections.Generic;

namespace TheWall.Models
{
    // Model bundle for Messages/Comments
    public class MessageBundle
    {
        // Class Instances
        public Account account { get; set; }
        public Message message {  get; set; }
        public Comment comment { get; set; }

        // Linked Lists
        public List<Message> messageList { get; set; }
    }
}