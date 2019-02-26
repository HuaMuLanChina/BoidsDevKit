using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

[CustomEditor(typeof(EditAnimator))]
public class EditAnimatorInspector : Editor
{
    /// <summary>
    /// 滑动杆的当前时间
    /// </summary>
    private float m_CurTime;

    /// <summary>
        /// 是否已经烘培过
        /// </summary>
    private bool m_HasBake;

    /// <summary>
        /// 当前是否是预览播放状态
        /// </summary>
    private bool m_Playing;

    /// <summary>
    /// 当前运行时间
    /// </summary>
    private float m_RunningTime;

    /// <summary>
        /// 上一次系统时间
        /// </summary>
    private double m_PreviousTime;

    /// <summary>
        /// 总的记录时间
        /// </summary>
    private float m_RecorderStopTime;

    /// <summary>
    /// 滑动杆总长度
    /// </summary>
    private const float kDuration = 30f;

    private Animator m_Animator;

    private EditAnimator editAnimator { get { return target as EditAnimator; } }

    private Animator animator
    {
        get { return m_Animator ?? (m_Animator = editAnimator.GetComponent<Animator>()); }
    }

    private List<Vector3> verts = new List<Vector3>();

    private int textureChannel = 0;

    void OnEnable()
    {
        m_PreviousTime = EditorApplication.timeSinceStartup;
        EditorApplication.update += inspectorUpdate;
    }

    void OnDisable()
    {
        EditorApplication.update -= inspectorUpdate;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Bake"))
        {
            m_HasBake = false;
            bake();
        }
        if (GUILayout.Button("Play"))
        {
            play();
        }
        if (GUILayout.Button("Stop"))
        {
            stop();
        }
        EditorGUILayout.EndHorizontal();
        m_CurTime = EditorGUILayout.Slider("Time:", m_CurTime, 0f, kDuration);
        textureChannel = EditorGUILayout.IntField("TextChannel:", textureChannel);
        if(GUILayout.Button("Bake Into TextChannel"))
        {
            bakeIntoTextureCoord();
        }
        if(GUILayout.Button("Bake Into VertexColor"))
        {
            bakeIntoVC();
        }
        if (GUILayout.Button("Save to asset"))
        {
            export();
        }
        manualUpdate();
    }

    /// <summary>
    /// 烘培记录动画数据
    /// </summary>
    private void bake()
    {
        if (m_HasBake)
        {
            return;
        }

        if (Application.isPlaying || animator == null)
        {
            return;
        }

        const float frameRate = 30f;
        const int frameCount = (int)((kDuration * frameRate) + 2);
        animator.Rebind();
        animator.StopPlayback();
        animator.recorderStartTime = 0;

        // 开始记录指定的帧数
        animator.StartRecording(frameCount);

        for (var i = 0; i < frameCount - 1; i++)
        {
            // 这里可以在指定的时间触发新的动画状态
            //if (i == 200)
            //{
            //    animator.SetTrigger("Move");
            //}

            // 记录每一帧
            animator.Update(1.0f / frameRate);
        }
        // 完成记录
        animator.StopRecording();

        // 开启回放模式
        animator.StartPlayback();
        m_HasBake = true;
        m_RecorderStopTime = animator.recorderStopTime;
    }

    /// <summary>
    /// 进行预览播放
    /// </summary>
    private void play()
    {
        if (Application.isPlaying || animator == null)
        {
            return;
        }

        bake();
        m_RunningTime = 0f;
        m_Playing = true;
    }

    /// <summary>
    /// 停止预览播放
    /// </summary>
    private void stop()
    {
        if (Application.isPlaying || animator == null)
        {
            return;
        }

        m_Playing = false;
        m_CurTime = 0f;
    }

    /// <summary>
    /// 预览播放状态下的更新
    /// </summary>
    private void update()
    {
        if (Application.isPlaying || animator == null)
        {
            return;
        }

        if (m_RunningTime > m_RecorderStopTime)
        {
            m_Playing = false;
            return;
        }

        // 设置回放的时间位置
        animator.playbackTime = m_RunningTime;
        animator.Update(0);
        m_CurTime = m_RunningTime;
    }

    /// <summary>
    /// 非预览播放状态下，通过滑杆来播放当前动画帧
    /// </summary>
    private void manualUpdate()
    {
        if (animator && !m_Playing && m_HasBake && m_CurTime < m_RecorderStopTime)
        {
            animator.playbackTime = m_CurTime;
            animator.Update(0);
        }
    }

    private void inspectorUpdate()
    {
        var delta = EditorApplication.timeSinceStartup - m_PreviousTime;
        m_PreviousTime = EditorApplication.timeSinceStartup;

        if (!Application.isPlaying && m_Playing)
        {
            m_RunningTime = Mathf.Clamp(m_RunningTime + (float)delta, 0f, kDuration);
            update();
        }
    }

    private void bakeIntoTextureCoord()
    {
        Mesh mesh = new Mesh();
        var skin = editAnimator.GetComponentInChildren<SkinnedMeshRenderer>();
        skin.BakeMesh(mesh);
        mesh.GetVertices(verts);
        var targetMesh = skin.sharedMesh;
        targetMesh.SetUVs(textureChannel, verts);
    }

    private void bakeIntoVC()
    {
        Mesh mesh = new Mesh();
        var skin = editAnimator.GetComponentInChildren<SkinnedMeshRenderer>();
        skin.BakeMesh(mesh);
        mesh.GetVertices(verts);
        List<Color> vc = new List<Color>();
        foreach(var v in verts)
        {
            vc.Add(new Color(v.x,v.y,v.z));
        }
        var targetMesh = skin.sharedMesh;
        targetMesh.SetColors(vc);
    }

    private void export()
    {
        var skin = editAnimator.GetComponentInChildren<SkinnedMeshRenderer>();
        string path = AssetDatabase.GetAssetPath(skin.sharedMesh);
        Mesh oldmesh = skin.sharedMesh;
        Mesh newmesh = new Mesh();
        newmesh.vertices = oldmesh.vertices;
        newmesh.triangles = oldmesh.triangles;
        newmesh.uv = oldmesh.uv;
        List<Vector3> uv = new List<Vector3>();
        oldmesh.GetUVs(1, uv);
        newmesh.SetUVs(1, uv);
        uv.Clear();
        oldmesh.GetUVs(2, uv);
        newmesh.SetUVs(2, uv);
        uv.Clear();
        oldmesh.GetUVs(3, uv);
        newmesh.SetUVs(3, uv);

        newmesh.normals = oldmesh.normals;
        newmesh.colors = oldmesh.colors;
        newmesh.tangents = oldmesh.tangents;
        string dir = Path.GetDirectoryName(path);
        string name = Path.GetFileNameWithoutExtension(path);
        string filename = name + ".asset";
        string filepath = Path.Combine(dir, filename);
        AssetDatabase.CreateAsset(newmesh, filepath);
        AssetDatabase.Refresh();
    }
}
