using System.Collections.Generic;

namespace TheWall.Models
{
    public class WrapperModel 
    {
        public List<Message> AllMessages {get;set;}
        public Message NewMessage {get;set;}
        public Comment NewComment {get;set;}
    }
}