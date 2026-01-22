using UnityEngine;
using TMPro;

/// <summary>
/// Simple UI component that displays brief student information.
/// Shows basic student data without a panel.
/// </summary>
public class StudentInfoPanelUI : MonoBehaviour
{
    private StudentAgent currentStudent;

    /// <summary>
    /// Show student info (minimal implementation - data is shown in TeacherUI's selectedStudentText)
    /// </summary>
    public void ShowStudentInfo(StudentAgent student)
    {
        if (student == null)
        {
            Debug.LogWarning("StudentInfoPanelUI: Cannot show info for null student");
            return;
        }

        currentStudent = student;
        // Info is displayed in TeacherUI's selectedStudentText, not here
    }

    /// <summary>
    /// Close panel (minimal implementation)
    /// </summary>
    public void ClosePanel()
    {
        currentStudent = null;
    }

    /// <summary>
    /// Get the currently selected student
    /// </summary>
    public StudentAgent GetCurrentStudent()
    {
        return currentStudent;
    }

    /// <summary>
    /// Check if showing a student
    /// </summary>
    public bool IsShowingStudent()
    {
        return currentStudent != null;
    }
}
