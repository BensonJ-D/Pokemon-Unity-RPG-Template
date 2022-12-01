using System.Collections;
using System.Collections.Generic;

namespace System.Utilities.DoubleLinkedList
{
    public class LinkedEnumerator<T> where T : class
    {
        public class Node<T> {
            public T Value { get; }
            public Node<T> Previous { get; set; }
            public Node<T> Next { get; set; }
            
            public Node(T value) { Value = value; }
            public bool IsEmpty => Value == null;
        }
        
        private static bool IsEmpty(Node<T> node) => node == null || node.IsEmpty;
        
        public bool HasNext() => !IsEmpty(_currentNode?.Next);
        public bool HasPrevious() => !IsEmpty(_currentNode?.Previous);
        
        public bool MoveNext()
        {
            if (_atStart) {
                _currentNode = First;
                _atStart = false; 
            }
            else if (HasNext()) _currentNode = _currentNode.Next;
            else {
                _currentNode = null;
                _atEnd = true; 
            }
            return !IsEmpty(_currentNode);
        }


        public bool MovePrevious()
        {            
            if (_atEnd) {
                _currentNode = Last;
                _atEnd = false; 
            }
            else if (HasPrevious()) _currentNode = _currentNode.Previous;
            else {
                _currentNode = null;
                _atStart = true; 
            }
            return !IsEmpty(_currentNode);
        }

        public T Current => _currentNode?.Value;
        private Node<T> _currentNode;

        private bool _atStart;
        private bool _atEnd;
        private Node<T> First { get; set; }
        private Node<T> Last { get; set; }
        
        private void Append(T value)
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
        
        public LinkedEnumerator(IEnumerator<T> enumerator)
        {
            _atStart = true;
            _atEnd = true;
            enumerator.MoveNext();
            Node<T> initial = new Node<T>(enumerator.Current);
            First = initial;
            Last = initial;
            _currentNode = null;

            while (enumerator.MoveNext()) Append(enumerator.Current);
        }

        public LinkedEnumerator(List<T> list) : this(list.GetEnumerator()) { }
    }
}