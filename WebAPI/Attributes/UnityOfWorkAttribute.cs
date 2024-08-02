namespace WebAPI.Attributes
{
    /// <summary>
    /// 标注此注解的方法，将会自动执行 dbContext.SaveChanges() 操作
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class UnityOfWorkAttribute(params Type[] dbContextTypes) : Attribute
    {
        public Type[] DbContextTypes { get; } = dbContextTypes;
    }
}
