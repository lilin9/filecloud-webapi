using Domain.Entities.SalveModel;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities {
    public class Disk {
        public Disk() { }

        public Disk( MyFile totalCapacity, MyFile usedCapacity, MyFile remainingCapacity) {
            TotalCapacity = totalCapacity;
            UsedCapacity = usedCapacity;
            RemainingCapacity = remainingCapacity;
        }


        /// <summary>
        /// 磁盘总容量，必须
        /// </summary>
        [Comment("磁盘总容量，必须")]
        public MyFile TotalCapacity { get; set; }

        /// <summary>
        /// 磁盘使用容量，必选
        /// </summary>
        [Comment("磁盘使用容量，必选")]
        public MyFile UsedCapacity { get; set; }

        /// <summary>
        /// 磁盘剩余容量，必选
        /// </summary>
        [Comment("磁盘剩余容量，必选")]
        public MyFile RemainingCapacity { get; set; }
    }
}
