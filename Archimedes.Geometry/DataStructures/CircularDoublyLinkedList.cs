using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archimedes.Geometry.DataStructures
{
    /// <summary>
    /// Circular doubly linked lists of T, which is a one-element circular list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CircularDoublyLinkedList<T>
    {
        private readonly T _val;

        /// <summary>
        /// Creates a CircularDoublyLinkedList node 
        /// </summary>
        /// <param name="val"></param>
        public CircularDoublyLinkedList(T val)
        {
            this._val = val; Next = Prev = this;
        }

        public T Value
        {
            get { return _val; }
        }

        public CircularDoublyLinkedList<T> Prev { get; private set; }
        public CircularDoublyLinkedList<T> Next { get; private set; }

        /// <summary>
        /// Adjust the remaining elements, make this one point nowhere
        /// </summary>
        public void Delete()
        {
            Next.Prev = Prev; Prev.Next = Next;
            Next = Prev = null;
        }

        public CircularDoublyLinkedList<T> Prepend(CircularDoublyLinkedList<T> elt)
        {
            elt.Next = this; elt.Prev = Prev; Prev.Next = elt; Prev = elt;
            return elt;
        }

        public CircularDoublyLinkedList<T> Append(CircularDoublyLinkedList<T> elt)
        {
            elt.Prev = this; elt.Next = Next; Next.Prev = elt; Next = elt;
            return elt;
        }

        public int Size()
        {
            int count = 0;
            var node = this;
            do
            {
                count++;
                node = node.Next;
            } while (node != this);
            return count;
        }

        public void CopyInto(T[] vals, int i)
        {
            var node = this;
            do
            {
                vals[i++] = node._val;	// still, implicit checkcasts at runtime 
                node = node.Next;
            } while (node != this);
        }
    }
}
