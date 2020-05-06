using System;
using System.Collections.Generic;
using System.Text;

namespace Voiceroid2Sharp
{
    public class CommentEntity
    {
        public DateTime SendDate { get; private set; }
        public string Message { get; set; }

        public CommentEntity(string message)
        {
            this.SendDate = DateTime.Now;
            this.Message = message;
        }
    }
}
