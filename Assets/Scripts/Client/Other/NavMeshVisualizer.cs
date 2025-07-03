using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using System.IO;

public class NavMeshVisualizer : MonoBehaviour
{
	void Start()
	{
		string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
		string filePath = Path.Combine(desktopPath, "navmesh_data.xml");

		CreateMeshFromXML(filePath);
	}

	void CreateMeshFromXML(string path)
	{
		if (!File.Exists(path))
		{
			Debug.LogError("XML 파일이 없습니다: " + path);
			return;
		}

		XmlDocument doc = new XmlDocument();
		doc.Load(path);

		List<Vector3> vertices = new List<Vector3>();
		XmlNodeList vertexNodes = doc.SelectNodes("//Vertices/Vertex");
		foreach (XmlNode node in vertexNodes)
		{
			float x = float.Parse(node.Attributes["x"].Value);
			float y = float.Parse(node.Attributes["y"].Value);
			float z = float.Parse(node.Attributes["z"].Value);
			vertices.Add(new Vector3(x, y, z));
		}

		List<int> triangles = new List<int>();
		XmlNodeList triangleNodes = doc.SelectNodes("//Triangles/Triangle");
		foreach (XmlNode node in triangleNodes)
		{
			int v0 = int.Parse(node.Attributes["v0"].Value);
			int v1 = int.Parse(node.Attributes["v1"].Value);
			int v2 = int.Parse(node.Attributes["v2"].Value);
			triangles.Add(v0);
			triangles.Add(v1);
			triangles.Add(v2);
		}

		// 메쉬 생성
		Mesh mesh = new Mesh();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.RecalculateNormals();

		GameObject go = new GameObject("NavMesh_Visualized");
		MeshFilter mf = go.AddComponent<MeshFilter>();
		MeshRenderer mr = go.AddComponent<MeshRenderer>();

		mf.mesh = mesh;
		mr.material = new Material(Shader.Find("Standard")) { color = Color.cyan };
	}
}