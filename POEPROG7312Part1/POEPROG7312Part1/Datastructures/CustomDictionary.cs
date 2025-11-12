using System;
using System.Collections;
using System.Collections.Generic;

namespace POEPROG7312Part1.Datastructures
{
    public class CustomDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private Node<TKey, TValue>[] buckets;
        private int size;

        public CustomDictionary(int capacity = 10)
        {
            buckets = new Node<TKey, TValue>[capacity];
            size = capacity;
        }

        private int GetBucketIndex(TKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return Math.Abs(key.GetHashCode() % size);
        }

        public void Add(TKey key, TValue value)
        {
            int index = GetBucketIndex(key);
            Node<TKey, TValue> newNode = new Node<TKey, TValue>(key, value);

            if (buckets[index] == null)
            {
                buckets[index] = newNode;
            }
            else
            {
                Node<TKey, TValue> current = buckets[index];
                while (current.Next != null)
                {
                    if (EqualityComparer<TKey>.Default.Equals(current.Key, key))
                        throw new Exception("Duplicate key not allowed");
                    current = current.Next;
                }

                if (EqualityComparer<TKey>.Default.Equals(current.Key, key))
                    throw new Exception("Duplicate key not allowed");

                current.Next = newNode;
            }
        }

        public TValue Get(TKey key)
        {
            int index = GetBucketIndex(key);
            Node<TKey, TValue> current = buckets[index];

            while (current != null)
            {
                if (EqualityComparer<TKey>.Default.Equals(current.Key, key))
                    return current.Value;
                current = current.Next;
            }

            throw new Exception("Key not found");
        }

        public bool ContainsKey(TKey key)
        {
            try
            {
                Get(key);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Remove(TKey key)
        {
            int bucketIndex = GetBucketIndex(key);
            Node<TKey, TValue> current = buckets[bucketIndex];
            Node<TKey, TValue> prev = null;

            while (current != null)
            {
                if (EqualityComparer<TKey>.Default.Equals(current.Key, key))
                {
                    if (prev == null)
                        buckets[bucketIndex] = current.Next;
                    else
                        prev.Next = current.Next;
                    return true;
                }
                prev = current;
                current = current.Next;
            }
            return false;
        }

        public List<TValue> ToList()
        {
            var list = new List<TValue>();
            foreach (var bucket in buckets)
            {
                Node<TKey, TValue> current = bucket;
                while (current != null)
                {
                    list.Add(current.Value);
                    current = current.Next;
                }
            }
            return list;
        }

       
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var bucket in buckets)
            {
                Node<TKey, TValue> current = bucket;
                while (current != null)
                {
                    yield return new KeyValuePair<TKey, TValue>(current.Key, current.Value);
                    current = current.Next;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerable<TKey> Keys()
        {
            foreach (var bucket in buckets)
            {
                Node<TKey, TValue> current = bucket;
                while (current != null)
                {
                    yield return current.Key;
                    current = current.Next;
                }
            }
        }

       
        public void AddOrAppend(TKey key, TValue value, Action<TValue, TValue> appendAction = null)
        {
            if (ContainsKey(key))
            {
                if (appendAction != null)
                    appendAction(Get(key), value);
                else
                    throw new Exception("Duplicate key not allowed");
            }
            else
            {
                Add(key, value);
            }
        }
    }
}

//Stack Overflow, 2015 (updated 2020+). Recreating a Dictionary from an IEnumerable<KeyValuePair<>>. [online] Available at: https://stackoverflow.com/questions/2636603/recreating-a-dictionary-from-an-ienumerablekeyvaluepair [Accessed 11 Oct. 2025].

//GeeksforGeeks, 2025. Dictionary in C#. [online] Available at: https://www.geeksforgeeks.org/c-sharp/dictionary-in-c-sharp [Accessed 11 Oct. 2025].