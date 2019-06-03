﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ME3Explorer.SharedUI;
using ME3Explorer;
using ME3Explorer.Packages;
using ME3Explorer.Unreal;

namespace ME3Explorer.Matinee
{
    public class InterpGroup : NotifyPropertyChangedBase
    {
        public string GroupName { get; set; }

        public ObservableCollectionExtended<InterpTrack> Tracks { get; } = new ObservableCollectionExtended<InterpTrack>();

        public InterpGroup(IExportEntry export)
        {
            GroupName = export.GetProperty<NameProperty>("GroupName")?.Value ?? "Group";
            var tracksProp = export.GetProperty<ArrayProperty<ObjectProperty>>("InterpTracks");
            if (tracksProp != null)
            {
                var trackExports = tracksProp.Where(prop => export.FileRef.isUExport(prop.Value)).Select(prop => export.FileRef.getUExport(prop.Value));
                foreach (IExportEntry trackExport in trackExports)
                {
                    if (trackExport.inheritsFrom("BioInterpTrack"))
                    {
                        Tracks.Add(new BioInterpTrack(trackExport));
                    }
                    else if (trackExport.inheritsFrom("InterpTrackFloatBase"))
                    {
                        Tracks.Add(new InterpTrackFloatBase(trackExport));
                    }
                    else if (trackExport.inheritsFrom("InterpTrackVectorBase"))
                    {
                        Tracks.Add(new InterpTrackVectorBase(trackExport));
                    }
                    else if (trackExport.inheritsFrom("InterpTrackEvent"))
                    {
                        Tracks.Add(new InterpTrackEvent(trackExport));
                    }
                    else if (trackExport.inheritsFrom("InterpTrackFaceFX"))
                    {
                        Tracks.Add(new InterpTrackFaceFX(trackExport));
                    }
                    else if (trackExport.inheritsFrom("InterpTrackAnimControl"))
                    {
                        Tracks.Add(new InterpTrackAnimControl(trackExport));
                    }
                    else if (trackExport.inheritsFrom("InterpTrackMove"))
                    {
                        Tracks.Add(new InterpTrackMove(trackExport));
                    }
                    else if (trackExport.inheritsFrom("InterpTrackVisibility"))
                    {
                        Tracks.Add(new InterpTrackVisibility(trackExport));
                    }
                    else if (trackExport.inheritsFrom("InterpTrackToggle"))
                    {
                        Tracks.Add(new InterpTrackToggle(trackExport));
                    }
                    else if (trackExport.inheritsFrom("InterpTrackWwiseEvent"))
                    {
                        Tracks.Add(new InterpTrackWwiseEvent(trackExport));
                    }
                    else if (trackExport.inheritsFrom("InterpTrackDirector"))
                    {
                        Tracks.Add(new InterpTrackDirector(trackExport));
                    }
                    else
                    {
                        throw new FormatException($"Unknown Track Type: {trackExport.ClassName}");
                    }
                }
            }
        }
    }

    public abstract class InterpTrack : NotifyPropertyChangedBase
    {
        public string TrackTitle { get; set; }

        public ObservableCollectionExtended<Key> Keys { get; } = new ObservableCollectionExtended<Key>();

        protected InterpTrack(IExportEntry export)
        {
            TrackTitle = export.ObjectName;
        }
    }

    public class BioInterpTrack : InterpTrack
    {
        public BioInterpTrack(IExportEntry exp) : base(exp)
        {
            var trackKeys = exp.GetProperty<ArrayProperty<StructProperty>>("m_aTrackKeys");
            if (trackKeys != null)
            {
                foreach (StructProperty bioTrackKey in trackKeys)
                {
                    var fTime = bioTrackKey.GetProp<FloatProperty>("fTime");
                    Keys.Add(new Key(fTime));
                }
            }
        }

    }
    public class InterpTrackFloatBase : InterpTrack
    {
        public InterpTrackFloatBase(IExportEntry export) : base(export)
        {
        }
    }
    public class InterpTrackVectorBase : InterpTrack
    {
        public InterpTrackVectorBase(IExportEntry export) : base(export)
        {
        }
    }
    public class InterpTrackEvent : InterpTrack
    {
        public InterpTrackEvent(IExportEntry export) : base(export)
        {
        }
    }
    public class InterpTrackFaceFX : InterpTrack
    {
        public InterpTrackFaceFX(IExportEntry export) : base(export)
        {
        }
    }
    public class InterpTrackAnimControl : InterpTrack
    {
        public InterpTrackAnimControl(IExportEntry export) : base(export)
        {
        }
    }
    public class InterpTrackMove : InterpTrack
    {
        public InterpTrackMove(IExportEntry export) : base(export)
        {
        }
    }
    public class InterpTrackVisibility : InterpTrack
    {
        public InterpTrackVisibility(IExportEntry export) : base(export)
        {
        }
    }
    public class InterpTrackToggle : InterpTrack
    {
        public InterpTrackToggle(IExportEntry export) : base(export)
        {
        }
    }
    public class InterpTrackWwiseEvent : InterpTrack
    {
        public InterpTrackWwiseEvent(IExportEntry export) : base(export)
        {
        }
    }
    public class InterpTrackDirector : InterpTrack
    {
        public InterpTrackDirector(IExportEntry export) : base(export)
        {
        }
    }
}
