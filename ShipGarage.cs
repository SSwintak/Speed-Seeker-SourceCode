using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ShipGarage : MonoBehaviour
{
    
    public static ShipGarage Instance {get; private set;}
        
    public Dictionary<string, ShipStats> garage {get; private set;}

    [SerializeField, ES3NonSerializable] SerializedDictionary<string, ShipObject> _AllShips = new SerializedDictionary<string, ShipObject>();
    public SerializedDictionary<string, ShipObject> AllShips => _AllShips;

    public static string SaveKey => "ShipGarage";
    
    public string SelectedShip;

    private void Awake() 
    {
        if (!Instance) Instance = this;

        if(!ES3.FileExists(SavePath.SaveFilePath)) garage = new Dictionary<string, ShipStats>();
        else LoadGarageFromSave(SaveKey);
    }
    
    /// <summary>
    /// Adds a purchased ship to the players garage
    /// </summary>
    /// <remarks>
    /// parameter: "name"; If left empty/null it will get the name through the ShipObject component (scriptable object)
    /// </remarks>
    public void AddShip(ShipStats ship, string shipName)
    {
        
        if (!garage.ContainsKey(shipName))
        {
            garage.Add(shipName, ship);
        }
        SaveGarage();
        Debug.Log("Ships in Garage: " + garage.Count);
    }

    public void UpdateShipStats(ShipStats newStats, string name)
    {
        if(!HasShip(name)) return;

        var currentStats = garage[name];
        if(currentStats != newStats)
        {
            garage[name] = new ShipStats(newStats);
        }
        SaveGarage();
    }

    public void SaveGarage()
    {        
        ES3.Save(SaveKey, garage);        
    }

    /// <summary>
    /// Returns the ships stats by name
    /// </summary>
    public ShipStats GetShipStats(string name)
    {
        if(!garage.ContainsKey(name))
        {
            return new ShipStats();
        } 
            
        ShipStats ship = new ShipStats(garage[name]);
        return ship;
    }

    public bool HasShip(string name)
    {
        Assert.IsNotNull(garage, "Error, garage is null!");
        return garage.ContainsKey(name);
    }

    private void LoadGarageFromSave(string saveKey)
    {
        if(!ES3.FileExists(SavePath.SaveFilePath) || !ES3.KeyExists(saveKey)) garage = new Dictionary<string, ShipStats>();
        garage = ES3.Load<Dictionary<string, ShipStats>>(saveKey, new Dictionary<string, ShipStats>());
        Assert.IsNotNull(garage, "Error, garage is null!");
    }
}
