namespace Iguagile
{
    internal class ObjectIdGenerator
    {
        private readonly object _idLock = new object();
        private byte[] _allocatedId;
        private int _nextTryId;
        private int _allocatedIdCount;
        private int _maxSize;

        private const int bits = 8;

        public ObjectIdGenerator(int maxSize)
        {
            _allocatedId = new byte[maxSize / 8 + 1];
            _maxSize = maxSize;
        }

        public int Generate()
        {
            lock (_idLock)
            {
                while (true)
                {
                    if (_allocatedIdCount >= _maxSize)
                    {
                        return -1;
                    }

                    if (_nextTryId >= _maxSize)
                    {
                        _nextTryId = 0;
                    }

                    if (!IsAllocated(_nextTryId))
                    {
                        var id = _nextTryId;
                        Allocate(id);
                        _nextTryId++;
                        return id;
                    }

                    _nextTryId++;
                }
            }
        }

        public void Free(int id)
        {
            if (id > _maxSize)
            {
                return;
            }

            lock (_idLock)
            {
                var index = id / bits;
                var b = _allocatedId[index];
                var shift = (byte) (id % bits);
                var mask = (byte) (1 << shift);
                var flag = b & ~mask;
                _allocatedId[index] = (byte)flag;
                _allocatedIdCount--;
            }
        }

        public void FreeAll()
        {
            lock (_idLock)
            {
                var max = _allocatedId.Length;
                for (var i = 0; i < max; i++)
                {
                    _allocatedId[i] = 0;
                }

                _nextTryId = 0;
                _allocatedIdCount = 0;
            }
        }

        private bool IsAllocated(int id)
        {
            var index = id / bits;
            var b = _allocatedId[index];
            var shift = (byte) (id % bits);
            var mask = (byte) (1 << shift);
            var flag = b & mask;
            return flag != 0;
        }

        private void Allocate(int id)
        {
            var index = id / bits;
            var b = _allocatedId[index];
            var shift = (byte) (id % bits);
            var mask = (byte) (1 << shift);
            var flag = b | mask;
            _allocatedId[index] = (byte) flag;
            _allocatedIdCount++;
        }
    }

}