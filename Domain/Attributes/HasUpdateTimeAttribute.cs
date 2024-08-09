namespace Domain.Attributes {
    /// <summary>
    /// 在类属性上标注上此特性，在每次修改实体时，都会自动更新标注上这个特性的属性值，
    /// 将其更新为最新更改时间
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class HasUpdateTimeAttribute: Attribute {
    }
}
