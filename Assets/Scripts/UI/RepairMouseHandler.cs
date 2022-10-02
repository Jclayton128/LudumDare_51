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
    [SerializeField] GraphicRaycaster m_Raycaster;
    [SerializeField] EventSystem m_EventSystem;

    PointerEventData m_PointerEventData;
    StatsHandler stats;
    InputController input;

    private void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");
        AppIntegrity.Assert(player != null, "CANNOT FIND PLAYER - has the player spawned?");
        stats = player.GetComponent<StatsHandler>();
        input = player.GetComponent<InputController>();
    }

    void Update()
    {
        // SOLUTION BELOW FROM THIS FORUM THREAD: https://forum.unity.com/threads/how-to-raycast-onto-a-unity-canvas-ui-image.855259/
        if (!input.IsFirePressed) return;
        //Set up the new Pointer Event
        m_PointerEventData = new PointerEventData(m_EventSystem);
        //Set the Pointer Event Position to that of the game object
        m_PointerEventData.position = input.MousePositionScreen;
        //Create a list of Raycast Results
        List<RaycastResult> results = new List<RaycastResult>();
        //Raycast using the Graphics Raycaster and mouse click position
        m_Raycaster.Raycast(m_PointerEventData, results);
        if (results.Count > 0) OnHit(results[0].gameObject);
    }

    void OnHit(GameObject raycastedGameObject)
    {
        if (!input.IsFirePressed) return;
        if (raycastedGameObject != gameObject) return;
        stats.RepairDamage(subsystemIndex);
    }
}
