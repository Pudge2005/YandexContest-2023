namespace DevourDev.SheetCodeUtility
{
    public static class ListsPool<T>
    {
        private class ListsPoolHelper
        {
            private readonly object _lockObj = new();

            private readonly Stack<List<T>> _stack;


            public ListsPoolHelper(int initialCapacity = 0)
            {
                _stack = new(initialCapacity);
            }


            public List<T> Rent(int minCapacity = 0)
            {
                lock (_lockObj)
                {
                    if (!_stack.TryPop(out var item))
                    {
                        item = new List<T>(minCapacity);
                    }

                    return item;
                }
            }

            public void Return(List<T> item)
            {
                lock (_lockObj)
                {
                    item.Clear();
                    _stack.Push(item);
                }
            }
        }


        private const int _largeCount = 1000_000;
        private const int _mediumCount = 10_000;
        private const int _smallCount = 512;

        private static readonly ListsPoolHelper _largeListsPool = new();
        private static readonly ListsPoolHelper _mediumListsPool = new();
        private static readonly ListsPoolHelper _smallListsPool = new();
        private static readonly ListsPoolHelper _mimicListsPool = new();


        public static List<T> Rent(int minCapacity = 0)
        {
            var pool = GetPool(minCapacity);
            return pool.Rent(minCapacity);
        }

        public static void Return(List<T> item)
        {
            var pool = GetPool(item.Capacity);
            pool.Return(item);
        }

        private static ListsPoolHelper GetPool(int minCapacity)
        {
            return minCapacity switch
            {
                >= _largeCount => _largeListsPool,
                >= _mediumCount => _mediumListsPool,
                >=_smallCount => _smallListsPool,
                _ => _mimicListsPool,
            };
        }
    }
}
