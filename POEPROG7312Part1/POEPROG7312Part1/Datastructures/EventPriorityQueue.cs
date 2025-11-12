using POEPROG7312Part1.Models;
using System;
using System.Collections.Generic;

namespace POEPROG7312Part1.Datastructures
{
    public class EventPriorityQueue
    {
        private readonly List<Event> heap = new();

        private int Parent(int i) => (i - 1) / 2;
        private int Left(int i) => 2 * i + 1;
        private int Right(int i) => 2 * i + 2;

        private void Swap(int i, int j)
        {
            (heap[i], heap[j]) = (heap[j], heap[i]);
        }

        public void Enqueue(Event e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));

            heap.Add(e);
            int i = heap.Count - 1;

            while (i > 0 && heap[i].Priority < heap[Parent(i)].Priority)
            {
                Swap(i, Parent(i));
                i = Parent(i);
            }
        }

        public Event Dequeue()
        {
            if (heap.Count == 0)
                return null;

            var root = heap[0];
            heap[0] = heap[^1];
            heap.RemoveAt(heap.Count - 1);

            Heapify(0);
            return root;
        }

        private void Heapify(int i)
        {
            int left = Left(i);
            int right = Right(i);
            int smallest = i;

            if (left < heap.Count && heap[left].Priority < heap[smallest].Priority)
                smallest = left;

            if (right < heap.Count && heap[right].Priority < heap[smallest].Priority)
                smallest = right;

            if (smallest != i)
            {
                Swap(i, smallest);
                Heapify(smallest);
            }
        }

        public List<Event> GetAll()
        {
            var sorted = new List<Event>(heap);
            sorted.Sort((a, b) => a.Priority.CompareTo(b.Priority)); // earliest first
            return sorted;
        }

        public int Count => heap.Count;

        public Event Peek() => heap.Count > 0 ? heap[0] : null;

        public void Clear() => heap.Clear();
    }
}

//Microsoft Learn, 2021. PriorityQueue Class (System.Collections.Generic). [online] Available at: https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.priorityqueue-2 [Accessed 11 Oct. 2025].
