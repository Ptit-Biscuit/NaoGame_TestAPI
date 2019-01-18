using UnityEngine;
using System.Collections.Generic;
using Delaunay;
using Delaunay.Geo;

public class Map : MonoBehaviour
{
	[SerializeField]
	private int m_pointCount = 10;

	[SerializeField]
	private float m_mapWidth = 100;

	[SerializeField]
	private float m_mapHeight = 50;

	private List<LineSegment> m_delaunayTriangulation;
	private List<Vector2> m_points;

	void Awake()
	{
		Demo();
	}

	void Update()
	{
		if (Input.anyKeyDown) {
			Demo();
		}
	}

	private void Demo()
	{
		List<uint> colors = new List<uint>();
		m_points = new List<Vector2>();
			
		for (int i = 0; i < m_pointCount; i++) {
			colors.Add(0);
			m_points.Add(new Vector2 (
				UnityEngine.Random.Range(0, m_mapWidth),
				UnityEngine.Random.Range(0, m_mapHeight))
			);
		}
		Delaunay.Voronoi v = new Delaunay.Voronoi(m_points, colors, new Rect(0, 0, m_mapWidth, m_mapHeight));
		m_delaunayTriangulation = v.DelaunayTriangulation();
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		if (m_points != null) {
			for (int i = 0; i < m_points.Count; i++) {
				Gizmos.DrawSphere(m_points [i], 0.5f);
			}
		}

		Gizmos.color = Color.magenta;
		if (m_delaunayTriangulation != null) {
			for (int i = 0; i< m_delaunayTriangulation.Count; i++) {
				Vector2 left = (Vector2)m_delaunayTriangulation [i].p0;
				Vector2 right = (Vector2)m_delaunayTriangulation [i].p1;
				Gizmos.DrawLine((Vector3)left, (Vector3)right);
			}
		}

		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(new Vector2(0, 0), new Vector2(0, m_mapHeight));
		Gizmos.DrawLine(new Vector2(0, 0), new Vector2(m_mapWidth, 0));
		Gizmos.DrawLine(new Vector2(m_mapWidth, 0), new Vector2(m_mapWidth, m_mapHeight));
		Gizmos.DrawLine(new Vector2(0, m_mapHeight), new Vector2(m_mapWidth, m_mapHeight));
	}
}