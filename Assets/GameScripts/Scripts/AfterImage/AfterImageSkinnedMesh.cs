using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

public class AfterImageSkinnedMesh : AfterImageBase
{
    [SerializeField] private GameObject _meshParent = null;
    [SerializeField] private GameObject _boneParent = null;
    [SerializeField, ReadOnly] private List<SkinnedMeshRenderer> _skinnedMeshRenderers = new List<SkinnedMeshRenderer>();
    [SerializeField, ReadOnly] private List<Transform> _bones = new List<Transform>();

    [ContextMenu("SetRenderers")]
    private void SetRenderers()
    {
        _skinnedMeshRenderers.Clear();
        foreach(var renderer in _meshParent.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            _skinnedMeshRenderers.Add(renderer);
        }
    }

    [ContextMenu("SetBones")]
    private void SetBones()
    {
        _bones.Clear();
        foreach (var bone in _boneParent.GetComponentsInChildren<Transform>())
        {
            _bones.Add(bone);
        }
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    public override void Initialize()
    {
        if (IsInitialized)
        {
            return;
        }
        base.Initialize();

        if (_skinnedMeshRenderers == null || _skinnedMeshRenderers.Count <= 0)
        {
            throw new Exception("Skinned Mesh Renderer Empty");
        }

        // メッシュとボーンの参照を取得しておく
        if (_skinnedMeshRenderers.Count <= 0)
        {
            SetRenderers();
        }
        if (_bones.Count <= 0)
        {
            SetBones();
        }

        // マテリアルを設定して初期化
        foreach (var mesh in _skinnedMeshRenderers)
        {
            mesh.sharedMaterial = _material;
        }

        IsInitialized = true;
    }

    /// <summary>
    /// 残像の開始時の設定処理
    /// </summary>
    /// <param name="param">残像のパラメーター</param>
    public override void Setup(IAfterImageSetupParam param)
    {
        if (param is AfterImageSkinnedMeshParam useBonesParam)
        {
            Transform temp = null;
            for(int i = 0; i < _bones.Count; i++)
            {
                temp = useBonesParam.Transforms.Find(b => b.name == _bones[i].name);
                if (temp != null)
                {
                    _bones[i].position = temp.position;
                    _bones[i].rotation = temp.rotation;
                    _bones[i].localScale = temp.localScale;
                }
            }
        }
        else
        {
            Debug.LogError("引数がAfterImageSkinnedMeshParamにキャスト出来ませんでした");
        }
        rate = 0f;
    }
}
