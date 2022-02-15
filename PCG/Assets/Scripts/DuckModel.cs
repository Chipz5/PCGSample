using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckModel : MonoBehaviour
{
    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public CapsuleCollider capsuleCollider;
    public Material mat;

    public DuckModel(Renderer textureRender, MeshFilter meshFilter, MeshRenderer meshRenderer, CapsuleCollider capsuleCollider, Material mat)
    {
        this.textureRender = textureRender;
        this.meshFilter = meshFilter;
        this.meshRenderer = meshRenderer;
        this.capsuleCollider = capsuleCollider;
        this.mat = mat;
    }
}


public class Duck
{
    readonly DuckModel model;
    Vector3 position;
    Vector3 scale;

    public Duck(DuckModel model, Vector3 position, Vector3 scale)
    {
        this.model = model;
        this.position = position;
        this.scale = scale;
    }
}