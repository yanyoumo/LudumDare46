using UnityEditor;
using UnityEngine;

namespace theArchitectTechPack
{
    class ArchAssetPostProcessor : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            //TODO 线性空间和法线控制那些东西。但是估计LD46用不上。
            TextureImporter textureImporter = assetImporter as TextureImporter;
            System.Diagnostics.Debug.Assert(textureImporter != null, nameof(textureImporter) + " != null");
            if (assetPath.ToLower().Contains(".hdr"))
            {
                textureImporter.textureShape = TextureImporterShape.TextureCube;
                textureImporter.sRGBTexture = true;
            }
        }

        void OnPreprocessModel()
        {
            Debug.Log("Something In");
            if (assetPath.ToLower().Contains(".fbx"))
            {
                ModelImporter modelImporter = assetImporter as ModelImporter;
                System.Diagnostics.Debug.Assert(modelImporter != null, nameof(modelImporter) + " != null");
                modelImporter.materialImportMode = ModelImporterMaterialImportMode.None;
                modelImporter.animationType = ModelImporterAnimationType.None;
                modelImporter.importAnimation = false;
                modelImporter.importConstraints = false;
            }
        }
    }
}
