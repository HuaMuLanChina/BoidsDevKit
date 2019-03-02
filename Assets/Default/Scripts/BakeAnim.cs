using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class BakeAnim : EditorWindow
{
    private Mesh ViewMesh;
    List<Vector4> uv = new List<Vector4>();
    List<Vector4> uv1 = new List<Vector4>();
    List<Vector4> uv2 = new List<Vector4>();
    List<Vector4> uv3 = new List<Vector4>();

    [MenuItem("Window/VertexAnimBaker/BakeAnim")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        BakeAnim editorWindow = EditorWindow.GetWindow<BakeAnim>();
        editorWindow.Show();

    }

    Vector2 scrolpos;
    bool vertTog = false;
    bool uvTog = false;
    bool uv1Tog = false;
    bool uv2Tog = false;
    bool uv3Tog = false;
    private void OnGUI()
    {
        ViewMesh = (Mesh)EditorGUILayout.ObjectField("SkinMesh", ViewMesh, typeof(Mesh),false);
        if(ViewMesh != null)
        { 
            scrolpos = EditorGUILayout.BeginScrollView(scrolpos);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            vertTog = EditorGUILayout.Toggle(vertTog);
            if (vertTog)
            {
                foreach(var pos in ViewMesh.vertices)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Box(pos.x.ToString());
                    GUILayout.Box(pos.y.ToString());
                    GUILayout.Box(pos.z.ToString());
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            uvTog = EditorGUILayout.Toggle("uvTog", uvTog);
            if (uvTog)
            {
                ViewMesh.GetUVs(0, uv);
                foreach (var pos in uv)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Box(pos.x.ToString());
                    GUILayout.Box(pos.y.ToString());
                    GUILayout.Box(pos.z.ToString());
                    GUILayout.Box(pos.w.ToString());
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            uv1Tog = EditorGUILayout.Toggle("uv1Tog", uv1Tog);
            if (uv1Tog)
            {
                ViewMesh.GetUVs(1, uv1);
                foreach (var pos in uv1)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Box(pos.x.ToString());
                    GUILayout.Box(pos.y.ToString());
                    GUILayout.Box(pos.z.ToString());
                    GUILayout.Box(pos.w.ToString());
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            uv2Tog = EditorGUILayout.Toggle("uv2Tog", uv2Tog);
            if (uv2Tog)
            {
                ViewMesh.GetUVs(2, uv2);
                foreach (var pos in uv2)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Box(pos.x.ToString());
                    GUILayout.Box(pos.y.ToString());
                    GUILayout.Box(pos.z.ToString());
                    GUILayout.Box(pos.w.ToString());
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            uv3Tog = EditorGUILayout.Toggle("uv3Tog", uv3Tog);
            if (uv3Tog)
            {
                ViewMesh.GetUVs(3, uv3);
                foreach (var pos in uv3)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Box(pos.x.ToString());
                    GUILayout.Box(pos.y.ToString());
                    GUILayout.Box(pos.z.ToString());
                    GUILayout.Box(pos.w.ToString());
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }
    }
}
