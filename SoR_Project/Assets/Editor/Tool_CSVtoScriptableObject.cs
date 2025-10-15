using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Tool_CSVtoScriptableObject : EditorWindow
{
    private string filePath = "";
    private string outputFolder = "Assets/Scripts/ScriptableObjects";
    private string[,] csvData;
    private Type selectedType;
    private List<Type> scriptableObjectTypes;

    [MenuItem("Tools/CSV to ScriptableObjects")]
    public static void ShowWindow() {
        GetWindow<Tool_CSVtoScriptableObject>("CSV to ScriptableObject");
    }

    private void OnEnable() {
        //* Obtiene todos los tipos que heredan de ScriptableObject
        // scriptableObjectTypes = AppDomain.CurrentDomain.GetAssemblies()
        //     .SelectMany(assembly => assembly.GetTypes())
        //     .Where(type => type.IsSubclassOf(typeof(ScriptableObject)) && type.Name.StartsWith("SO_"))
        //     .ToList();
        string[] excludedTypes = { "Readme" };
        scriptableObjectTypes = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => assembly.GetName().Name == "Assembly-CSharp")
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.BaseType == typeof(ScriptableObject) && !excludedTypes.Contains(type.Name))
            .ToList();
    }
    
    public void OnGUI() {
        GUILayout.Label("Import to CSV and generate ScriptableObjects", EditorStyles.boldLabel);
        
        // * Selección del tipo de ScriptableObject
        GUILayout.Label("Select type of ScriptableObjects: ", EditorStyles.label);
        if (scriptableObjectTypes.Count > 0) {
            string[] typeNames = scriptableObjectTypes.Select(t => t.Name).ToArray();
            int selectedIndex = selectedType == null ? 0 : scriptableObjectTypes.IndexOf(selectedType);
            selectedIndex = EditorGUILayout.Popup(selectedIndex, typeNames);
            selectedType = scriptableObjectTypes[selectedIndex];
        } else {
            GUILayout.Label("ScriptableObjects not found", EditorStyles.boldLabel);
            return;
        }     

        //* Botón para seleccionar archivo CSV
        if (GUILayout.Button("Select CSV")) {        
            filePath = EditorUtility.OpenFilePanel("Select CSV File", "", "csv");
            if (!string.IsNullOrEmpty(filePath)) {
                csvData = CSVImporter.ImportCSV(filePath);
                Debug.Log("File selected succesfully");
            }
        }

        //* Mostrar ruta del archivo seleccionado
        if (!string.IsNullOrEmpty(filePath)) {
            EditorGUILayout.LabelField("File Selected: ", filePath);
        }

        //* Carpeta de salida
        outputFolder = EditorGUILayout.TextField("Output Folder: ", outputFolder);
        
        //* Previsualizacion de datos del CSV
        if (csvData !=null) {
            GUILayout.Label("Preview of CSV", EditorStyles.boldLabel);
            for (int r = 0; r < Mathf.Min(10, csvData.GetLength(0)); r++)  { // Máximo 10 filas para mostrar
                GUILayout.BeginHorizontal();
                for (int c = 0; c < csvData.GetLength(1); c++) {
                    GUILayout.Label(csvData[r, c], GUILayout.Width(100));
                }                
            GUILayout.EndHorizontal();
            }
            // Muestra 3 puntos suspensivos y se han ocultado filas
            if (csvData.GetLength(0) > 10) {
                GUILayout.Label("[...]");
            }
        }

        //* Botón para generar ScriptableObjects
        if (csvData !=null && GUILayout.Button("Generate ScriptableObject")) {
            GenerateScriptableObjects(csvData, $"{outputFolder}/{selectedType.Name.Substring(3)}s", selectedType);
        }
    }

/// <summary>
/// Hace un checkeo de si existe ya un SO en la ruta para modificarlo en vez de crearlo
/// </summary>
/// <param name="name"></param>
/// <param name="folder"></param>
/// <param name="type"></param>
/// <returns></returns>
    private ScriptableObject UpdateOrCreateScriptableObject(string name, string folder, Type type) {
        //* Crea la ruta completa derl asset
        string assetPath = Path.Combine(folder, $"{name}.asset");

        // * Intenta cargar el ScriptableObject existente
        ScriptableObject existingObject = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);

        if (existingObject !=null) {
            return existingObject;
        }
        ScriptableObject newObject = CreateInstance(type);
        AssetDatabase.CreateAsset(newObject, assetPath);
        return newObject;
    }
/// <summary>
/// Genera los scriptables objects según las filas y columnas del CSV dado en la ruta indicada para el tipo dd SO indicado
/// </summary>
/// <param name="data"></param>
/// <param name="folder"></param>
    private void GenerateScriptableObjects(string[,] data, string folder, Type type) {
        if (!Directory.Exists(folder)) {
            Directory.CreateDirectory(folder);
        }
        for (int r = 1; r < data.GetLength(0); r++) { // Comienza en 1 para saltar encabezados
            string assetName = data[r, 0];
            ScriptableObject newData = UpdateOrCreateScriptableObject(assetName, folder, type);

            var fields = type.GetFields();
            for (int c = 0; c < fields.Length && c < data.GetLength(1); c++) {
                try {
                    object value = Convert.ChangeType(data[r, c], fields[c].FieldType);
                    fields[c].SetValue(newData, value);
                } catch (Exception e) {
                    Debug.LogWarning($"Cant assign the value: {data[r, c]} to the field {fields[c].Name}: {e.Message}");
                }
            }
            EditorUtility.SetDirty(newData); // Lo marcamos como modificado

        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("ScriptableObjects generated and updated succesfuly");
    }
    
}
