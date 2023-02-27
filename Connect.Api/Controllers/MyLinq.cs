namespace Connect.Api.Controllers
{
    public static class MyLinq
    {
        public static IEnumerable<T> MyWhere<T>(
            this IEnumerable<T> src,
            Func<T, bool> func)
        {
            foreach (var item in src)
            {
                if (func(item))
                {
                    yield return item;  
                }
            }
        }
    }
}