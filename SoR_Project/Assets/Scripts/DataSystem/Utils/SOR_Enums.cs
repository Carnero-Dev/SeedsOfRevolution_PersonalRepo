using System;
/// <summary>
/// Contenedor de Enums, para centralizar en todo el proyeto el uso de enums y evitar errores a la hora de utilizar los mismos y modificarlos de forma global.
/// Author: Carlos Carnero Cabrera
/// </summary>
public class SOR_Enums
{
    #region Provinces
     [Serializable] public enum ProvinceType {
        Default,
        Industrial,
        Turistic,
        Capital
    }
     public static ProvinceType provinceType;
    [Serializable]
     public enum GameModes {
        Game, UI
     }
    [Serializable]
     public enum Parameters {
         Infuelnce,
         Popularity,
         Affiliates,
         Aligned,
         Fame,
         Determination,
         Stability
     }
    [Serializable]
    public struct ParameterValue{
        public Parameters parameter;
        public float value;
    }
     #endregion
}
