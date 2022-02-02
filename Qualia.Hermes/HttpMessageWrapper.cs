namespace Qualia.Hermes
{
    public abstract class HttpMessageWrapper<TInner, TOutter> 
        where TInner : class, new()
        where TOutter : HttpMessageWrapper<TInner, TOutter>, new()
    {
        public TInner Message { get; private set; }
        protected HttpMessageWrapper()
        { }
        public static TOutter From(TInner msg)
        {
            var outter = new TOutter();
            outter.Message = msg;
            return outter;
        }
    }
}