﻿using System;

namespace Voiceroid2Sharp.Standard.Models
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
