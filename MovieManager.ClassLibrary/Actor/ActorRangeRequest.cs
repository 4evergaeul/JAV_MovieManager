using System;
using System.Collections.Generic;
using System.Text;

namespace MovieManager.ClassLibrary
{
    public class ActorRangeRequest
    {
        public int HeightUpper { get; set; }
        public int HeightLower { get; set; }
        public string CupUpper { get; set; }
        public string CupLower { get; set; }
        public decimal BustLower { get; set; }
        public decimal BustUpper { get; set; }
        public decimal WaistLower { get; set; }
        public decimal WaistUpper { get; set; }
        public decimal HipsLower { get; set; }
        public decimal HipsUpper { get; set; }
        public decimal LooksLower { get; set; }
        public decimal LooksUpper { get; set; }
        public decimal BodyLower { get; set; }
        public decimal BodyUpper { get; set; }
        public decimal SexAppealLower { get; set; }
        public decimal SexAppealUpper { get; set; }
        public decimal OverallLower { get; set; }
        public decimal OverallUpper { get; set; }
        public int Age { get; set; }

    }
}
