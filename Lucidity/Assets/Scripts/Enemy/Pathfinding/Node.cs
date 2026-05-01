using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Lucidity
{

    public class Node : IHeapItem<Node>
    {
        public bool walkable;
        public Vector3 worldPosition;
        public int gridX;
        public int gridY;

        public bool isVisible;

        public int gCost;
        public int hCost;

        public Node parent;
        int heapIndex;

        public Node(bool _walkable, Vector3 _worldPos, int gridX, int gridY)
        {
            walkable = _walkable;
            worldPosition = _worldPos;
            this.gridX = gridX;
            this.gridY = gridY;
        }

        public int fCost
        {
            get
            {
                return gCost + hCost;
            }
        }

        public int HeapIndex
        {
            get
            {
                return heapIndex;
            }
            set
            {
                heapIndex = value;
            }
        }

        public int CompareTo(Node nodeToCompare)
        {
            int compare = fCost.CompareTo(nodeToCompare.fCost);
            if (compare == 0)
            {
                compare = hCost.CompareTo(nodeToCompare.hCost);
            }
            return -compare;
        }

    }
}
