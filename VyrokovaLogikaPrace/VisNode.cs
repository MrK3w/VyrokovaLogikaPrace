using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VyrokovaLogikaPrace
{
    public class VisNode
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public int ParentId { get; set; }
        public int TruthValue { get; set; }
        public string Operator { get; set; }
        public bool Contradiction { get; set; }
        public bool IsChanged { get; set; }
    }
}
