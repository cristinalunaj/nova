﻿using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace ssi
{
    public class AnnoListItem : IObservableListItem
    {
        private double start;
        private double duration;
        private string label;
        private double score;
        private string meta;
        private Color color;
        private double confidence;
        private PointList points;
        private PolygonList polygonList;

        public enum TYPE
        {
            POINT,
            POLYGON,
            NONE
        }

        public TYPE Type { get; set; }        

        public PointList Points
        {
            get
            {
                return points;
            }
            set
            {
                points = value;
                OnPropertyChanged("Points");
            }
        }

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

        public string Label
        {
            get { return label; }
            set
            {
                label = value;
                OnPropertyChanged("Label");
            }
        }

        public double Score
        {
            get { return score; }
            set
            {
                score = value;
                OnPropertyChanged("Score");
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


        public string Meta
        {
            get { return meta; }
            set
            {
                meta = value;
                OnPropertyChanged("Meta");
            }
        }

        public Color Color
        {
            get { return color; }
            set
            {
                color = value;
                OnPropertyChanged("Color");
            }
        }

        public PolygonList PolygonList { get => polygonList; set => polygonList = value; }

        public AnnoListItem(double start, double duration, string label, string meta = "", Color color = new Color(), double confidence = 1.0, TYPE type = TYPE.NONE, PointList points = null, PolygonList polygonList = null)
        {
            this.start = Math.Max(0, start);
            this.duration = Math.Max(0, duration);
            this.label = label;
            this.score = double.NaN;
            this.meta = meta;
            this.color = color;
            this.confidence = confidence;
            this.Type = type;
            if (this.Type == TYPE.POINT)
            {
                if (points != null)
                {
                    this.points = points;
                }
            }

            this.polygonList = polygonList;
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