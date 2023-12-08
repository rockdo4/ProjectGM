using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RemoveSelectGameObjectCollider : MonoBehaviour
{
    [MenuItem("Tools/Remove Collider And Apply Prefab Changes %#p")]
    static public void removeColliderAndApplyPrefabChanges()
    {
        string log = "";
        var obj = Selection.gameObjects;
        var count = obj.Length;
        var successCount = 0;
        if (obj != null)
        {
            for (int i = 0; i < obj.Length; i++)
            {
                bool modified = false;
                var prefab_root = PrefabUtility.GetOutermostPrefabInstanceRoot(obj[i]);
                if (prefab_root == null)
                {
                    continue;
                }
                var parentObject = PrefabUtility.GetCorrespondingObjectFromOriginalSource(prefab_root);
                var prefab_src = AssetDatabase.GetAssetPath(parentObject);
                if (prefab_src != null && prefab_src != string.Empty)
                {
                    log += "<color=white>CHECKING PREFAB:</color> " + obj[i].name + "\n";
                    // now check to see if has a collider
                    Collider[] colliders = obj[i].GetComponentsInChildren<Collider>();
                    foreach (Collider collider in colliders)
                    {
                        log += "\t<color=red>REMOVING COLLIDER:</color> " + collider.name + "\n";

                        // remove the collider
                        DestroyImmediate(collider);

                        // disable the collider
                        //collider.enabled = false;

                        modified = true;
                    }
                    if (modified)
                    {
                        //PrefabUtility.SaveAsPrefabAssetAndConnect(prefab_root, prefab_src, InteractionMode.AutomatedAction);
                        PrefabUtility.SaveAsPrefabAsset(prefab_root, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(prefab_root), out bool success);
                        if (success)
                        {
                            successCount++;
                        }
                        log += "\t<color=yellow>APPLYING PREFAB:</color> " + AssetDatabase.GetAssetPath(prefab_root) + "\n\n";
                    }
                }
            }
            Debug.Log(successCount);
            Debug.Log(log);
        }
        else
        {
            Debug.Log("Nothing selected");
        }
    }
    public static string GetGameObjectPath(GameObject obj)
    {
        string path = "/" + obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = "/" + obj.name + path;
        }
        return path;
    }
}