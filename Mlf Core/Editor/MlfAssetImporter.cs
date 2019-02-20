using System.IO;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;
using UnityEditor;
using System;

namespace Ardenfall.Mlf
{
    [ScriptedImporter(1, "mlf")]
    public class MlfAssetImporter : ScriptedImporter
    {

        public override void OnImportAsset(AssetImportContext ctx)
        {
            MlfAsset mainObject = AssetDatabase.LoadAssetAtPath<MlfAsset>(ctx.assetPath);

            if (mainObject == null)
                mainObject = ScriptableObject.CreateInstance<MlfAsset>();

            //Modify ScriptableAsset here
            mainObject.mlfObject = new MlfObject(File.ReadAllText(ctx.assetPath), ctx.assetPath);

            ctx.AddObjectToAsset("main obj", mainObject);
            ctx.SetMainObject(mainObject);

        }
        
    }
}
