using System.Collections.Generic;

namespace System.Utilities.LinkedEnumerator
{
    public class LinkedEnumerator<T> where T : class
    {
        private bool _atEnd;

        private bool _atStart;

        public LinkedEnumerator(IEnumerator<T> enumerator) {
            _atStart = true;
            _atEnd = true;
            enumerator.MoveNext();
            var initial = new Node<T>(enumerator.Current);
            First = initial;
            Last = initial;
            CurrentNode = null;

            while (enumerator.MoveNext()) Append(enumerator.Current);
        }

        public LinkedEnumerator(List<T> list) : this(list.GetEnumerator()) { }

        public T CurrentValue => CurrentNode?.Value;
        public Node<T> CurrentNode { get; private set; }
        public Node<T> First { get; private set; }
        public Node<T> Last { get; private set; }

        private static bool IsEmpty(Node<T> node) {
            return node == null || node.IsEmpty;
        }

        public bool HasNext() {
            return !IsEmpty(CurrentNode?.Next);
        }

        public bool HasPrevious() {
            return !IsEmpty(CurrentNode?.Previous);
        }

        public bool MoveNext() {
            if (_atStart) {
                CurrentNode = First;
                _atStart = false;
            }
            else if (HasNext()) {
                CurrentNode = CurrentNode.Next;
            }
            else {
                CurrentNode = null;
                _atEnd = true;
            }

            return !IsEmpty(CurrentNode);
        }


        public bool MovePrevious() {
            if (_atEnd) {
                CurrentNode = Last;
                _atEnd = false;
            }
            else if (HasPrevious()) {
                CurrentNode = CurrentNode.Previous;
            }
            else {
                CurrentNode = null;
                _atStart = true;
            }

            return !IsEmpty(CurrentNode);
        }

        public void Append(T value) {
            var node = new Node<T>(value);
            Append(node);
        }

        private void Append(Node<T> node) {
            var current = Last;
            current.Next = node;
            node.Previous = current;
            Last = node;
        }

        private void Remove(Node<T> node) {
            if (node.Previous != null) node.Previous.Next = node.Next;
            if (node.Next != null) node.Next.Previous = node.Previous;

            if (First == node) First = node.Next;
            if (Last == node) Last = node.Previous;

            if (CurrentNode == node) CurrentNode = node.Next;
            if (IsEmpty(CurrentNode)) _atEnd = true;
        }

        public void RemoveNext(Predicate<T> predicate) {
            var node = CurrentNode.Next;
            while (node != null) {
                if (predicate(node.Value)) Remove(node);
                node = node?.Next;
            }
        }

        public void InsertNext(T value) {
            var node = new Node<T>(value);
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
        }

        public List<T> ToList() {
            var list = new List<T>();
            var node = CurrentNode.Next;
            while (!IsEmpty(node)) {
                list.Add(node.Value);
                node = node.Next;
            }

            return list;
        }

        public class Node<TU>
        {
            public Node(TU value) {
                Value = value;
            }

            public TU Value { get; }
            public Node<TU> Previous { get; set; }
            public Node<TU> Next { get; set; }
            public bool IsEmpty => Value == null;
        }
    }
}