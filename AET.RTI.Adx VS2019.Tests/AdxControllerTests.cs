using System;
using System.Collections.Generic;
using System.Linq;
using AET.Unity.SimplSharp;
using AET.Unity.SimplSharp.Timer;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AET.RTI.ADx.Tests {
  [TestClass]
  public class AdxControllerTests {
    private readonly AdxController adx = new AdxController();
    private readonly TestTimer timer = new TestTimer { ElapseImmediately = true};
    private readonly List<string> lastCommands = new List<string>();

    private string LastCommand {
      get { return lastCommands.LastOrDefault(); }
    }

    [TestInitialize]
    public void Setup() {
      adx.Mutex = new TestMutex();
      adx.Timer = timer;
      adx.Tx = s => lastCommands.Add(s.ToString());
      for(var i = 1; i <= 16; i++) adx.zonePowerStatus[i] = true;
    }

    [TestMethod]
    [DataRow(3, 0, "ZN03VOL70")]
    [DataRow(3, 20, "ZN03VOL69")]
    [DataRow(10, 32767, "ZN10VOL35")]
    [DataRow(16, 64500, "ZN16VOL01")]
    [DataRow(16, 65535, "ZN16VOL00")]

    public void Volume_SendsCorrectCommand(int zone, int level, string expected) {
      adx.Volume((ushort)zone, (ushort)level);
      LastCommand.Should().Be("*"+expected+"\r");
    }

    [TestMethod]
    [DataRow(0,70)]
    [DataRow(32767, 35)]
    [DataRow(65535,0)]
    public void ConvertVolumeLevelToRti_ReturnCorrectLevel(int inLevel, int outLevel) {
      AdxController.ConvertVolumeLevelToRti((ushort)inLevel).Should().Be(outLevel);
    }

    [TestMethod]
    [DataRow(0, 0)]
    [DataRow(-200, 32)]
    [DataRow(-120, 32)]
    [DataRow(-109, 30)]
    [DataRow(-11, 22)]
    [DataRow(11, 2)]
    [DataRow(200, 12)]
    [DataRow(120, 12)]
    [DataRow(109, 10)]
    public void ConvertTrebleBassToRti_ReturnsCorrectLevel(int inLevel, int outLevel) {
      AdxController.ConvertTrebleBassToRti((short)inLevel).Should().Be(outLevel);
    }

    [TestMethod]
    [DataRow(3, 0, "ZN03TRB00")]
    [DataRow(3, 60, "ZN03TRB06")]
    [DataRow(10, -51, "ZN10TRB26")]
    [DataRow(16, -120, "ZN16TRB32")]
    [DataRow(16, 120, "ZN16TRB12")]
    [DataRow(16, 30, "ZN16TRB04")]
    public void Treble_SendsCorrectCommand(int zone, int level, string expected) {
      adx.Treble((ushort)zone, (short)level);
      LastCommand.Should().Be("*" + expected + "\r");
    }

    [TestMethod]
    public void Bass_SendsCorrectCommand() {
      adx.Bass(3, -120);
      LastCommand.Should().Be("*ZN03BAS32\r");
    }

    [TestMethod]
    [DataRow(1,3, "ZN01SRC03")]
    [DataRow(12, 16, "ZN12SRC16")]
    public void Source_SendsCorrectCommand(int zone, int input, string expected) {
      adx.Source((ushort)zone, (ushort)input);
      LastCommand.Should().Be("*"+expected+"\r");
    }

    [TestMethod]
    public void Source_ZoneIsOff_TurnsOnZone() {
      adx.zonePowerStatus[3] = false;
      adx.Source(3, 3);
      lastCommands[0].Should().Be("*ZN03PWR01\r");
      lastCommands[1].Should().Be("*ZN03SRC03\r");
    }

    [TestMethod]
    public void Source_SetToZero_TurnsOffZone() {
      adx.Source(3, 0);
      LastCommand.Should().Be("*ZN03PWR00\r");
    }

    [TestMethod]
    public void PowerOff_SendsCorrectCommand() {
      adx.PowerOff(9);
      LastCommand.Should().Be("*ZN09PWR00\r");
    }

    [TestMethod]
    public void PowerOn_SendsCorrectCommand() {
      adx.PowerOn(16);
      LastCommand.Should().Be("*ZN16PWR01\r");
    }

    [TestMethod]
    public void Mute_SendsCorrectCommand() {
      adx.Mute(3,1);
      LastCommand.Should().Be("*ZN03MUT01\r");
      adx.Mute(3, 0);
      LastCommand.Should().Be("*ZN03MUT00\r");
    }

    [TestMethod]
    public void ZoneIsOff_VolumeMuteTrebAndBass_ShouldNotSend() {
      adx.zonePowerStatus[1] = false;
      adx.Mute(1,1);
      adx.Volume(1,100);
      adx.Treble(1,-30);
      adx.Bass(1, -30);
      lastCommands.Count.Should().Be(0);
    }
  }
}