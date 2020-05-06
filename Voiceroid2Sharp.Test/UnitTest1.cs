using NUnit.Framework;
using RM.Friendly.WPFStandardControls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                this.voiceroid2Sharp_.Message = $"コメントその{i}";
            }
        }

        [Test]
        public void StartV2()
        {
            this.voiceroid2Sharp_.ConnectV2();
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