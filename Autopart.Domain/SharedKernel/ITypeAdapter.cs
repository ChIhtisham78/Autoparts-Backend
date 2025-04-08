namespace Autopart.Domain.SharedKernel
{
    /// <summary>
    /// Base contract for map aggregate to dto.
    /// <remarks>
    /// This is a  contract for work with "auto" mappers ( automapper,mapster,emitmapper,valueinjecter...)
    /// or adhoc mappers
    /// </remarks>
    /// </summary>
    public interface ITypeAdapter
    {
        /// <summary>
        /// Adapt a source object to an instance of type <typeparamref name="TTarget"/>
        /// </summary>
        /// <typeparam name="TSource">Type of source item</typeparam>
        /// <typeparam name="TTarget">Type of target item</typeparam>
        /// <param name="source">Instance to adapt</param>
        /// <returns><paramref name="source"/> mapped to <typeparamref name="TTarget"/></returns>
        TTarget Adapt<TSource, TTarget>(TSource source)
            where TTarget : class
            where TSource : class;

        /// <summary>
        /// Adapt a source object to an existing instance of type <typeparamref name="TTarget"/>
        /// </summary>
        /// <typeparam name="TSource">Type of source item</typeparam>
        /// <typeparam name="TTarget">Type of target item</typeparam>
        /// <param name="source">Instance to adapt from</param>
        /// <param name="destination">Instance to adapt to</param>
        /// <returns><paramref name="source"/> mapped to <typeparamref name="TTarget"/></returns>
        TTarget Adapt<TSource, TTarget>(TSource source, TTarget destination)
            where TTarget : class
            where TSource : class;

        /// <summary>
        /// Adapt a source object to an instance of type <typeparamref name="TTarget"/>
        /// </summary>
        /// <typeparam name="TTarget">Type of target item</typeparam>
        /// <param name="source">Instance to adapt</param>
        /// <returns><paramref name="source"/> mapped to <typeparamref name="TTarget"/></returns>
        TTarget Adapt<TTarget>(object source)
            where TTarget : class;

        /// <summary>
        /// Adapt a source object to an existing instance of type <typeparamref name="TTarget"/>
        /// </summary>
        /// <typeparam name="TTarget">Type of target item</typeparam>
        /// <param name="source">Instance to adapt from</param>
        /// <param name="destination">Instance to adapt to</param>
        /// <returns><paramref name="source"/> mapped to <typeparamref name="TTarget"/></returns>
        TTarget Adapt<TTarget>(object source, TTarget destination)
            where TTarget : class;
    }
}
