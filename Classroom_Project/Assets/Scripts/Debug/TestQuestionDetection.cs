using UnityEngine;

/// <summary>
/// Simple script to manually test question detection
/// Attach this to any GameObject and press T during play mode to trigger a test question
/// </summary>
public class TestQuestionDetection : MonoBehaviour
{
    void Update()
    {
        // Press T key to test question
        if (Input.GetKeyDown(KeyCode.T))
        {
            TestQuestion();
        }
    }

    void TestQuestion()
    {
        Debug.Log("[TEST] Manually triggering question...");

        // Find all students
        StudentAgent[] students = FindObjectsOfType<StudentAgent>();
        Debug.Log($"[TEST] Found {students.Length} students");

        string testQuestion = "מה התשובה?";
        int responsiveCount = 0;

        foreach (StudentAgent student in students)
        {
            // Get or add StudentQuestionResponder
            StudentQuestionResponder responder = student.GetComponent<StudentQuestionResponder>();
            if (responder == null)
            {
                Debug.Log($"[TEST] Adding StudentQuestionResponder to {student.studentName}");
                responder = student.gameObject.AddComponent<StudentQuestionResponder>();
            }

            // Check if student has response bubble
            StudentResponseBubble bubble = student.GetComponent<StudentResponseBubble>();
            if (bubble == null)
            {
                Debug.LogWarning($"[TEST] {student.studentName} is MISSING StudentResponseBubble! Adding it...");
                bubble = student.gameObject.AddComponent<StudentResponseBubble>();
            }

            // Trigger question
            responder.OnQuestionAsked(testQuestion);

            if (responder.HasAnswerReady())
            {
                responsiveCount++;
                Debug.Log($"[TEST] {student.studentName} is eager to answer!");
            }
        }

        Debug.Log($"[TEST] {responsiveCount}/{students.Length} students want to answer");

        if (responsiveCount == 0)
        {
            Debug.LogWarning("[TEST] NO STUDENTS RESPONDED! Check:");
            Debug.LogWarning("1. Students have StudentResponseBubble component");
            Debug.LogWarning("2. Students have high Academic Motivation (>0.5)");
            Debug.LogWarning("3. Base Response Threshold is low (<0.5)");
        }
    }
}
