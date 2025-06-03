using System.Collections.Generic;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    private IEventHandler events;

    public List<Vehicle> CarsInLevel;
    int carsInitialCount;
    GameObject carEscaped;

    public Transform CamPos;

    private void Start()
    {
        events = FindObjectOfType<EventHandler>();
        Camera.main.transform.SetPositionAndRotation(CamPos.position, CamPos.rotation);

        carsInitialCount = CarsInLevel.Count;
    }

    public bool AllCarStatic()
    {
        if (CarsInLevel == null || CarsInLevel.Count == 0)
            return true;

        foreach (Vehicle c in CarsInLevel)
        {
            if (c != null && c.isMoving)
                return false;
        }

        return true;
    }

    public void CarEscape(GameObject car)
    {
        carEscaped = car;

        CarsInLevel.Remove(car.GetComponent<Vehicle>());

        if (CarsInLevel.Count <= 0)
        {
            events.Publish("OnWin");
        }

        events.Publish("OnEscapeEffect");
        events.Publish("OnEscapeSound");

        Invoke(nameof(DestroyCar), 0.4f);

    }

    void DestroyCar()
    {
        Destroy(carEscaped);
    }

}



