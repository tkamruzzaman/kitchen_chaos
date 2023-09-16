using UnityEditor;
using UnityEngine;

//We connect the editor with the KitchenObjectSO class
[CustomEditor(typeof(KitchenObjectSO))]

public class KitchenObjectSOEditor : Editor
{
    //Here we grab a reference to our KitchenObjectSO
    KitchenObjectSO kitchenObjectSO;

    private void OnEnable()
    {
        //target is by default available for you
        //because we inherite Editor
        kitchenObjectSO = target as KitchenObjectSO;
    }

    public override void OnInspectorGUI()
    {
        //Draw whatever we already have in SO definition
        base.OnInspectorGUI();

        //Guard clause
        if (kitchenObjectSO.sprite == null)
            return;

        //Convert the sprite (see SO script) to Texture
        Texture2D texture = AssetPreview.GetAssetPreview(kitchenObjectSO.sprite);
        //We crate empty space 80x80 (you may need to tweak it to scale better your sprite
        //This allows us to place the image JUST UNDER our default inspector
        GUILayout.Label("", GUILayout.Height(80), GUILayout.Width(80));
        //Draws the texture where we have defined our Label (empty space)
        GUI.DrawTexture(GUILayoutUtility.GetLastRect(), texture);
    }
}
