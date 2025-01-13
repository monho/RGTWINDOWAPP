using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGT.Core.Entities
{
    public class OrderData
    {
        /*
            2025.01.13 문준호
            RGT과제 작업
            주문 정보 엔티티 작업
         */
        public int OrderId { get; set; }
        public string TableNumber { get; set; }
        public List<string> OrderItems { get; set; }
        public DateTime OrderTime { get; set; }
    }
}
