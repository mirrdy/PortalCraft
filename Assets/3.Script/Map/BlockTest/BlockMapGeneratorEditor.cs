using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BlockMapGenerator))]
public class BlockMapGeneratorEditor : Editor
{
    // 에디터에서 BlockMapGenerator 컴포넌트
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
