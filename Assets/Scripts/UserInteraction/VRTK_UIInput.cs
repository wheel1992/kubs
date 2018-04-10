using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VRTK;

[RequireComponent(typeof(VRTK_DestinationMarker))]
public class VRTK_UIInput : MonoBehaviour
{
    private VRTK_DestinationMarker laserPointer;

    private void OnEnable()
    {
		if (GetComponent<VRTK_DestinationMarker>() == null)
        {
            VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_ControllerPointerEvents_ListenerExample", "VRTK_DestinationMarker", "the Controller Alias"));
            return;
        }

        laserPointer = GetComponent<VRTK_DestinationMarker>();
        laserPointer.DestinationMarkerEnter -= HandlePointerIn;
        laserPointer.DestinationMarkerEnter += HandlePointerIn;
        laserPointer.DestinationMarkerExit -= HandlePointerOut;
        laserPointer.DestinationMarkerExit += HandlePointerOut;
        laserPointer.DestinationMarkerSet -= HandleTriggerClicked;
        laserPointer.DestinationMarkerSet += HandleTriggerClicked;
    }

    private void HandleTriggerClicked(object sender, DestinationMarkerEventArgs e)
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            ExecuteEvents.Execute(EventSystem.current.currentSelectedGameObject, new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
        }
    }

    private void HandlePointerIn(object sender, DestinationMarkerEventArgs e)
    {
        var button = e.target.GetComponent<Button>();
        if (button != null)
        {
            button.Select();
            Debug.Log("HandlePointerIn", e.target.gameObject);
        }
    }

    private void HandlePointerOut(object sender, DestinationMarkerEventArgs e)
    {
        var button = e.target.GetComponent<Button>();
        if (button != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            Debug.Log("HandlePointerOut", e.target.gameObject);
        }
    }
}
