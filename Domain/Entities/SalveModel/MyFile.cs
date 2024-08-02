using Domain.Entities.Enum;

namespace Domain.Entities.SalveModel {
    public record MyFile {
        public double Value { get; set; }

        public FileUnit Unit { get; set; }

        public MyFile() { }

        public MyFile(long value) {
            switch (value) {
                case < 1024:    //小于 1Kb
                    Value = value;
                    Unit = FileUnit.Bytes;
                    break;
                case < (long)1024 * 1024:     //小于 1Mb
                    Value = (long)Math.Round((double)value / 1024, 2);
                    Unit = FileUnit.KB;
                    break;
                case < (long)1024 * 1024 * 1024:  //小于 1Gb
                    Value = (long)Math.Round((double)value / (1024 * 1024), 2);
                    Unit = FileUnit.MB;
                    break;
                case < (long)1024 * 1024 * 1024 * 1024:    //小于 1Tb
                    Value = (long)Math.Round((double)value / (1024 * 1024 * 1024), 2);
                    Unit = FileUnit.GB;
                    break;
                default:    //大于 1Tb
                    Value = (long)Math.Round((double)value / 1024 * 1024 * 1024 * 1024, 2);
                    Unit = FileUnit.TB;
                    break;
            }
        }

        public MyFile(long value, FileUnit unit) {
            Value = value;
            Unit = unit;
        }
    }
}
