using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TeacherBagUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject bagPanel;

    [Header("Refs")]
    public ClassroomManager classroomManager;

    public void ToggleBag()
    {
        if (bagPanel == null) return;
        bagPanel.SetActive(!bagPanel.activeSelf);
    }

    // Hook this to item buttons using an int parameter (0..3)
    public void UseItem(int itemType)
    {
        if (classroomManager == null) return;

        BagItemType item = (BagItemType)itemType;
        classroomManager.ExecuteBagItem(item);
    }
}
