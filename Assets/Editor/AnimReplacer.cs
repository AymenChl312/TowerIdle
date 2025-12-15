using UnityEngine;
using UnityEditor;
using System.IO; 

public class AnimReplacer : EditorWindow
{
    AnimationClip sourceClip;
    Object[] newSprites;
    string newName = "New_Animation";

    [MenuItem("Tools/Animation Replacer")]
    public static void ShowWindow()
    {
        GetWindow<AnimReplacer>("Anim Replacer");
    }

    void OnGUI()
    {
        GUILayout.Label("Dupliquer une anim en bouclant les sprites", EditorStyles.boldLabel);

        sourceClip = (AnimationClip)EditorGUILayout.ObjectField("Animation Source", sourceClip, typeof(AnimationClip), false);

        newName = EditorGUILayout.TextField("Nouveau Nom", newName);

        GUILayout.Label("Sélectionne tes sprites (Même s'il en manque, ça bouclera !)", EditorStyles.helpBox);

        if (GUILayout.Button("UTILISER LA SÉLECTION (" + Selection.objects.Length + " sprites)"))
        {
            newSprites = Selection.objects;
        }

        if (newSprites != null && newSprites.Length > 0)
        {
            GUILayout.Label(newSprites.Length + " sprites sélectionnés.");
        }

        GUILayout.Space(10);

        if (GUILayout.Button("CRÉER LA NOUVELLE ANIMATION"))
        {
            CreateNewClip();
        }
    }

    void CreateNewClip()
    {
        if (sourceClip == null || newSprites == null || newSprites.Length == 0)
        {
            Debug.LogError("Il manque des infos ! (Source ou Sprites)");
            return;
        }

        string folderPath = "Assets/Animations";

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            AssetDatabase.Refresh(); 
        }

        string newPath = folderPath + "/" + newName + ".anim";
        newPath = AssetDatabase.GenerateUniqueAssetPath(newPath); 

        AnimationClip newClip = Instantiate(sourceClip);
        newClip.name = newName;

        var bindings = AnimationUtility.GetObjectReferenceCurveBindings(sourceClip);

        foreach (var binding in bindings)
        {
            if (binding.propertyName == "m_Sprite")
            {
                ObjectReferenceKeyframe[] keyframes = AnimationUtility.GetObjectReferenceCurve(sourceClip, binding);

                for (int i = 0; i < keyframes.Length; i++)
                {
                    int spriteIndex = i % newSprites.Length;

                    if (newSprites[spriteIndex] is Sprite)
                    {
                        keyframes[i].value = newSprites[spriteIndex];
                    }
                }

                AnimationUtility.SetObjectReferenceCurve(newClip, binding, keyframes);
            }
        }

        AssetDatabase.CreateAsset(newClip, newPath);
        AssetDatabase.SaveAssets();

        Debug.Log("Animation créée avec boucle : " + newPath);
        EditorGUIUtility.PingObject(newClip);
    }
}