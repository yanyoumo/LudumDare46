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

            if (assetPath.ToLower().Contains("_basecolor.tga"))
            {
                textureImporter.textureShape = TextureImporterShape.Texture2D;
                textureImporter.sRGBTexture = true;
                textureImporter.textureType = TextureImporterType.Default;
            }
            if (assetPath.ToLower().Contains("_maskmap.tga"))
            {
                textureImporter.textureShape = TextureImporterShape.Texture2D;
                textureImporter.sRGBTexture = false;
                textureImporter.textureType = TextureImporterType.Default;
            }
            if (assetPath.ToLower().Contains("_normal.tga"))
            {
                textureImporter.textureShape = TextureImporterShape.Texture2D;
                textureImporter.sRGBTexture = false;
                textureImporter.textureType = TextureImporterType.NormalMap;
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
