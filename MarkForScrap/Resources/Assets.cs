using System.Reflection;
using UnityEngine;

namespace MarkForScrap.Resources
{
    internal static class Assets
    {
        private static AssetBundle _mainAssetBundle;
        
        internal static T Load<T>(string asset) where T : Object
        {
            return _mainAssetBundle.LoadAsset<T>(asset);
        }

        internal static void LoadAssets()
        {
            using var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"MarkForScrap.Resources.AssetBundles.markforscrap");
            _mainAssetBundle = AssetBundle.LoadFromStream(assetStream);
        }
    }
}