using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace Voiceroid2Sharp.Test
{
    public class Tests
    {
        private Voiceroid2Sharp voiceroid2Sharp_;

        [SetUp]
        public void Setup()
        {
            this.voiceroid2Sharp_ = new Voiceroid2Sharp();
            this.voiceroid2Sharp_.ConnectV2(true);
        }

        [Test]
        public void ReadOneHandredCommment()
        {
            while (!this.voiceroid2Sharp_.IsV2Connected) {
                Thread.Sleep(1000);
            }

            for (int i = 0; i < 100; i++) {
                this.voiceroid2Sharp_.Messages.Add($"ƒRƒƒ“ƒg‚»‚Ì{i}");
            }
        }
    }
}