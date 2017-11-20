using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace AssetBundles.Utility
{
    public class AssetBundleNameManager : EditorWindow
    {
        /// <summary>AssetBundleが設定されているAssetImporterリスト</summary>
        public static List<AssetImporter> ImporterList { get; set; }

        Vector2 scrollPos = Vector2.zero;
        static AssetBundleNameManager instance = null;


        [MenuItem("Window/Asset Bundle Name Manager")]
        static void Open()
        {
            if (ImporterList == null)
            {
                ImporterList = new List<AssetImporter>();
                Init();
            }
            instance = GetWindow<AssetBundleNameManager>();
        }

        /// <summary>GUIの更新</summary>
        public static void GUIUpdate()
        {
            if (instance == null) { return; }
            AssetDatabase.RemoveUnusedAssetBundleNames();
            instance.Repaint();
        }

        /// <summary>初期化</summary>
        static void Init()
        {
            /**
            * データ登録関数
            */
            System.Action<string[]> Register =
                (assetPaths) =>
            {
                foreach (var assetPath in assetPaths)
                {
                    var importer =
                        AssetImporter.GetAtPath(assetPath);
                    if (!string.IsNullOrEmpty(importer.assetBundleName))
                    {
                        ImporterList.Add(importer);
                    }
                }
            };

            /**
            * フォルダに設定されているAssetBundle名を取得
            */
            {
                var directories =
                    Directory.GetDirectories(
                        Application.dataPath,
                        "*",
                        searchOption: SearchOption.AllDirectories);
                for (int i = 0; i < directories.Length; ++i)
                {
                    directories[i] =
                        directories[i].Remove(
                            0, Application.dataPath.LastIndexOf("Assets"));
                }
                Register(directories);
            }

            /**
            * ファイルに設定されているAssetBundle名を取得
            */
            {
                var assetBundleNames =
                    AssetDatabase.GetAllAssetBundleNames();
                foreach (var abName in assetBundleNames)
                {
                    // 対応するAssetBundle名が設定されているAssetのPathリストを取得.
                    // ※下記の関数ではフォルダが取得できないので注意.
                    // →フォルダにラベルを設定している場合は、
                    // 　中に入っている適用範囲のAssetのPathが返ってくる.
                    var assetPaths =
                        AssetDatabase.GetAssetPathsFromAssetBundle(abName);
                    Register(assetPaths);
                }
            }
        }


        void OnGUI()
        {
            if (ImporterList == null) { return; }

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            /**
            * AssetBundleName
            */
            EditorGUILayout.LabelField("[AssetBundle]", EditorStyles.boldLabel);
            var assetBundleNames = ImporterList
                .Select(_ => _.assetBundleName)
                .Distinct()
                .ToList();
            foreach (var name in assetBundleNames)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                var field = EditorGUILayout.TextField(name);
                string preField = name;
                if (EditorGUI.EndChangeCheck())
                {
                    this.ChangeAssetBundleName(field, preField);
                    return;
                }
                if (GUILayout.Button("Remove"))
                {
                    // 空文字を渡して設定を消す
                    this.ChangeAssetBundleName(string.Empty, preField);
                }
                EditorGUILayout.EndHorizontal();
            }

            /**
            * Variant
            */
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("[Variant]", EditorStyles.boldLabel);
            var variants = ImporterList
                .Select(_ => _.assetBundleVariant)
                .Where(_ => !string.IsNullOrEmpty(_))
                .Distinct()
                .ToList();
            foreach (var variant in variants)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                var field = EditorGUILayout.TextField(variant);
                string preField = variant;
                if (EditorGUI.EndChangeCheck())
                {
                    this.ChangeVariant(field, preField);
                    return;
                }
                if (GUILayout.Button("Remove"))
                {
                    // 空文字を渡して設定を消す
                    this.ChangeVariant(string.Empty, preField);
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }


        /// <summary>AssetBundle名変更</summary>
        /// <param name="changedName">変更後のAssetBundleName</param>
        /// <param name="preName">変更前のAssetBundleName</param>
        void ChangeAssetBundleName(string changedName, string preName)
        {
            var list = ImporterList
                .Where(_ => _.assetBundleName == preName)
                .ToList();
            foreach (var importer in list)
            {
                importer.assetBundleName = changedName;
            }
            GUIUpdate();
        }


        /// <summary>Variant名変更</summary>
        /// <param name="changedVariant">変更後のVariant</param>
        /// <param name="preVariant">変更前のVariant</param>
        void ChangeVariant(string changedVariant, string preVariant)
        {
            var list = ImporterList
                .Where(_ => _.assetBundleVariant == preVariant)
                .ToList();
            foreach (var importer in list)
            {
                importer.assetBundleVariant = changedVariant;
            }
            GUIUpdate();
        }
    }
}