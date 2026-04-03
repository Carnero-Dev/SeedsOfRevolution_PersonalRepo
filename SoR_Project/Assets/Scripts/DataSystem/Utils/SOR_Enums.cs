using System;
using UnityEngine;
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
    public enum EventCategory {
        Generic,
        Political,
        Economic
    }
    [Serializable]
    public struct EventStorage {
        public string eventId;
        public string title;
        [TextArea] public string description;
        public EventCategory category;
        public Sprite thumbnail;
        public int weight;
        public bool isUnique;
        [Tooltip("Optional, if null, use weight")] 
        public DateTime triggerDate;
        public int triggerHour;
        public SO_Decision[] decisions;
    }
     #endregion
}
