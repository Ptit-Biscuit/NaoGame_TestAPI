using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour {

    public new Camera camera;

    public GameObject circle;

    [Range(1, 10)]
	public int minDepth = 5;

    [Range(5, 20)]
    public int maxDepth = 10;

    public int maxSiblings = 3;

    private Color[] colors = new Color[] {Color.blue, Color.red, Color.green};

    private List<TreeNode> treeNodes = new List<TreeNode>();

    void clear() {
        this.camera.orthographicSize = 5;

        if (treeNodes.Count > 0) {
            treeNodes.ForEach(node => Destroy(node.obj));
            treeNodes.Clear();
        }
    }

	public void generateTree() {
        clear();

        int depth = Random.Range(minDepth, maxDepth);

        if(this.camera.orthographicSize <= depth / 2) {
            this.camera.orthographicSize = (depth / 2) + 1;
        }
        
        TreeNode root = new TreeNode(-1, new List<TreeNode>(), new Vector3(-depth - 1, 0, 0), Color.white);
        treeNodes.Add(root);

		for(int i = 0; i <= depth; i++) {
            List<TreeNode> parents = treeNodes.FindAll(x => x.index == i - 1);

            int y = 0;

            for(int j = 1; j <= Random.Range(1, maxSiblings + 2); j++) {
                treeNodes.Add(new TreeNode(i, parents, new Vector3(parents[0].position.x + 2, y, 0), colors[Random.Range(0, 3)]));
                
                if(j % 2 != 0) {
                    y += 2 * j;
                }
                else {
                    y *= -1;
                }
            }
        }

        StartCoroutine(drawTree());
	}

    IEnumerator drawTree() {
        if (treeNodes.Count > 1) {
            foreach(TreeNode node in treeNodes) {
                node.Get(circle);
                yield return new WaitForSeconds(.1f);
            }
        }
    }
}
