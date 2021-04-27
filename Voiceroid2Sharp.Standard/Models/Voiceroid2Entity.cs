namespace Voiceroid2Sharp.Standard.Models
{
    public class Voiceroid2Entity
    {
        public string VoiceroidName { get; set; }
        public string Command { get; set; }
        public Voiceroid2Entity(string name, string command)
        {
            this.VoiceroidName = name;
            this.Command = command;
        }
    }
}
