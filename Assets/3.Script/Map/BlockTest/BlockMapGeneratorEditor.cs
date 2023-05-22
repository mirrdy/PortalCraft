using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BlockMapGenerator))]
public class BlockMapGeneratorEditor : Editor
{
    // �����Ϳ��� BlockMapGenerator ������Ʈ
    public override void OnInspectorGUI()
    {
        BlockMapGenerator mapGen = (BlockMapGenerator)target;

        if (DrawDefaultInspector())
        {
            if (mapGen.isTest)
            {
                mapGen.GenerateMap();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            mapGen.GenerateMap();
        }

    }
}
