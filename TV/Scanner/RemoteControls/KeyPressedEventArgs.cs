using System;
using System.Configuration;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;

namespace FutureConcepts.Media.TV.Scanner.RemoteControls
{
    public enum KeyName
    {
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
    	Zero = 0,
        Asterisk = 10,
        Red = 11,
        Radio = 12,
        Menu = 13,
        Pound = 14,
        Mute = 15,
        VolumeUp = 16,
        VolumeDown = 17,
        PreviousChannel = 18,
        Up = 20,
        Down = 21,
        Left = 22,
        Right = 23,
        Videos = 24,
        Music = 25,
        Pictures = 26,
        Guide = 27,
        Tv = 28,
        Skip = 30,
        BackExit = 31,
        ChannelUp = 32,
        ChannelDown = 33,
        Replay = 36,
        Ok = 37,
        Blue = 41,
        Green = 46,
        Pause = 48,
        Rewind = 50,
        FastForward = 52,
        Play = 53,
        Stop = 54,
        Record = 55,
        Yellow = 56,
        Go = 59,
        Power = 61
    }

    public class KeyPressedEventArgs : EventArgs
    {
        private KeyName _keyName;

        public KeyName PressedKeyName
        {
            get
            {
                return _keyName;
            }
            set
            {
                _keyName = value;
            }
        }

        private DateTime _attackTime;

        public DateTime AttackTime
        {
            get
            {
                return _attackTime;
            }
            set
            {
                _attackTime = value;
            }
        }
    }
}
