using UnityEngine;
using Unity.Profiling;

namespace ChronoVoid.Client
{
    public class PerformanceMonitor : MonoBehaviour
    {
        [Header("Performance Settings")]
        public bool enableProfiling = true;
        public bool showFPS = true;
        
        // Unity 6 ProfilerRecorder for enhanced profiling
        private ProfilerRecorder mainThreadTimeRecorder;
        private ProfilerRecorder renderThreadTimeRecorder;
        private ProfilerRecorder memoryRecorder;
        
        private float deltaTime = 0f;
        private GUIStyle style;

        private void Start()
        {
            if (enableProfiling)
            {
                // Unity 6 enhanced profiling setup
                mainThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 15);
                renderThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Render Thread", 15);
                memoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Used Memory", 15);
                
                Debug.Log("Performance monitoring enabled for Unity 6", this);
            }
        }

        private void Update()
        {
            if (showFPS)
            {
                deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            }
        }

        private void OnDestroy()
        {
            if (enableProfiling)
            {
                mainThreadTimeRecorder.Dispose();
                renderThreadTimeRecorder.Dispose();
                memoryRecorder.Dispose();
            }
        }

        private void OnGUI()
        {
            if (!showFPS) return;

            if (style == null)
            {
                style = new GUIStyle();
                style.alignment = TextAnchor.UpperLeft;
                style.fontSize = Screen.height * 2 / 100;
                style.normal.textColor = Color.white;
            }

            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            string text = $"FPS: {fps:0.} ({msec:0.0} ms)";
            
            if (enableProfiling && memoryRecorder.Valid)
            {
                long memoryBytes = memoryRecorder.LastValue;
                float memoryMB = memoryBytes / (1024.0f * 1024.0f);
                text += $"\nMemory: {memoryMB:F1} MB";
            }
            
            GUI.Label(new Rect(10, 10, 200, 100), text, style);
        }

        // Unity 6 enhanced method for getting performance stats
        public float GetAverageMainThreadTime()
        {
            if (!mainThreadTimeRecorder.Valid || mainThreadTimeRecorder.Count == 0)
                return 0f;
                
            return (float)mainThreadTimeRecorder.LastValue / 1000000f; // Convert to milliseconds
        }

        public long GetMemoryUsage()
        {
            if (!memoryRecorder.Valid)
                return 0;
                
            return memoryRecorder.LastValue;
        }
    }
}