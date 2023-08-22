using System.Collections.Generic;
using System.Linq;

namespace System.Utilities.LinkedEnumerator
{
    public class LinkedEnumerator<T> where T : class
    {
        public class Node<TU> {
            public TU Value { get; }
            public Node<TU> Previous { get; set; }
            public Node<TU> Next { get; set; }
            
            public Node(TU value) { Value = value; }
            public bool IsEmpty => Value == null;
        }
        
        private static bool IsEmpty(Node<T> node) => node == null || node.IsEmpty;
        
        public bool HasNext() => !IsEmpty(CurrentNode?.Next);
        public bool HasPrevious() => !IsEmpty(CurrentNode?.Previous);
        
        public bool MoveNext()
        {
            if (_atStart) {
                CurrentNode = First;
                _atStart = false; 
            }
            else if (HasNext()) CurrentNode = CurrentNode.Next;
            else {
                CurrentNode = null;
                _atEnd = true; 
            }
            return !IsEmpty(CurrentNode);
        }


        public bool MovePrevious()
        {            
            if (_atEnd) {
                CurrentNode = Last;
                _atEnd = false; 
            }
            else if (HasPrevious()) CurrentNode = CurrentNode.Previous;
            else {
                CurrentNode = null;
                _atStart = true; 
            }
            return !IsEmpty(CurrentNode);
        }

        public T CurrentValue => CurrentNode?.Value;
        public Node<T> CurrentNode { get; private set; }

        private bool _atStart;
        private bool _atEnd;
        public Node<T> First { get; private set; }
        public Node<T> Last { get; private set; }
        
        public void Append(T value)
        {
            Node<T> node = new Node<T>(value);
            Append(node);
        }
        
        private void Append(Node<T> node)
        {
            var current = Last;
            current.Next = node;
            node.Previous = current;
            Last = node;
            
        }

        private void Remove(Node<T> node) {
            if(node.Previous != null) node.Previous.Next = node.Next;
            if(node.Next != null) node.Next.Previous = node.Previous;

            if (First == node) First = node.Next;
            if (Last == node) Last = node.Previous;

            if(CurrentNode == node) CurrentNode = node.Next;
            if(IsEmpty(CurrentNode)) _atEnd = true;
        }

        public void RemoveNext(Predicate<T> predicate) {
            var node = CurrentNode.Next;
            while (node != null) {
                if(predicate(node.Value)) Remove(node);
                node = node?.Next;
            }
        }

        public void InsertNext(T value) {
            Node<T> node = new Node<T>(value);
            node.Next = CurrentNode?.Next;
            node.Previous = CurrentNode;

            if (IsEmpty(CurrentNode)) {
                CurrentNode = node;
                Last = node;
                _atEnd = false;
                return;
            }

            if (CurrentNode == Last) Last = node;
            CurrentNode.Next = node;

            return;
        }
        
        public LinkedEnumerator(IEnumerator<T> enumerator) {
            _atStart = true;
            _atEnd = true;
            enumerator.MoveNext();
            Node<T> initial = new Node<T>(enumerator.Current);
            First = initial;
            Last = initial;
            CurrentNode = null;

            while (enumerator.MoveNext()) Append(enumerator.Current);
        }

        public LinkedEnumerator(List<T> list) : this(list.GetEnumerator()) { }

        public List<T> ToList() {
            var list = new List<T>();
            var node = CurrentNode.Next;
            while (!IsEmpty(node)) {
                list.Add(node.Value);
                node = node.Next;
            }

            return list;
        }
    }
}