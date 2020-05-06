using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace Voiceroid2Sharp.Test
{
    public class Tests
    {
        private Voiceroid2 voiceroid2Sharp_;

        [SetUp]
        public void Setup()
        {
            this.voiceroid2Sharp_ = new Voiceroid2();
            this.voiceroid2Sharp_.ConnectV2(true);
            Thread.Sleep(5000);
        }

        [Test]
        public void ReadOneHandredCommment()
        {
            while (!this.voiceroid2Sharp_.IsV2Connected) {
                Thread.Sleep(1000);
            }

            for (int i = 0; i < 100; i++) {
                this.voiceroid2Sharp_.Message = $"ƒRƒƒ“ƒg‚»‚Ì{i}";
                Thread.Sleep(1000);
            }
        }
    }
}