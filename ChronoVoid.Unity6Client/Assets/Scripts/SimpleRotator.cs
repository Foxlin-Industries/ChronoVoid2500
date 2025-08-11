using UnityEngine;

namespace ChronoVoid.Client
{
    /// <summary>
    /// Simple rotation script for testing visibility
    /// </summary>
    public class SimpleRotator : MonoBehaviour
    {
        public float rotationSpeed = 90f;
        
        private void Update()
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
    }
}