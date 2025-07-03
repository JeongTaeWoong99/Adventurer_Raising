using System;
using System.Xml;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class ExportNavMeshToXML
{
	# if UNITY_EDITOR
	[MenuItem("Tools/Export NavMesh to XML")]
	static void ExportNavMeshData()
	{
		NavMeshTriangulation navMesh = NavMesh.CalculateTriangulation();

		XmlDocument xmlDoc = new XmlDocument();
		XmlElement root = xmlDoc.CreateElement("NavMesh");
		xmlDoc.AppendChild(root);

		// 정점 저장
		XmlElement verts = xmlDoc.CreateElement("Vertices");
		root.AppendChild(verts);
		foreach (Vector3 v in navMesh.vertices)
		{
			XmlElement vert = xmlDoc.CreateElement("Vertex");
			vert.SetAttribute("x", v.x.ToString());
			vert.SetAttribute("y", v.y.ToString());
			vert.SetAttribute("z", v.z.ToString());
			verts.AppendChild(vert);
		}

		// 삼각형 저장
		XmlElement tris = xmlDoc.CreateElement("Triangles");
		root.AppendChild(tris);
		for (int i = 0; i < navMesh.indices.Length; i += 3)
		{
			XmlElement tri = xmlDoc.CreateElement("Triangle");
			tri.SetAttribute("v0", navMesh.indices[i].ToString());
			tri.SetAttribute("v1", navMesh.indices[i + 1].ToString());
			tri.SetAttribute("v2", navMesh.indices[i + 2].ToString());
			tris.AppendChild(tri);
		}

		// 바탕화면 경로에 저장
		string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
		string path = Path.Combine(desktop, "navmesh_data.xml");

		try
		{
			xmlDoc.Save(path);
			Debug.Log("NavMesh XML 저장 완료: " + path);
		}
		catch (Exception e)
		{
			Debug.LogError("저장 실패: " + e.Message);
		}

		AssetDatabase.Refresh();
	}
	#endif
}