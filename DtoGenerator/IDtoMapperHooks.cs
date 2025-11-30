namespace DtoGenerator
{ 
    /// <summary>
    /// 强制 DTO 实现的映射钩子接口
    /// </summary>
    /// <typeparam name="TEntity">源实体类型</typeparam>
    public interface IDtoMapperHooks<TEntity>
    {
        /// <summary>
        /// 当 DTO 从 Entity 创建完成后调用（用于自定义正向映射逻辑）
        /// </summary>
        void OnDtoCreated(TEntity sourceEntity);

        /// <summary>
        /// 当 Entity 从 DTO 创建完成后调用（用于自定义反向映射逻辑）
        /// </summary>
        void OnEntityCreated(TEntity targetEntity);
    }
}