using UnityEngine;
using UnityEditor;
 
class CreateMesh
{
	[MenuItem("Assets/Create/Mesh")]
	static void Create()
	{
		string filePath = AssetDatabase.GenerateUniqueAssetPath("Assets/New mesh.asset");
    	AssetDatabase.CreateAsset(new Mesh(), filePath);
	}
}