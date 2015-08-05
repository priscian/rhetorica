// Based on classes from: Hilyard J and Teilhet S: C# 3.0 Cookbook. O'Reilly Media, Inc., 2007.

using System;
using System.Collections;
using System.Collections.Generic;

public class NTree<T> : IEnumerable<T>
  where T : IComparable<T>
{
  public NTree()
  {
    maxChildren = int.MaxValue;
  }

  public IEnumerator<T> GetEnumerator()
  {
    throw new NotImplementedException();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return this.GetEnumerator();
  }

  public NTree(int maxNumChildren)
  {
    maxChildren = maxNumChildren;
  }

  // The root node of the tree.
  private NTreeNodeFactory<T>.NTreeNode<T> root = null;
  // The maximum number of child nodes that a parent node may contain.
  private int maxChildren = 0;

  public NTreeNodeFactory<T>.NTreeNode<T> GetRoot()
  {
    return root;
  }

  public void AddRoot(NTreeNodeFactory<T>.NTreeNode<T> node)
  {
    root = node;
  }

  public int MaxChildren
  {
    get { return (maxChildren); }
  }
}

public class NTreeNodeFactory<T>
  where T : IComparable<T>
{
  public NTreeNodeFactory(NTree<T> root)
  {
    maxChildren = root.MaxChildren;
  }

  private int maxChildren = 0;

  public int MaxChildren
  {
    get { return (maxChildren); }
  }

  public NTreeNode<T> CreateNode(T value)
  {
    return (new NTreeNode<T>(value, maxChildren));
  }

  // Nested Node class.
  public class NTreeNode<U>
    where U : IComparable<U>
  {
    public NTreeNode(U value, int maxChildren)
    {
      if (value != null) {
        nodeValue = value;
      }
      childNodes = new NTreeNode<U>[maxChildren];
    }

    protected U nodeValue = default(U);
    protected NTreeNode<U>[] childNodes = null;

    public int CountChildren
    {
      get
      {
        int currCount = 0;
        for (int index = 0; index <= childNodes.GetUpperBound(0); index++) {
          if (childNodes[index] != null) {
            ++currCount;
            currCount += childNodes[index].CountChildren;
          }
        }
        return (currCount);
      }
    }

    public int CountImmediateChildren
    {
      get
      {
        int currCount = 0;
        for (int index = 0; index <= childNodes.GetUpperBound(0); index++) {
          if (childNodes[index] != null) {
            ++currCount;
          }
        }
        return (currCount);
      }
    }

    public NTreeNode<U>[] Children
    {
      get { return (childNodes); }
    }

    public NTreeNode<U> GetChild(int index)
    {
      return (childNodes[index]);
    }

    public U Value()
    {
      return (nodeValue);
    }

    public void AddNode(NTreeNode<U> node)
    {
      int numOfNonNullNodes = CountImmediateChildren;
      if (numOfNonNullNodes < childNodes.Length) {
        childNodes[numOfNonNullNodes] = node;
      }
      else {
        throw (new Exception("Cannot add more children to this node."));
      }
    }

    public NTreeNode<U> DepthFirstSearch(U targetObj)
    {
      NTreeNode<U> retObj = default(NTreeNode<U>);
      if (targetObj.CompareTo(nodeValue) == 0) {
        retObj = this;
      }
      else {
        for (int index = 0; index <= childNodes.GetUpperBound(0); index++) {
          if (childNodes[index] != null) {
            retObj = childNodes[index].DepthFirstSearch(targetObj);
            if (retObj != null) {
              break;
            }
          }
        }
      }

      return (retObj);
    }

    public NTreeNode<U> BreadthFirstSearch(U targetObj)
    {
      Queue<NTreeNode<U>> row = new Queue<NTreeNode<U>>();
      row.Enqueue(this);
      while (row.Count > 0) {
        // Get next node in queue.
        NTreeNode<U> currentNode = row.Dequeue();

        // Is this the node we are looking for?
        if (targetObj.CompareTo(currentNode.nodeValue) == 0) {
          return (currentNode);
        }

        for (int index = 0; index < currentNode.CountImmediateChildren; index++) {
          if (currentNode.Children[index] != null) {
            row.Enqueue(currentNode.Children[index]);
          }
        }
      }

      return (null);
    }

    public void PrintDepthFirst()
    {
      Console.WriteLine("this: " + nodeValue.ToString());
      for (int index = 0; index < childNodes.Length; index++) {
        if (childNodes[index] != null) {
          Console.WriteLine("\tchildNodes[" + index + "]: " +
          childNodes[index].nodeValue.ToString());
        }
        else {
          Console.WriteLine("\tchildNodes[" + index + "]: NULL");
        }
      }
      for (int index = 0; index < childNodes.Length; index++) {
        if (childNodes[index] != null) {
          childNodes[index].PrintDepthFirst();
        }
      }
    }

    public List<U> IterateDepthFirst()
    {
      List<U> tempList = new List<U>();
      for (int index = 0; index < childNodes.Length; index++) {
        if (childNodes[index] != null) {
          tempList.Add(childNodes[index].nodeValue);
        }
      }
      for (int index = 0; index < childNodes.Length; index++) {
        if (childNodes[index] != null) {
          tempList.AddRange(childNodes[index].IterateDepthFirst());
        }
      }

      return (tempList);
    }

    public void RemoveNode(int index)
    {
      // Remove node from array and compact the array.
      if (index < childNodes.GetLowerBound(0) || index > childNodes.GetUpperBound(0)) {
        throw (new ArgumentOutOfRangeException("index", index, "Array index out of bounds."));
      }
      else if (index < childNodes.GetUpperBound(0)) {
        Array.Copy(childNodes, index + 1, childNodes, index, childNodes.Length - index - 1);
      }
      childNodes.SetValue(null, childNodes.GetUpperBound(0));
    }
  }
}
