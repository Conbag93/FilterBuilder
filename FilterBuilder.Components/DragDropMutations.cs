namespace FilterBuilder.Components;

public static class DragDropMutations
{
    // Navigate to the group at path from root.
    // path=[] returns root itself.
    private static Condition Navigate(Condition root, int[] path)
    {
        var current = root;
        foreach (var idx in path)
            current = current.GroupChildren()[idx];
        return current;
    }

    // Replace the node at path with transform(node).
    private static Condition ReplaceAt(Condition root, int[] path, Func<Condition, Condition> transform)
    {
        if (path.Length == 0) return transform(root);

        var children = root.GroupChildren().ToList();
        children[path[0]] = ReplaceAt(children[path[0]], path[1..], transform);
        return root.WithGroupChildren(children);
    }

    // Adjust a path after removing an element at (removedGroupPath, removedIndex).
    // If path goes through removedGroupPath and then picks an index > removedIndex,
    // decrement that component.
    private static int[] AdjustPath(int[] path, int[] removedGroupPath, int removedIndex)
    {
        if (path.Length <= removedGroupPath.Length) return path;

        for (int i = 0; i < removedGroupPath.Length; i++)
            if (path[i] != removedGroupPath[i]) return path;

        var next = path[removedGroupPath.Length];
        if (next > removedIndex)
        {
            var copy = (int[])path.Clone();
            copy[removedGroupPath.Length] = next - 1;
            return copy;
        }
        return path;
    }

    // Returns true if dstGroupPath is inside the node being dragged (srcGroupPath + srcIndex).
    // Prevents dropping a group into one of its own descendants.
    public static bool IsDescendantDrop(int[] srcGroupPath, int srcIndex, int[] dstGroupPath)
    {
        var prefix = srcGroupPath.Concat(new[] { srcIndex }).ToArray();
        if (dstGroupPath.Length < prefix.Length) return false;
        return prefix.SequenceEqual(dstGroupPath.Take(prefix.Length));
    }

    // Move the node at (srcGroupPath, srcIndex) to just before or after
    // the node at (dstGroupPath, dstIndex).
    // Returns the original root unchanged if the move is a no-op or invalid.
    public static Condition MoveNode(
        Condition root,
        int[] srcGroupPath, int srcIndex,
        int[] dstGroupPath, int dstIndex,
        bool above)
    {
        bool sameGroup = srcGroupPath.SequenceEqual(dstGroupPath);

        // Same item, same position — no-op
        if (sameGroup && srcIndex == dstIndex) return root;

        // Can't drop into a descendant of the item being moved
        if (IsDescendantDrop(srcGroupPath, srcIndex, dstGroupPath)) return root;

        // The item to move
        var item = Navigate(root, srcGroupPath).GroupChildren()[srcIndex];

        // Raw insert position (before adjustment for same-group shifts)
        int insertIndex = above ? dstIndex : dstIndex + 1;

        // Same group: removing src shifts indices for items after it
        if (sameGroup && srcIndex < insertIndex)
            insertIndex--;

        // No-op after adjustment
        if (sameGroup && srcIndex == insertIndex) return root;

        // Step 1: remove from source group
        root = ReplaceAt(root, srcGroupPath, g =>
        {
            var ch = g.GroupChildren().ToList();
            ch.RemoveAt(srcIndex);
            return g.WithGroupChildren(ch);
        });

        // Step 2: adjust dstGroupPath if cross-group and removal shifted it
        var adjustedDstPath = sameGroup
            ? dstGroupPath
            : AdjustPath(dstGroupPath, srcGroupPath, srcIndex);

        // Step 3: insert at destination
        root = ReplaceAt(root, adjustedDstPath, g =>
        {
            var ch = g.GroupChildren().ToList();
            var clampedIdx = Math.Clamp(insertIndex, 0, ch.Count);
            ch.Insert(clampedIdx, item);
            return g.WithGroupChildren(ch);
        });

        return root;
    }
}
