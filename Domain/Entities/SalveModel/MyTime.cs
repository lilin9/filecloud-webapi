using Domain.Entities.Enum;

namespace Domain.Entities.SalveModel {
    public record MyTime() {
        public long Value { get; set; } = 0;
        public TimeUnit Unit { get; set; } = TimeUnit.Seconds;
    }
}
