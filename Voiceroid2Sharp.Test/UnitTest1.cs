using NUnit.Framework;
using RM.Friendly.WPFStandardControls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Voiceroid2Sharp.Test
{
    public class Tests
    {
        private Voiceroid2 voiceroid2Sharp_;

        [SetUp]
        public async Task Setup()
        {
            this.voiceroid2Sharp_ = new Voiceroid2();
            await this.voiceroid2Sharp_.ConnectV2(true);
        }

        [Test]
        public async Task ReadOneHandredCommment()
        {
            for (int i = 0; i < 100; i++) {
                this.voiceroid2Sharp_.Message = $"コメントその{i}";
            }
            while ((this.voiceroid2Sharp_.Messages.Any() || this.voiceroid2Sharp_.IsTalking) && this.voiceroid2Sharp_.IsOpen) {
                Console.WriteLine($"isOpen : {this.voiceroid2Sharp_.IsOpen}");
                await Task.Delay(500);
            }
            Console.WriteLine(this.voiceroid2Sharp_.Log);
        }

        [Test]
        public void StartV2()
        {
            Console.WriteLine(this.voiceroid2Sharp_.Log);
            while (this.voiceroid2Sharp_.IsOpen) ;
        }

        [Test]
        public void CloseV2()
        {
            var isClose = false;

            this.voiceroid2Sharp_.OnExitVoiceroid2 += (s) =>
            {
                Console.WriteLine(s);
                isClose = true;
            };

            while (!isClose) ;
        }

        [Test]
        public void EnumrateTextView()
        {
            for (int i = 0; i < this.voiceroid2Sharp_.TextViewCollextion.Count; i++) {
                if (this.voiceroid2Sharp_.TextViewCollextion[i].ToString().Contains("TextBlock")) {
                    Console.WriteLine($"アイテムID{i}:{this.voiceroid2Sharp_.TextViewCollextion[i]}");
                    var textBlock = new WPFTextBlock(this.voiceroid2Sharp_.TextViewCollextion[i]);
                    Console.WriteLine(textBlock.Text);
                }
                else if (this.voiceroid2Sharp_.TextViewCollextion[i].ToString().Contains("Button")) {
                    Console.WriteLine($"アイテムID{i}:{this.voiceroid2Sharp_.TextViewCollextion[i]}");
                }
            }
        }
    }
}