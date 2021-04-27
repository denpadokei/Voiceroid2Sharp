using Codeer.Friendly.Dynamic;
using NUnit.Framework;
using RM.Friendly.WPFStandardControls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Voiceroid2Sharp.Standard;

namespace Voiceroid2Sharp.Test
{
    public class Tests
    {
        private Voiceroid2 voiceroid2Sharp;

        [SetUp]
        public void Setup()
        {
            this.voiceroid2Sharp = new Voiceroid2();
            this.voiceroid2Sharp.Connect(true);
        }

        [Test]
        public void ReadOneHandredCommment()
        {
            for (int i = 0; i < 100; i++) {
                this.voiceroid2Sharp.Talk($"ƒRƒƒ“ƒg‚»‚Ì{i}");
            }
        }
    }
}