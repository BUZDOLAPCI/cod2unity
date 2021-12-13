using UnityEngine;
using UnityEditor;

public class SetAllTexturesTransparent : AssetPostprocessor
{
    void OnPreprocessTexture() {

        if( assetPath.Contains( "images" ) ) {
            TextureImporter importer = assetImporter as TextureImporter;
            importer.textureType = TextureImporterType.Default;
            //importer.textureFormat = TextureImporterFormat.RGBA32;
            importer.textureCompression = TextureImporterCompression.Compressed;
            importer.alphaIsTransparency = true;
            importer.isReadable = true;
            importer.filterMode = FilterMode.Bilinear;
            importer.npotScale = TextureImporterNPOTScale.None;

            Object asset = AssetDatabase.LoadAssetAtPath( importer.assetPath, typeof( Texture2D ) );
            if( asset ) {
                EditorUtility.SetDirty( asset );
            } else {
                importer.textureType = TextureImporterType.Default;
            }
        }
    }
}