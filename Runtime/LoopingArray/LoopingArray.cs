using System;

namespace VED.Utilities
{
    // a looping fixed size array, in which you can continuously push new values to the end of the array, and once the limit is reached the latest entries will overwrite the earliest entries
    // tracking count + offset allows indices to shift with offset as entries are overwritten
    // i.e. LoopingArray[0] will always be earliest entry, LoopingArray[^1] will always be latest entry
    public struct LoopingArray<T>
    {
        private T[] _array;

        public int Length => _length;
        private int _length;

        public int Count => _count;
        private int _count;

        private int _offset;

        public LoopingArray(int length)
        {
            _length = length;
            _count  = 0;
            _offset = 0;

            _array = new T[_length];
        }

        private int GetIndex(int i)
        {
            int index = _offset + i;

            // if index exceeds length, loop around to start
            if (index >= _length)
                index -= _length;

            return index;
        }

        public T this[Index index]
        {
            get
            {
                if (index.IsFromEnd)
                    return _array[GetIndex(_count - index.Value)];

                return _array[GetIndex(index.Value)];
            }
        }

        public void Push(T value)
        {
            _array[GetIndex(_count)] = value;

            // if less entries than length, add to entries
            if (_count < (_length - 1))
            {
                _count++;
                return;
            }

            // if at max entries, overwrite earliest entries
            if (_offset < (_length - 1))
            {
                _offset++;
                return;
            }

            // if overwritten all entries, rewrite earliest entries again
            _offset = 0;
        }

        public void Clear()
        {
            _array  = new T[_length];
            _count  = 0;
            _offset = 0;
        }
    }
}