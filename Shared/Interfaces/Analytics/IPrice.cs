using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interfaces.Analytics {
    public class IPrice {
        public uint Id { get; set; }
        public double Price { get; set; }
        public ILocation Location { get; set; }
        public ICategory Category { get; set; }
        public IMatrix Matrix { get; set; }
    }
}
