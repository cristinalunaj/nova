﻿using System;
using System.Collections.Generic;

namespace ssi
{
    public class AnnoListItem : MyListItem
    {
        private double start;
        private double duration;
        private String label;
        private String meta;
        private String tier;
        private String bg;
        private double confidence;

        public double Start
        {
            get { return start; }
            set
            {
                duration = Math.Max(0, duration + start - value);
                start = value;
                OnPropertyChanged("Start");
                OnPropertyChanged("Duration");
            }
        }

        public double Stop
        {
            get { return start + duration; }
            set
            {
                duration = Math.Max(0, value - start);
                OnPropertyChanged("Stop");
                OnPropertyChanged("Duration");
            }
        }

        public double Duration
        {
            get { return duration; }
            set
            {
                duration = Math.Max(0, value);
                OnPropertyChanged("Stop");
                OnPropertyChanged("Duration");
            }
        }

        public String Label
        {
            get { return label; }
            set
            {
                label = value;
                OnPropertyChanged("Label");
            }
        }

        public double Confidence
        {
            get { return confidence; }
            set
            {
                confidence = value;
                OnPropertyChanged("Confidence");
            }
        }

        public String Tier
        {
            get { return tier; }
            set
            {
                tier = value;
                OnPropertyChanged("Tier");
            }
        }

        public String Meta
        {
            get { return meta; }
            set
            {
                meta = value;
                OnPropertyChanged("Meta");
            }
        }

        public String Bg
        {
            get { return bg; }
            set
            {
                bg = value;
                OnPropertyChanged("Bg");
            }
        }

        public AnnoListItem(double _start, double _duration, String _label, String _meta = "", String _bg = "#000000", double _confidence = 1.0)
        {
            start = _start;
            duration = Math.Max(0, _duration);
            label = _label;
            meta = _meta;
            bg = _bg;
            confidence = _confidence;
        }

        public class AnnoListItemComparer : IComparer<AnnoListItem>
        {
            int IComparer<AnnoListItem>.Compare(AnnoListItem a, AnnoListItem b)
            {
                if (a.start < b.start)
                {
                    return -1;
                }
                else if (a.start > b.start)
                {
                    return 1;
                }
                else
                {
                    if (a.duration < b.duration)
                    {
                        return -1;
                    }
                    else if (a.duration < b.duration)
                    {
                        return 1;
                    }
                }

                return 0;
            }            
        }
    }
}