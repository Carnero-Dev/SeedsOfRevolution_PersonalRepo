# 🚀 CSV Importer Tool: Automatiza la Creación de ScriptableObjects

## 🌟 Visión General

La herramienta **CSV Importer Tool** permite a diseñadores y desarrolladores importar datos masivos desde archivos CSV (hojas de cálculo) directamente a **ScriptableObjects** (SO) en Unity.

Esto elimina la necesidad de crear y actualizar manualmente cientos de SOs, haciendo la gestión de contenido (como provincias, eventos, decisiones, unidades) **rápida, escalable y libre de errores**.

---

## 🎯 Características Principales

### 1. 📂 Importación Genérica por Reflection

* **Compatibilidad Universal:** Funciona con **cualquier clase** que herede de `ScriptableObject`, sin necesidad de modificar el código de la herramienta.
* **Mapeo Inteligente:** Asocia automáticamente las **cabeceras del CSV** (la primera fila) con los **nombres de los campos públicos** del SO. Esto garantiza que el orden de las columnas en el CSV no importa, siempre y cuando los nombres coincidan.

### 2. 🔄 Creación y Actualización Controlada

* **Creación:** Si no existe un SO con el ID único especificado, se crea una nueva instancia.
* **Actualización:** Si el SO ya existe, la herramienta lo carga y **solo sobrescribe los campos** definidos en el CSV. Esto es crucial para **mantener referencias** a otros *assets* o *GameObjects* en la escena que ya apunten al SO existente, evitando roturas de enlaces.

### 3. 🧩 Manejo de Tipos de Datos Complejos

La herramienta no se limita a números y textos, sino que soporta:

* **Tipos Primitivos:** `int`, `float`, `string`, `bool`, etc.
* **Enums:** Conversión automática de *strings* de CSV a tipos `enum`, ignorando mayúsculas y minúsculas para mayor flexibilidad.
* **Colecciones:** Soporte para **Arrays** (`[]`) y **Listas** (`List<T>`).
* **Referencias a SOs:** Permite enlazar automáticamente referencias a otros `ScriptableObject`s existentes en el proyecto buscando por el nombre del *asset* provisto en la celda del CSV.

---

## 🛠️ Guía de Uso Rápido

### 1. 📑 Preparación del CSV

Asegúrate de que tu hoja de cálculo cumple las siguientes convenciones:

| Fila/Columna | Requisito | Ejemplo |
| :--- | :--- | :--- |
| **Cabeceras (Fila 1)** | Deben coincidir **exactamente** con los nombres de los campos públicos del `ScriptableObject` de destino. | `_provinceId`, `_name`, `_population` |
| **Columna ID** | Debe contener un valor **único** para identificar el *asset* (ej. `MAD`, `NUM`). Este valor se usa como el nombre del archivo `.asset`. | `MAD` |
| **Separador de Listas** | Usa el **punto y coma (`;`)** para separar valores dentro de una misma celda que vayan a una lista o *array*. | `10;50;100` |

### 2. 🖥️ Uso de la Ventana de Herramienta

1.  Abre la herramienta desde el menú de Unity: **`Tools / CSV to ScriptableObjects`**.
2.  **Selecciona el Tipo:** Elige la clase `ScriptableObject` (ej. `SO_Province`) a la que quieres importar los datos.
3.  **Selecciona CSV:** Haz clic en "Select CSV" y carga el archivo.
4.  **Columna ID:** En la interfaz, selecciona la cabecera que contiene el **ID único** que se usará para nombrar los *assets*.
5.  **Carpeta de Salida:** Verifica que la carpeta de destino sea correcta (ej. `Assets/Scripts/ScriptableObjects/Provinces`).
6.  **Generar:** Haz clic en "Generate ScriptableObject".

---

## ⚠️ A Considerar

### 1. Convenciones y Mapeo

* **Visibilidad:** La herramienta solo mapea y modifica **campos públicos** (`public float fieldName;`) del `ScriptableObject`. Los campos privados o propiedades con *getter/setter* no serán modificados (lo cual es bueno, ya que protege la lógica interna).
* **Enums:** Para la conversión de `enum`s, usa nombres en el CSV que **coincidan lo más posible** con el `enum` de C#. La herramienta ignora mayúsculas/minúsculas (`capital` = `Capital`), pero no acentos (`Turistico` != `Turístico`).

### 2. Seguridad de Datos

* **Backup:** Siempre se recomienda trabajar con una copia de seguridad de tus SOs críticos o usar el control de versiones (Git) antes de ejecutar una importación masiva.
* **CSV Vacío:** Los campos vacíos en el CSV (`""`) se interpretarán como el valor por defecto del tipo (`0` para números, `null` para referencias, `""` para *strings*).

### 3. Rendimiento

* La herramienta utiliza `AssetDatabase.SaveAssets()` al final de la importación. Para archivos CSV muy grandes (miles de filas), la generación y serialización puede llevar unos segundos, lo cual es normal y necesario para la persistencia de datos.