using System;
using UnityEngine;

namespace GameScripts.Scripts.GameLogic
{
    public sealed class PlayerSpawnPos : MonoBehaviour
    {
        [SerializeField] private Vector3 _direction;
        public Vector3 Direction => _direction;
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position ,transform.position + _direction );
            Gizmos.DrawCube(transform.position + _direction , Vector3.one * .1f); 
        }
    }
}
