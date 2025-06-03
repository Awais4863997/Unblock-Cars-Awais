using UnityEngine;

public class InputManager : MonoBehaviour, IInputManager
{
    public LayerMask vehicleMask;
    private ITouchable _currentTouchedVehicle;


    private void Update()
    {
        ProcessInput();
    }

    public void ProcessInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryProcessRaycast(Input.mousePosition);
        }

        //if (Input.touchCount > 0)
        //{
        //    Touch touch = Input.GetTouch(0);
        //    if (touch.phase == TouchPhase.Began)
        //    {
        //        TryProcessRaycast(touch.position);
        //    }
        //}
    }

    private void TryProcessRaycast(Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, vehicleMask))
        {
            _currentTouchedVehicle = hit.collider.GetComponent<ITouchable>();
            if (_currentTouchedVehicle != null)
                _currentTouchedVehicle.GetTouch();
        }
    }


}
