using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeNode {

    public GameObject obj;
    
    public int index;
    
    public List<TreeNode> parents;
    
    public Vector3 position;
    
    Color color;

    public TreeNode(int index, List<TreeNode> parents, Vector3 position, Color color) {
        this.index = index;
        this.parents = parents;
        this.position = position;
        this.color = color;
    }

    public GameObject Get(GameObject prefab) {
        this.obj = GameObject.Instantiate(prefab, this.position, Quaternion.identity);
        this.obj.GetComponent<SpriteRenderer>().color = this.color;
        this.obj.SetActive(true);

        
        this.obj.AddComponent<LineRenderer>();
        LineRenderer lr = this.obj.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.startColor = Color.white;
        lr.startWidth = 0.1f;
        lr.positionCount = parents.Count * 2;

        for(int i = 0, j = 0; j < parents.Count; i += 2, j++) {
            lr.SetPosition(i, this.position);
            lr.SetPosition(i + 1, parents[j].position);
        }
        

        return this.obj;
    }
}