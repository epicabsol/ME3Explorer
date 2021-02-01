﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using ME3ExplorerCore.Packages;

namespace ME3ExplorerCore.Dialogue
{
    [DebuggerDisplay("SpeakerExtended {SpeakerID} {SpeakerName}")]

    public class SpeakerExtended : INotifyPropertyChanged
    {
        public int SpeakerID { get; set; }
        public string SpeakerName { get; set; }
        /// <summary>
        /// Male UIndex object reference
        /// </summary>
        public IEntry FaceFX_Male { get; set; }
        /// <summary>
        /// Female UIndex object reference
        /// </summary>
        public IEntry FaceFX_Female { get; set; }
        public int StrRefID { get; set; }
        public string FriendlyName { get; set; }

        public SpeakerExtended(int SpeakerID, string SpeakerName)
        {
            this.SpeakerID = SpeakerID;
            this.SpeakerName = SpeakerName;
        }

        public SpeakerExtended(int SpeakerID, string SpeakerName, IEntry FaceFX_Male, IEntry FaceFX_Female, int StrRefID, string FriendlyName)
        {
            this.SpeakerID = SpeakerID;
            this.SpeakerName = SpeakerName;
            this.FaceFX_Male = FaceFX_Male;
            this.FaceFX_Female = FaceFX_Female;
            this.StrRefID = StrRefID;
            this.FriendlyName = FriendlyName;
        }

#pragma warning disable
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore
    }

}
