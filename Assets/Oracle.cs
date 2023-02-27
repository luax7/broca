using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Oracle : MonoBehaviour
{
    public Transform Pointer;
    public Transform Base;
    public Transform Plate;
    public GameObject Canvas;

    private CharacterController PointerController;
    private CharacterController BaseController;

    public List<Vector2> Points ;

    [Space]
    [Header("Ranges")]
    [Space]
    [SerializeField]
    [Range(0f, 2)] private float PointerRange;

    [SerializeField]
    [Range(0f, 2)] private float baseRange;
    [SerializeField]
    [Range(0f, 0.05f)] private float Error;

    [Space]
    [Header("Points")]
    [Space]
    private bool Returning;
    private int PointIndex = 0;
    [Range(0f,10f)]public float speed = 0.2f;
    [SerializeField] private Vector2 Destination;

    [SerializeField] private int Going = 0;
    [SerializeField] private RaycastHit hit;
    private bool Movinhead = false;
    [SerializeField] [Range(0f,1.5f)] private float HeadMovementTime = 0.5f;
    [SerializeField][Range(0f, 1f)] private float ExceedTime = 0.300f;

    private float HeadMovementTimer = 0f;

    [Space]
    [Header("UI")]
    private bool UiState = true;
    public Slider SpeedSlider;
    public Slider ErrorSlider;

    public GameObject prefab;
    private List<GameObject> prefabs = new List<GameObject>(0);

    private void Start()
    {
        Destination = Points[PointIndex];

        PointerController = Pointer.GetComponent<CharacterController>();
        BaseController = Base.GetComponent<CharacterController>();
        
    }
    private bool MovePointerToDestination(float percentage)
    {
        // Calculate the target position
        float targetX = (PointerRange * percentage / 100f) - (PointerRange / 2f);
        Vector3 targetPosition = new Vector3(targetX, Pointer.position.y, Pointer.position.z);

        // Move towards the target position
        Pointer.position = Vector3.Lerp(Pointer.position, targetPosition, speed * Time.deltaTime);

        return Vector3.Distance(Pointer.position, targetPosition) < Error;
    }


    private bool MoveBaseToDestination(float percentage)
    {
        // Calculate the target position
        float targetZ =  (baseRange  * percentage / 100f) - (baseRange / 2f);
        Vector3 targetPosition = new Vector3(Base.position.x, Base.position.y, targetZ);

        // Move towards the target position
        Base.position = Vector3.Lerp(Base.position, targetPosition, speed * Time.deltaTime);

        return Vector3.Distance(Base.position, targetPosition) < Error;
    }


    private void Update()
    {


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UiState = !UiState;
            Canvas.SetActive(UiState);
        }
        if(!UiState ) { Work(); }

       
    }
    private void Work()
    {
        bool PointerInPos = false;
        bool BaseInPos = false;
        if (!Movinhead)
        {
            PointerInPos = MovePointerToDestination(Destination.x / 10);
            BaseInPos = MoveBaseToDestination(Destination.y / 10);
        }

        Ray ray = new Ray(Pointer.transform.GetChild(0).position, Vector3.down);

        Physics.Raycast(ray, out RaycastHit hit);

        if (PointerInPos && BaseInPos)
        {
            Movinhead = true;
        }

        //Descendo a cabeça
        if (Movinhead && !Returning)
        {
            HeadMovementTimer += Time.deltaTime;

            float currentHeadY = Mathf.Lerp(0f, -0.3f - ExceedTime, HeadMovementTimer / HeadMovementTime);
            Pointer.GetChild(0).position = new Vector3(Pointer.position.x, Pointer.position.y + currentHeadY, Pointer.position.z);

            if (HeadMovementTimer >= HeadMovementTime)
            {
                Returning = true;
                Movinhead = false;
                HeadMovementTimer = 0f;
                GameObject Instance = Instantiate(prefab);
                Instance.transform.position = hit.point;
                Instance.transform.SetParent(Base);
                prefabs.Add(Instance);
            }
        }

        //Subindo a cabeça
        if (Movinhead && Returning)
        {
            HeadMovementTimer += Time.deltaTime;

            float currentHeadY = Mathf.Lerp(-0.3f - ExceedTime, 0f, HeadMovementTimer / HeadMovementTime);
            Pointer.GetChild(0).position = new Vector3(Pointer.position.x, Pointer.position.y + currentHeadY, Pointer.position.z);

            if (HeadMovementTimer >= HeadMovementTime)
            {
                PointIndex++;
                if (PointIndex == Points.Count) PointIndex = 0;

                Destination = Points[PointIndex];
                Returning = false;
                Movinhead = false;
                HeadMovementTimer = 0f;
            }
        }
    }

    public void ChangeValues()
    {
        speed = SpeedSlider.value * 10;
        Error = ErrorSlider.value * 0.05f;
    }
}
