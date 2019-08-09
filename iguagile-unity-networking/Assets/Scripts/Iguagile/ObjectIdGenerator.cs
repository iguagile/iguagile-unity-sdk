using System.Collections;

namespace Iguagile
{
    internal class ObjectIdGenerator
    {
        private readonly object _idLock = new object();
        private BitArray _allocatedId;
        private int _nextTryId;
        private int _allocatedIdCount;
        private int _maxSize;
        
        public ObjectIdGenerator(int maxSize)
        {
            _allocatedId = new BitArray(maxSize);
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
                _allocatedId[id] = false;
                _allocatedIdCount--;
            }
        }

        public void FreeAll()
        {
            lock (_idLock)
            {
                _allocatedId.SetAll(false);

                _nextTryId = 0;
                _allocatedIdCount = 0;
            }
        }

        private bool IsAllocated(int id)
        {
            return _allocatedId[id];
        }

        private void Allocate(int id)
        {
            _allocatedId[id] = true;
            _allocatedIdCount++;
        }
    }

}