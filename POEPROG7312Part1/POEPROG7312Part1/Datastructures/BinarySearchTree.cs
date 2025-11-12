using System;
using System.Collections.Generic;

namespace POEPROG7312Part1.Datastructures
{
    // Represents a simple Binary Search Tree (BST) where each node stores a RequestID and its Status
    public class BinarySearchTree
    {
        // Root node of the BST
        public BSTNode Root { get; private set; }

        // Public method to insert a new node into the BST
        // key = RequestID as string, status = current status of the request
        public void Insert(string key, string status)
        {
            Root = InsertRecursive(Root, key, status);
        }

        // Recursive helper method to insert a node
        private BSTNode InsertRecursive(BSTNode root, string key, string status)
        {
            if (root == null)
                return new BSTNode(key, status); // Create a new node if position is empty

            // Compare keys to decide left or right subtree
            if (string.Compare(key, root.Key) < 0)
                root.Left = InsertRecursive(root.Left, key, status);  // Go left if smaller
            else if (string.Compare(key, root.Key) > 0)
                root.Right = InsertRecursive(root.Right, key, status); // Go right if larger

            return root;
        }

        // Public method to search for a node by key
        // Returns the status if found, "Not Found" otherwise
        public string Search(string key)
        {
            var node = SearchRecursive(Root, key);
            return node != null ? node.Status : "Not Found";
        }

        // Recursive search helper
        private BSTNode SearchRecursive(BSTNode root, string key)
        {
            if (root == null || root.Key == key)
                return root; // Return node if found or null if not

            // Traverse left or right based on key comparison
            if (string.Compare(key, root.Key) < 0)
                return SearchRecursive(root.Left, key);
            else
                return SearchRecursive(root.Right, key);
        }

        // Optional: Print nodes in-order for debugging or console output
        public void InOrderTraversal(BSTNode root)
        {
            if (root != null)
            {
                InOrderTraversal(root.Left); // Visit left subtree
                Console.WriteLine($"Request: {root.Key}, Status: {root.Status}"); // Process current node
                InOrderTraversal(root.Right); // Visit right subtree
            }
        }

        // Helper method to collect all keys in sorted order (in-order traversal)
        private void InOrderKeys(BSTNode root, List<string> keys)
        {
            if (root != null)
            {
                InOrderKeys(root.Left, keys);  // Left subtree
                keys.Add(root.Key);             // Current node
                InOrderKeys(root.Right, keys); // Right subtree
            }
        }

        // Returns a list of all RequestIDs sorted in ascending order
        public List<string> GetSortedKeys()
        {
            var keys = new List<string>();
            InOrderKeys(Root, keys); // Fill keys list using in-order traversal
            return keys;
        }
    }
}

//Chugh, A., 2022. Binary Search Tree (BST) – Search, Insert and Remove. DigitalOcean Community Tutorials, 4 August. Available at: https://www.digitalocean.com/community/tutorials/binary-search-tree-bst-search-insert-remove (Accessed: 12 November 2025).

//Lorenzen, A., Leijen, D., Swierstra, W. & Lindley, S., 2024. The Functional Essence of Imperative Binary Search Trees. Microsoft Research Technical Report MSR-TR-2023-28, 27 December. Available at: https://homepages.inf.ed.ac.uk/slindley/papers/fiptree.pdf (Accessed: 12 November 2025). homepages.inf.ed.ac.uk 