using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Forward,
    Left,
    Right,
    Full_Left,
    Full_Right,
}

public class Vehicle : MonoBehaviour, ITouchable
{
    private IEventHandler events;

    public Direction direction;
    int NumberOfTurns;

    LevelData _LevelData;

    Vector3 carTargetDirection;

    public float overallSpeed;

    public float speed;
    public float rotationSpeed = 2.0f;

    public float minDistanceToTurn;

    [HideInInspector] public bool isMoving;
    bool isTurning;

    float step;

    Animator animator;

    Vector3 targetepointPosition;
    Quaternion targetRotation;

    List<Vector3> positionHistory;
    List<Quaternion> rotationHistory;

    bool isrecording = true;
    bool isReseting;

    float timePassed;
    public float playbackSpeed = 1.0f;

    int frameIndex;

    bool didHitSomeone;

    private void Start()
    {
        events = FindObjectOfType<EventHandler>();

        _LevelData = GetComponentInParent<LevelData>();

        SetNumberOfTurns();
        SetCarTargetDir();

        positionHistory = new List<Vector3>();
        rotationHistory = new List<Quaternion>();

        animator = GetComponent<Animator>();
    }

    

    void FixedUpdate()
    {
        if (didHitSomeone)
            return;

        if (isrecording && isMoving)
        {
            positionHistory.Add(this.transform.position);
            rotationHistory.Add(this.transform.rotation);
        }

        if (isMoving)
        {
            if (isTurning)
            {
                // Calculate the rotation step
                step = overallSpeed * rotationSpeed * Time.deltaTime;

                // Rotate towards the target rotation using a linear interpolation
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);

                // Assuming targetRotation is the desired rotation and transform.rotation is the current rotation
                float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);

                // Set a small threshold to consider the rotation as done
                float rotationThreshold = 0.1f;

                if (angleDifference < rotationThreshold)
                {
                    NumberOfTurns--;

                    isTurning = false;

                    targetepointPosition = getTargetPoint();
                }
            }

            transform.position += overallSpeed * transform.forward * speed * Time.deltaTime;

            if (Vector3.Distance(this.transform.position, targetepointPosition) <= minDistanceToTurn && targetepointPosition != Vector3.zero)
            {
                ChangeDirection();
            }
        }

        if (isReseting)
        {
            timePassed += Time.deltaTime;

            if (timePassed >= 1.0 / playbackSpeed)
            {
                frameIndex--;

                this.transform.position = positionHistory[frameIndex];
                this.transform.rotation = rotationHistory[frameIndex];

                timePassed = 0f;
            }

            if (frameIndex <= 0)
            {
                isReseting = false;

                isrecording = true;

                positionHistory.Clear();
                rotationHistory.Clear();

                SetNumberOfTurns();
            }
        }
    }


    void SetNumberOfTurns()
    {
        if (direction == Direction.Forward) NumberOfTurns = 0;
        if (direction == Direction.Right || direction == Direction.Left) NumberOfTurns = 1;
        if (direction == Direction.Full_Left || direction == Direction.Full_Right) NumberOfTurns = 2;
    }

    void SetCarTargetDir()
    {
        if (direction == Direction.Forward) carTargetDirection = this.transform.forward;
        if (direction == Direction.Right || direction == Direction.Full_Right) carTargetDirection = this.transform.right;
        if (direction == Direction.Left || direction == Direction.Full_Left) carTargetDirection = -this.transform.right;
    }


    public void GetTouch()
    {
        if (!isMoving)
        {
            if (_LevelData.AllCarStatic() == false)
                return;

            events.Publish("OnMoveSound");

            isMoving = true;

            targetepointPosition = getTargetPoint();
        }
    }



    void ChangeDirection()
    {
        if (NumberOfTurns == 0) return;

        targetepointPosition = Vector3.zero;

        targetRotation = getDirection();

        isTurning = true;
    }

    Quaternion getDirection()
    {
        if (this.direction == Direction.Left || this.direction == Direction.Full_Left)
        {
            return Quaternion.Euler(0f, this.transform.eulerAngles.y - 90f, 0f);
        }

        if (this.direction == Direction.Right || this.direction == Direction.Full_Right)
        {
            return Quaternion.Euler(0f, this.transform.eulerAngles.y + 90f, 0f);
        }

        return Quaternion.Euler(0f, 0f, 0f);
    }

    // Comparison function for sorting RaycastHit array based on distance
    private int CompareRaycastHits(RaycastHit hit1, RaycastHit hit2)
    {
        float distanceToCar = Vector3.Distance(hit1.point, this.transform.position);
        float distanceToCar2 = Vector3.Distance(hit2.point, this.transform.position);

        // Compare distances and return the appropriate result
        if (distanceToCar < distanceToCar2)
        {
            return -1; // hit1 comes before hit2
        }
        else if (distanceToCar > distanceToCar2)
        {
            return 1; // hit1 comes after hit2
        }
        else
        {
            return 0; // distances are equal
        }
    }

    Vector3 getTargetPoint()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, 1 << 6);

        System.Array.Sort(hits, CompareRaycastHits);

        Vector3 targetPoint = Vector3.zero;

        foreach (RaycastHit hit in hits)
        {
            if (isRoadPointValid(hit.collider.transform.position))
            {
                return hit.collider.gameObject.transform.position;
            }
        }
        return targetPoint;
    }

    bool isRoadPointValid(Vector3 point)
    {
        Ray ray = new Ray(point, carTargetDirection);
        RaycastHit hit;

        // is there a land on that direction
        if (!Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 7))
        {
            return true;
        }

        return false;
    }

    void OnHit(Collider c)
    {
        if (!isMoving)
        {
            float dotProduct = Vector3.Dot(c.transform.forward.normalized, this.transform.forward.normalized);

            if (dotProduct > 0)
                animator.Play("OnHitBack");
            if (dotProduct < 0)
                animator.Play("OnHitFront");

        }

        if (this.isMoving)
        {
            isMoving = false;
            isTurning = false;

            isrecording = false;

            Invoke(nameof(Reset), 0.5f);

            frameIndex = positionHistory.Count - 1;
        }
    }

    public void StopCar()
    {
        didHitSomeone = true;
    }

    void Reset()
    {
        isReseting = true;
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.tag == "Car")
        {
            OnHit(c);
        }

        if (c.tag == "EscapeTrigger")
        {
            _LevelData.CarEscape(this.transform.gameObject);
        }
    }


}
