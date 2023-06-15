using System;
using System.Text;
using Crestron.SimplSharp;
using AET.Unity.SimplSharp;
using AET.Unity.SimplSharp.Timer;
using Crestron.SimplSharp.Ssh.Common;

namespace AET.RTI.ADx {
  public class AdxController {
    private TxQueue<string> queue;
    private const int spaceAfterVolumeMs = 15;
    public AnyKeyDictionary<int, bool> zonePowerStatus = new AnyKeyDictionary<int, bool>();
    public AdxController() {
      SetupDelegatePlaceholders();
      queue = new TxQueue<string>(s => Tx(s), 30);
    }

    private void SetupDelegatePlaceholders() {
      Tx = delegate { };
    }

    internal IMutex Mutex {
      get { return queue.Mutex; }
      set { queue.Mutex = value; }
    }

    internal ITimer Timer {
      get { return queue.Timer; }
      set { queue.Timer = value; }
    }

    private void Send(string value) {
      queue.Send(FormatCommand(value));
    }

    private void SendLowPriority(string value, string category) {
      queue.SendLowPriority(FormatCommand(value), category);
    }

    private void SendLowPriority(string value, string category, int spaceAfterCommand) {
      queue.SendLowPriority(FormatCommand(value), category, spaceAfterCommand);
    }


    private string FormatCommand(string value) {
      return string.Format("*{0}\r", value);
    }

    public void PowerOn(ushort zone) {
      zonePowerStatus[zone] = true;
      Send(string.Format("ZN{0:00}PWR01", zone));
    }

    public void PowerOff(ushort zone) {
      zonePowerStatus[zone] = false;
      Send(string.Format("ZN{0:00}PWR00", zone));
    }

    public void Source(ushort zone, ushort input) {
      if (input == 0) {
        PowerOff(zone);
        return;
      }
      if (!zonePowerStatus[zone]) PowerOn(zone);
      Send(string.Format("ZN{0:00}SRC{1:00}", zone, input));
    }

    public void Volume(ushort zone, ushort level) {
      SendLowPriority(string.Format("ZN{0:00}VOL{1:00}", zone, ConvertVolumeLevelToRti(level)),"vol" + zone, spaceAfterVolumeMs);
    }

    public void Treble(ushort zone, short level) {
      SendLowPriority(string.Format("ZN{0:00}TRB{1:00}", zone, ConvertTrebleBassToRti(level)), "tre" + zone, spaceAfterVolumeMs);
    }

    public void Bass(ushort zone, short level) {
      SendLowPriority(string.Format("ZN{0:00}BAS{1:00}", zone, ConvertTrebleBassToRti(level)), "bas" + zone, spaceAfterVolumeMs);
    }

    #region Scalers
    internal static int ConvertVolumeLevelToRti(ushort level) {
      return Math.Abs(((level - 65535) * 70) / 65535);
    }

    internal static int ConvertTrebleBassToRti(short level) {
      decimal scaledLevel = level.Clamp(-120, 120);
      if(scaledLevel < 0) scaledLevel = (Math.Round(Math.Abs(scaledLevel) / 20) * 2) +20;
      else scaledLevel = Math.Round(scaledLevel / 20) * 2;
      return ((int)scaledLevel);
    }
    #endregion

    public void Mute(ushort zone, ushort state) {
      Send(string.Format("ZN{0:00}MUT{1:00}", zone, state));
    }

    public SetStringOutputDelegate Tx { get; set; }
  }
}