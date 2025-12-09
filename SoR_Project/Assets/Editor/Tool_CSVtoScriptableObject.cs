using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

public class Tool_CSVtoScriptableObject : EditorWindow
{
    private string filePath = "";
    private string outputFolder = "Assets/Scripts/ScriptableObjects";
    private string[,] csvData;
    private Type selectedType;
    private List<Type> scriptableObjectTypes;
    private int idColumnIndex = 0;

    [MenuItem("Tools/CSV to ScriptableObjects")]
    public static void ShowWindow() {
        GetWindow<Tool_CSVtoScriptableObject>("CSV to ScriptableObject");
    }

    private void OnEnable() {
        //* Obtiene todos los tipos que heredan de ScriptableObject
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

        // INTERFAZ PARA SELECCIONAR LA COLUMNA ID ÚNICA
        // Muestra los nombres de las columnas (headers) para elegir cuál es la ID
        if (!string.IsNullOrEmpty(filePath)) {
            EditorGUILayout.LabelField("File Selected: ", filePath);  
            if (csvData != null && csvData.GetLength(0) > 0) {
                string[] headers = Enumerable.Range(0, csvData.GetLength(1))
                                            .Select(c => csvData[0, c].Trim())
                                            .ToArray();
                
                GUILayout.Space(10);
                GUILayout.Label("Select ID Column (Asset Name / Unique Key):", EditorStyles.boldLabel);
                idColumnIndex = EditorGUILayout.Popup(idColumnIndex, headers);
            }
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
            string finalFolder = $"{outputFolder}/{selectedType.Name.Substring(3)}s"; 
            GenerateScriptableObjects(csvData, finalFolder, selectedType, idColumnIndex);
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
    private void GenerateScriptableObjects(string[,] data, string folder, Type type, int idIndex) {
        if (!Directory.Exists(folder)) {
            Directory.CreateDirectory(folder);
        }

        if (data.GetLength(0) < 2) {
            Debug.LogError("CSV vacío o sin encabezados");
            return;
        }

        string[] headers = Enumerable.Range(0, data.GetLength(1))
                            .Select(c => data[0, c].Trim())
                            .ToArray();

        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance)              
                    .ToDictionary(f => f.Name, f => f);

        var fieldColumnMap = new Dictionary<System.Reflection.FieldInfo, int>();
        
        for (int i = 0; i < headers.Length; i++) {
            string headerName = headers[i];
            if (fields.ContainsKey(headerName)) {
                fieldColumnMap.Add(fields[headerName], i); 
            }
        }
       if (!Directory.Exists(folder)) { Directory.CreateDirectory(folder); }

        for (int r = 1; r < data.GetLength(0); r++) { 
            string assetName = data[r, idIndex]; 
            ScriptableObject newData = UpdateOrCreateScriptableObject(assetName, folder, type);

            foreach (var pair in fieldColumnMap) {
                System.Reflection.FieldInfo field = pair.Key;
                int columnIndex = pair.Value;
                string rawValue = data[r, columnIndex];

                try {
                    object convertedValue = ParseValue(rawValue, field.FieldType);
                    field.SetValue(newData, convertedValue);
                } catch (Exception e) {
                    Debug.LogWarning($"[Error en {assetName}.{field.Name}] No se puede asignar '{rawValue}' al tipo {field.FieldType}: {e.Message}");
                }
            }
        
        EditorUtility.SetDirty(newData);
    }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("ScriptableObjects generated and updated succesfuly");
    }
    /// <summary>
    /// Parsea el valor de la celda del CSV y lo convierte al tipo indicado 
    /// </summary>
    private object ParseValue(string rawValue, Type targetType) {
        if (string.IsNullOrEmpty(rawValue)) {
            // Devuelve el valor por defecto para el tipo si la celda está vacía
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }
        // MANEJO ESPECÍFICO DE ENUMS
        if (targetType.IsEnum) {
            try {                
                string cleanValue = rawValue.Trim();
                return Enum.Parse(targetType, cleanValue, ignoreCase: true);

            } catch (Exception) {
                // Si la conversión falla, registra un error y devuelve el valor por defecto (el primer elemento del enum, generalmente 0)
                Debug.LogError($"Fallo al parsear '{rawValue}' a Enum {targetType.Name}. Revise acentos o nombres.");
                return Activator.CreateInstance(targetType); 
            }
        }
        
        // Manejo de Tipos Simples (Strings, int, float, bool, etc.)
        if (targetType.IsPrimitive || targetType == typeof(string)) {
            return Convert.ChangeType(rawValue, targetType);
        }
        
        // ----------------------------------------------------
        // Manejo de Tipos Colección (Arrays o List<T>)
        // Usamos el punto y coma (;) como separador interno en la celda CSV, por convención.
        if (targetType.IsArray) {
            Type elementType = targetType.GetElementType();
            string[] elements = rawValue.Split(';');

            Array array = Array.CreateInstance(elementType, elements.Length);
            
            for (int i = 0; i < elements.Length; i++) {
                array.SetValue(ParseValue(elements[i].Trim(), elementType), i);
            }
            return array;
        }
        
        if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(List<>)) {
            Type elementType = targetType.GetGenericArguments()[0];
            string[] elements = rawValue.Split(';');
            
            var list = Activator.CreateInstance(targetType);
            var addMethod = targetType.GetMethod("Add");
            
            foreach (string element in elements) {
                object parsedElement = ParseValue(element.Trim(), elementType);
                addMethod.Invoke(list, new object[] { parsedElement });
            }
            return list;
        }
        // ----------------------------------------------------
        // Manejo de Referencias a ScriptableObject (Busca el SO por nombre)
        if (targetType.IsSubclassOf(typeof(ScriptableObject))) {
            // rawValue debería ser el nombre del SO (ej: "SO_Evento_Inicio")
            string assetName = rawValue.Trim();
            // Buscar el asset en todo el proyecto
            string[] guids = AssetDatabase.FindAssets($"{assetName} t:{targetType.Name}");
            
            if (guids.Length > 0) {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath(path, targetType);
            }
            Debug.LogWarning($"Referencia a SO no encontrada: '{assetName}' de tipo {targetType.Name}");
            return null; 
        }
        
        // Caso por defecto (Aqui se añaden nuevos casos)
        return Convert.ChangeType(rawValue, targetType);
    }
}
