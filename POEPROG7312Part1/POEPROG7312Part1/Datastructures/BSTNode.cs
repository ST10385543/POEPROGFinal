namespace POEPROG7312Part1.Datastructures
{
    // Represents a single node in the Binary Search Tree (BST)
    // Each node stores a key (RequestID) and its corresponding status
    public class BSTNode
    {
        // The key of the node, representing a RequestID as string
        public string Key { get; set; }

        // The current status of the request (e.g., "Read", "In Progress", "Resolved")
        public string Status { get; set; }

        // Reference to the left child node (nodes with smaller keys)
        public BSTNode Left { get; set; }

        // Reference to the right child node (nodes with larger keys)
        public BSTNode Right { get; set; }

        // Constructor to initialize a new BST node with a key and status
        public BSTNode(string key, string status)
        {
            Key = key;       // Assign RequestID
            Status = status; // Assign initial status
            Left = null;     // Initialize left child as null
            Right = null;    // Initialize right child as null
        }
    }
}
// Kondratev, D., 2023. Unlocking the Potential of Binary Search Trees with C# Programming. DZone. Available at: https://dzone.com/articles/unlocking-the-potential-of-binary-search-trees-wit (Accessed: 12 Nov 2025).