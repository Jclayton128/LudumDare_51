using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RepairMouseHandler : MonoBehaviour
{
    [Header("Player subsystem repair")]
    [SerializeField][Range(0, 3)] int subsystemIndex = 0;
    [Space]
    [Space]
    [Header("UI / Event")]

    TimeController _timeController;

    GraphicRaycaster m_Raycaster;
    EventSystem m_EventSystem;

    PointerEventData m_PointerEventData;

    GameObject player;
    StatsHandler stats;
    InputController input;
    Button button;

    float timeDebounceFindPlayer = 0f;

    bool _shouldBeRepairing = false;

    private void Awake()
    {
        FindPlayer();
        m_Raycaster = FindObjectOfType<GraphicRaycaster>();
        m_EventSystem = FindObjectOfType<EventSystem>();

        _timeController = FindObjectOfType<TimeController>();
        _timeController.OnNewPhase += HandlePhaseChange;
        button = GetComponent<Button>();
        button.interactable = false;

    }

    private void HandlePhaseChange(TimeController.Phase newPhase)
    {
        if (newPhase == TimeController.Phase.C_healing)
        {
            button.interactable = true;
        }
        else
        {
            button.interactable = false;
        }
    }

    public void OnPointerDown()
    {
        if (!input.IsFirePressed) return;
        Debug.Log($"pointer is down on system {subsystemIndex}");
        _shouldBeRepairing = true;
    }

    public void OnPointerExit()
    {
        Debug.Log($"pointer is exiting system {subsystemIndex}");
        _shouldBeRepairing = false;
    }

    public void OnPointerUp()
    { 
        Debug.Log($"pointer is released from system {subsystemIndex}");
        _shouldBeRepairing = false;
    }


    void Update()
    {
        if (_shouldBeRepairing)
        {
            stats.RepairDamage(subsystemIndex);
        }

        //if (player == null)
        //{
        //    FindPlayer();
        //    return;
        //}
        //// SOLUTION BELOW FROM THIS FORUM THREAD: https://forum.unity.com/threads/how-to-raycast-onto-a-unity-canvas-ui-image.855259/
        //if (!input.IsFirePressed) return;
        ////Set up the new Pointer Event
        //m_PointerEventData = new PointerEventData(m_EventSystem);
        ////Set the Pointer Event Position to that of the game object
        //m_PointerEventData.position = input.MousePositionScreen;
        ////Create a list of Raycast Results
        //List<RaycastResult> results = new List<RaycastResult>();
        ////Raycast using the Graphics Raycaster and mouse click position
        //m_Raycaster.Raycast(m_PointerEventData, results);
        //if (results.Count > 0) OnHit(results[0].gameObject);
    }

    void OnHit(GameObject raycastedGameObject)
    {
        if (!input.IsFirePressed) return;
        if (raycastedGameObject != gameObject) return;
        stats.RepairDamage(subsystemIndex);
    }

    void FindPlayer()
    {
        if (timeDebounceFindPlayer > 0)
        {
            timeDebounceFindPlayer -= Time.deltaTime;
            return;
        }
        timeDebounceFindPlayer = 0.2f;
        player = GameObject.FindWithTag("Player");
        if (player == null) return;
        stats = player.GetComponent<StatsHandler>();
        input = player.GetComponent<InputController>();
        AppIntegrity.Assert(stats != null, "stats should not be null");
        AppIntegrity.Assert(input != null, "input should not be null");
    }
}
