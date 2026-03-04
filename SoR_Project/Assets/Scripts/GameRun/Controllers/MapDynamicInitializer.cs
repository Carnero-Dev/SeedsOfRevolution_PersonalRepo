using UnityEngine;
using System.Collections.Generic;

public class MapDynamicInitializer : MonoBehaviour {
    [SerializeField] public SO_MapTemplate currentMap;
    [SerializeField] private Material mapMaterialBase; // Material URP Unlit/Lit

    public void Initialize() {
        // 1. Crear el objeto visual del mapa
        GameObject mapObj = new GameObject("Runtime_Map_Grid");
        MeshFilter filter = mapObj.AddComponent<MeshFilter>();
        MeshRenderer renderer = mapObj.AddComponent<MeshRenderer>();
        
        // Creamos una malla plana simple ajustada al aspecto de la imagen
        filter.mesh = CreatePlaneMesh(currentMap.colorMap.width, currentMap.colorMap.height);
        
        // 2. Configurar Material
        Material instanceMat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        instanceMat.mainTexture = currentMap.colorMap;
        renderer.material = instanceMat;

        // 3. Configurar Colisionador para Clics
        mapObj.AddComponent<MeshCollider>();
        
        // 5. Añadir el Handler de Interacción
        mapObj.layer = LayerMask.NameToLayer("Interactable");
        var handler = mapObj.AddComponent<MapInteractionHandler>();
        handler.Setup(currentMap.colorMap);
    }

    private Mesh CreatePlaneMesh(int w, int h) {
        Mesh m = new Mesh();
        float aspect = (float)w / h;
        // Genera vértices de un plano centrado
        m.vertices = new Vector3[] {
            new Vector3(-aspect, 0, -1), new Vector3(aspect, 0, -1),
            new Vector3(-aspect, 0, 1), new Vector3(aspect, 0, 1)
        };
        m.uv = new Vector2[] { new Vector2(0,0), new Vector2(1,0), new Vector2(0,1), new Vector2(1,1) };
        m.triangles = new int[] { 0, 2, 1, 2, 3, 1 };
        m.RecalculateNormals();
        return m;
    }
}