using UnityEngine;

public class Utilites {

	public static void setLayerRecursively(GameObject gameObject, int layer) {
        if (gameObject == null)
            return;

        gameObject.layer = layer;

        foreach (Transform child in gameObject.transform) {
            if (child == null)
                continue;

            setLayerRecursively(child.gameObject, layer);
        }
    }
}
